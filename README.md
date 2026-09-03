# Tenisu API

A small ASP.NET Core 10 Web API that manages tennis players and exposes a few statistics over them.
Started as a technical test, grew into a portfolio project used to explore modern .NET, layered architecture, and an Azure-ready configuration story.

## What it does

- CRUD-lite over players (paged listing, single lookup, add).
- Aggregate statistics: average BMI, median height, most successful country.
- Structured logging to console and Application Insights, health check on `/health`.

## Solution layout

```
src/
  Tenisu.Domain           - Entities, value objects, domain interfaces & exceptions.
  Tenisu.Application      - Use cases (services), DTOs, mapping (Mapperly).
  Tenisu.Infrastructure   - EF Core DbContext, repositories, seeding, migrations.
  Tenisu.WebApi           - Controllers, middleware, DI.
Tests/
  Tenisu.Application.Tests    - Unit tests (NUnit + Moq).
  Tenisu.Infrastructure.Tests - Integration tests against in-memory SQLite.
```

Dependencies flow inward: `WebApi` → `Application` → `Domain`, and `WebApi` → `Infrastructure` → `Domain`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (ships with Visual Studio) or any reachable SQL Server instance — the app was built with SQL Server in mind
- Visual Studio 2026 (or VS Code with the C# Dev Kit)

## Configuration

Configuration providers, in order of precedence (last wins):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Azure Key Vault *(when `KeyVaultUri` is set)*
4. Environment variables
5. User secrets *(Development only)*
6. Command-line arguments

Key settings:

| Key | Purpose |
|---|---|
| `ConnectionStrings:TenisuDB` | SQL Server connection string. Defaults to a LocalDB instance in `appsettings.Development.json` — change the value there if you want to point at another server, or override it via user secrets / environment variables. |
| `KeyVaultUri` | Optional. If empty, Azure Key Vault is skipped entirely. If set, secrets are loaded via `DefaultAzureCredential`. |
| `EnableDatabaseMigration` | Runs EF Core migrations + seeding at startup. Handy locally; would be turned off in production if a proper deployment pipeline handled the schema. |
| `Serilog:*` | Sinks (console + Application Insights) and log levels. |
| `RateLimiting:PermitLimit` / `Window` | Global fixed-window rate limiter. |

## Running locally

Defaults are already set up for a zero-config local run:

- `KeyVaultUri` is **empty** in `appsettings.json`, so the app skips Azure Key Vault entirely.
- `EnableDatabaseMigration` is **`true`**, so EF Core migrations are applied and the initial data set is seeded on startup.
- The connection string in `appsettings.Development.json` points at **SQL Server LocalDB**. Edit that value directly if you want to target another SQL Server instance, or override it via user secrets.

Then:

1. Clone and restore:
   ```powershell
   git clone https://github.com/WombatTheGrey/Tenisu.git
   cd Tenisu
   dotnet restore
   ```
2. (Optional) override the connection string with **user secrets** instead of editing the file:
   ```powershell
   cd src\Tenisu.WebApi
   dotnet user-secrets set "ConnectionStrings:TenisuDB" "<your-connection-string>"
   ```
3. Run the API:
   ```powershell
   dotnet run --project src\Tenisu.WebApi
   ```
4. Swagger UI: <https://localhost:59599/swagger>
5. Health probe: <https://localhost:59599/health>

## Running the tests

```powershell
dotnet test
```

Unit tests are tagged `[Category("Unit")]`; integration tests `[Category("Integration")]` and use SQLite in-memory.

## Observability

- **Serilog** with two sinks: Console and Application Insights. Sink names are keyed (not indexed) so the AI connection string can be overridden from Key Vault via the secret `Serilog--WriteTo--appInsights--Args--connectionString`.
- **`/health`** endpoint backed by `AddDbContextCheck<TenisuDbContext>` — verifies DB connectivity.
- Per-request structured logs via `UseSerilogRequestLogging`.

## Running in Azure

A live instance is deployed at <https://tenisuwebapp-dncvcshwbgcfaya0.francecentral-01.azurewebsites.net> (Swagger: <https://tenisuwebapp-dncvcshwbgcfaya0.francecentral-01.azurewebsites.net/swagger>).

The app runs as a **single instance** on Azure, on the following resources:

- **Azure App Service** (Web App, Free tier) — hosts the API. The Key Vault URI is injected as an environment variable (`KeyVaultUri`) so the app boots up wired to Key Vault without any config file change.
- **Azure Key Vault** — holds the SQL connection string and the Application Insights connection string.
- **Application Insights** — telemetry and Serilog sink.
- **Azure SQL Server / Database** — the persistence layer.

**No credentials are stored or handled anywhere in the app or its configuration.** Every Azure-to-Azure hop is authenticated via **managed identity**:

- The App Service uses its **system-assigned managed identity** to authenticate to Key Vault (via `DefaultAzureCredential`) and to Azure SQL.
- Access is granted through **Azure RBAC role assignments** (Key Vault Secrets User on the vault, and an Entra-based SQL user on the database) — no keys, no connection-string passwords, no secrets in App Service settings beyond the plain `KeyVaultUri`.


## Possible next steps

I'm aware of some gaps but I'm not actively developing this project right now. Things I'd look at if I picked it back up:

- **Containerization** — package the API as a container image with Docker and run it on App Service for Containers.
- **CI/CD pipeline** — build, test and deploy from GitHub Actions.
- **Move migrations out of the app** — if a CI/CD pipeline is added, `EnableDatabaseMigration` would be turned off and the schema applied from the pipeline via an EF Core migration bundle instead of at app startup.

## License

MIT.



