# AskErik.Api

Small API project for the AskErik solution (Blazor + API). This README explains how to run, configure, migrate the database, and deploy to Azure.

## Prerequisites
- .NET 10 SDK
- `dotnet-ef` tool matching the runtime (install/upgrade to 10.0.3)
  - `dotnet tool install --global dotnet-ef --version 10.0.3`
- PostgreSQL accessible from your machine or CI
- (Optional) Azure publish profile or service principal for deployments

## Local run (development)
1. Set the DB connection string (do not commit secrets). Example (bash):
```
export ConnectionStrings__tellerikdb="Host=...supabase.co;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
export ASPNETCORE_ENVIRONMENT=Development
```
2. Run:
```
dotnet restore AskErik.Api/AskErik.Api.csproj
dotnet build AskErik.Api/AskErik.Api.csproj
dotnet run --project AskErik.Api/AskErik.Api.csproj
```
3. Health endpoint: `GET /health` (e.g. `http://localhost:5000/health`)

## EF Core Migrations
- Ensure `ConnectionStrings__tellerikdb` is exported or provided to the design-time factory.
- Create a migration:
```
dotnet ef migrations add <Name> --project AskErik.Api/AskErik.Api.csproj --startup-project AskErik.Api/AskErik.Api.csproj
```
- Apply migrations:
```
dotnet ef database update --project AskErik.Api/AskErik.Api.csproj --startup-project AskErik.Api/AskErik.Api.csproj
```
Notes: The project includes (or should include) a `DesignTimeDbContextFactory` so `dotnet ef` can instantiate the `PostgresLessonContext` at design time without running the full app.

## Configuration & Secrets
- Application reads connection string via `builder.Configuration.GetConnectionString("tellerikdb")` or environment `ConnectionStrings__tellerikdb`.
- For Azure deployments prefer setting the connection string in the App Service configuration (Azure Portal or `az webapp config connection-string set`) instead of embedding secrets in `appsettings.json`.

## Deploy to Azure
- CI example: GitHub Actions workflow that builds/publishes `AskErik.Api` and deploys using the publish profile or `az webapp deploy`.
- For migrations in CI, run the `dotnet ef database update` step (using secrets as env vars) before or after deploying depending on your workflow.

## Troubleshooting
- If `/swagger/index.html` returns 404, check `Program.cs` — Swagger may be enabled only in `Development`.
- If EF tools fail to create the context, ensure `appsettings.json` is valid and the design-time factory is present.
- Use `az webapp log tail -g <RG> -n <APP_NAME>` or the App Service log stream to inspect runtime errors.

## Useful commands
- Build & publish:
```
dotnet publish AskErik.Api/AskErik.Api.csproj -c Release -o ./publish
```
- Update EF tools:
```
dotnet tool update --global dotnet-ef --version 10.0.3
```
