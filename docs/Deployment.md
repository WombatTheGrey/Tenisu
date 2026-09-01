# Deploying Tenisu to Azure

Portal-only walk-through. No Azure CLI, no ARM/Bicep. Target architecture:

```
             ┌──────────────────────┐
Client ──► Azure App Service ─┬─► Instance #1 ─┐
    (platform load balancer)  │                 ├─► Azure SQL Database
                              └─► Instance #2 ─┘
                                       │
                                       ├─► Azure Key Vault  (secrets)
                                       └─► Application Insights (telemetry)
```

The two App Service instances live under one plan and are balanced by the App Service front-end. That covers the "round-robin between two instances" requirement without additional infrastructure. See the [Optional: Front Door](#optional-explicit-round-robin-with-front-door) section for a stricter round-robin story.

---

## Prerequisites

- Azure subscription with permissions to create resources.
- A resource group. Create one now: `rg-tenisu`.
- Your Entra ID account will be used to bootstrap SQL access.

---

## Step 1 — Azure SQL Database

1. Portal → **Create a resource** → *SQL Database*.
2. Resource group: `rg-tenisu`. Database name: `tenisu-db`.
3. Server → **Create new**: `sql-tenisu-<yourname>`. Region: same as everything else.
4. Authentication: **Use Microsoft Entra-only authentication**. Set yourself as admin.
5. Compute + storage: **Basic** or **Serverless** — cheapest tier is fine.
6. Networking → **Public endpoint**, allow Azure services, add your client IP.
7. Review + create.

After creation:

- SQL server → **Networking** → confirm your IP is in the firewall (needed for the migration step from your laptop).
- SQL server → **Microsoft Entra ID** → note the admin name (your account).

---

## Step 2 — Key Vault

1. Portal → **Create a resource** → *Key Vault*.
2. Resource group: `rg-tenisu`. Name: `kv-tenisu-<yourname>` (globally unique).
3. Permission model: **Azure role-based access control (RBAC)**.
4. Networking: public endpoint (fine for a demo).
5. Review + create.

Grant yourself access:

- Key Vault → **Access control (IAM)** → Add role assignment → **Key Vault Secrets Officer** → your user.

Add the secrets (Secrets → Generate/Import):

| Secret name | Value |
|---|---|
| `ConnectionStrings--TenisuDB` | `Server=tcp:sql-tenisu-yourname.database.windows.net,1433;Database=tenisu-db;Authentication=Active Directory Default;Encrypt=True;` |
| `Serilog--WriteTo--appInsights--Args--connectionString` | *(Application Insights connection string — set in Step 3)* |

The `--` in Key Vault secret names maps to `:` in .NET configuration, so these override the corresponding `appsettings.json` values.

---

## Step 3 — Application Insights

1. Portal → **Create a resource** → *Application Insights*.
2. Resource group: `rg-tenisu`. Name: `appi-tenisu`.
3. Resource mode: **Workspace-based**. Create a new Log Analytics workspace if needed.
4. Review + create.
5. Once created → **Overview** → copy the **Connection String**.
6. Paste it into the Key Vault secret `Serilog--WriteTo--appInsights--Args--connectionString`.

---

## Step 4 — App Service Plan + Web App

1. Portal → **Create a resource** → *Web App*.
2. Resource group: `rg-tenisu`. Name: `app-tenisu-<yourname>`.
3. Publish: **Code**. Runtime: **.NET 10**. OS: Linux (cheaper) or Windows.
4. **App Service Plan** → Create new: `plan-tenisu`. Pricing plan: **S1** (Standard). S1 is the minimum for scale-out with an SLA.
5. Deployment: skip GitHub Actions for now (we deploy from Visual Studio).
6. Networking: public.
7. Monitoring: link `appi-tenisu`.
8. Review + create.

---

## Step 5 — Managed identity + role assignments

1. Web App → **Identity** → System-assigned → **On** → Save. Copy the object (principal) ID.

Grant the identity access to Key Vault:

- Key Vault (`kv-tenisu-…`) → Access control (IAM) → Add role assignment → **Key Vault Secrets User** → Managed identity → select `app-tenisu-…`.

Grant the identity access to the DB. Connect to the DB with SSMS, Azure Data Studio, or the portal query editor as your Entra admin and run:

```sql
CREATE USER [app-tenisu-yourname] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [app-tenisu-yourname];
ALTER ROLE db_datawriter ADD MEMBER [app-tenisu-yourname];
-- Only if the app is allowed to run migrations at startup:
-- ALTER ROLE db_ddladmin ADD MEMBER [app-tenisu-yourname];
```

The user name must match the App Service name exactly.

---

## Step 6 — App Service configuration

Web App → **Environment variables → Application settings**. Add:

| Name | Value |
|---|---|
| `KeyVaultUri` | `https://kv-tenisu-yourname.vault.azure.net/` |
| `EnableDatabaseMigration` | `false` |
| `AllowedHosts` | `app-tenisu-yourname.azurewebsites.net` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

Save → the app restarts.

At startup:

- `ConfigureAzureKeyVault` reads `KeyVaultUri`.
- `DefaultAzureCredential` picks up the App Service managed identity.
- The SQL connection string and Application Insights connection string come from Key Vault.
- Everything else stays in `appsettings.json`.

---

## Step 7 — Migrate the database *(before publishing the app)*

`EnableDatabaseMigration` is `false` in App Service, so the app will not run migrations on startup. Apply them out-of-band with an **EF Core migrations bundle**.

From your dev machine:

```powershell
dotnet ef migrations bundle `
  --project src\Tenisu.Infrastructure `
  --startup-project src\Tenisu.WebApi `
  --self-contained -r win-x64 `
  -o .\artifacts\tenisu-migrator.exe
```

Apply against Azure SQL (uses your local Entra login):

```powershell
.\artifacts\tenisu-migrator.exe `
  --connection "Server=tcp:sql-tenisu-yourname.database.windows.net,1433;Database=tenisu-db;Authentication=Active Directory Default;Encrypt=True;"
```

The bundle triggers the same `UseAsyncSeeding` code path as the app, so the initial data is seeded automatically on first run.

### How it fits with the deployment

- **Deploy order**: run `tenisu-migrator.exe` first, then publish the app (Step 8).
- **Rollbacks**: generate the bundle from the same source revision as the app version being deployed so schema and code stay aligned.
- **Where to run it**: for a manual/showcase workflow, from your laptop is fine. In a CI/CD pipeline, run it as a job step before deploying the app.

### Alternative — in-solution console utility

If you prefer explicit code over the EF tooling, add a `src\Tenisu.Migrator` console project referencing `Tenisu.Infrastructure`:

```csharp
using Microsoft.EntityFrameworkCore;
using Tenisu.Infrastructure.Context;
using Tenisu.Infrastructure.Initialization;

var connectionString = args.FirstOrDefault()
    ?? Environment.GetEnvironmentVariable("TENISUDB_CONNECTION")
    ?? throw new InvalidOperationException("Provide connection string as arg or TENISUDB_CONNECTION.");

var options = new DbContextOptionsBuilder<TenisuDbContext>()
    .UseSqlServer(connectionString)
    .UseAsyncSeeding(async (db, _, ct) =>
        await TenisuSeeder.SeedAsync((TenisuDbContext)db, ct))
    .Options;

using var ctx = new TenisuDbContext(options);
await ctx.Database.MigrateAsync();
Console.WriteLine("Migration + seeding complete.");
```

Publish it as a single-file exe and run it with the same argument as the bundle. Both approaches slot into the deployment identically.

**Recommendation:** stick with the EF bundle — zero extra code, always aligned with your `Migrations/` folder, and the officially recommended pattern for this scenario.

---

## Step 8 — Deploy from Visual Studio

1. Right-click `Tenisu.WebApi` → **Publish**.
2. Target: **Azure** → **Azure App Service (Linux/Windows)** matching your plan.
3. Sign in, select `app-tenisu-…`, Finish.
4. Click **Publish**.

Once deployed, hit `https://app-tenisu-yourname.azurewebsites.net/health` — you should get `Healthy`.

---

## Step 9 — Scale to two instances

- Web App → **Scale out (App Service plan)** → **Manual scale** → **Instance count = 2** → Save.
- Web App → **Configuration** → General settings → **Session affinity: Off** *(otherwise the ARR cookie sticks users to one instance and you effectively have no LB)*.

The App Service front-end now distributes incoming requests across both instances. Its balancing algorithm is proprietary (roughly least-connections with health awareness); for guaranteed round-robin, see the next section.

---

## Optional — explicit round-robin with Front Door

Only needed if the "round-robin" wording is strict:

1. Portal → **Front Door and CDN profiles** → Create → **Azure Front Door Standard**.
2. Add an origin group; origin = your App Service hostname.
3. Load balancing → switch the algorithm to **Round-robin** (available on Standard/Premium).
4. Update `AllowedHosts` in App Service configuration to include the Front Door endpoint hostname.
5. Optional: lock down the App Service to only accept traffic from Front Door (Access Restrictions → `AzureFrontDoor.Backend` service tag + `X-Azure-FDID` header match).

For a portfolio project, App Service scale-out alone is enough — Front Door is a talking point, not a hard requirement.

---

## Post-deployment checks

- `GET /health` → 200 `Healthy` (means DB connectivity is fine).
- `GET /swagger` → Swagger UI reachable.
- Application Insights → **Live metrics** shows request traffic; **Logs** contains Serilog entries.
- Kill one instance from **Advanced Tools → Kudu** to verify the other keeps serving.

## Cost & teardown

Every resource lives under `rg-tenisu`. Deleting the resource group removes the whole stack. Standard-tier App Service and Azure SQL are the two paying components; scale down / stop when you're not demoing.
