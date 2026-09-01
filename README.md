# Tenisu API

A small ASP.NET Core 10 Web API that manages tennis players and exposes a few statistics over them. Started as a technical test, grew into a portfolio project used to explore modern .NET, layered architecture, and an Azure-ready configuration story.

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
  Tenisu.WebApi           - Controllers, middleware, DI composition root.
Tests/
  Tenisu.Application.Tests    - Unit tests (NUnit + Moq).
  Tenisu.Infrastructure.Tests - Integration tests against in-memory SQLite.
```

Dependencies flow inward: `WebApi` → `Application` → `Domain`, and `WebApi` → `Infrastructure` → `Domain`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (ships with Visual Studio) or any reachable SQL Server instance
- Visual Studio 2026 (or VS Code with the C# Dev Kit)

## Running locally

1. Clone and restore:
   ```powershell
   git clone https://github.com/WombatTheGrey/Tenisu.git
   cd Tenisu
   dotnet restore
   ```
2. The default connection string in `appsettings.Development.json` targets LocalDB. To use your own server, override it via **user secrets** (recommended — never committed):
   ```powershell
   cd src\Tenisu.WebApi
   dotnet user-secrets set "ConnectionStrings:TenisuDB" "<your-connection-string>"
   ```
3. Run the API:
   ```powershell
   dotnet run --project src\Tenisu.WebApi
   ```
   On first launch, migrations are applied and initial data is seeded (`EnableDatabaseMigration=true` in `appsettings.json`).
4. Swagger UI: <https://localhost:59599/swagger>
5. Health probe: <https://localhost:59599/health>

## Running the tests

```powershell
dotnet test
```

Unit tests are tagged `[Category("Unit")]`; integration tests `[Category("Integration")]` and use SQLite in-memory.

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
| `ConnectionStrings:TenisuDB` | SQL Server connection string. |
| `KeyVaultUri` | Optional. If set, loads secrets from Azure Key Vault using `DefaultAzureCredential`. |
| `EnableDatabaseMigration` | Runs EF Core migrations + seeding at startup. Disable in production; use the migration bundle instead. |
| `Serilog:*` | Sinks (console + Application Insights) and log levels. |
| `RateLimiting:PermitLimit` / `Window` | Global fixed-window rate limiter. |

## Observability

- **Serilog** with two sinks: Console and Application Insights. Sink names are keyed (not indexed) so the AI connection string can be overridden from Key Vault via the secret `Serilog--WriteTo--appInsights--Args--connectionString`.
- **`/health`** endpoint backed by `AddDbContextCheck<TenisuDbContext>` — verifies DB connectivity.
- Per-request structured logs via `UseSerilogRequestLogging`.

## Deploying to Azure

The app is designed for Azure App Service with:
- Azure SQL Database (Entra-authenticated via managed identity)
- Azure Key Vault for secrets
- Application Insights for telemetry
- Two App Service instances behind the platform front-end for horizontal scale

DB schema is applied out-of-band using an EF Core migrations bundle, not at app startup. See [`docs/Deployment.md`](docs/Deployment.md) for the full portal walk-through.

## License

MIT.



