# Employee CRUD

Simple ASP.NET Core Razor Pages application for managing employee records on a single page.

## Projects

- `src/EmployeeCrud.Application` - employee entity, EF Core database context, data accessor layer, service layer, and seed data.
- `src/EmployeeCrud.Web` - Bootstrap Razor Pages UI, Serilog configuration, SQLite/SQL Server database registration, and TypeScript-backed browser interaction.
- `database/EmployeeCrud.Database` - Visual Studio SSDT SQL project that builds a DACPAC with the `Employees` schema and post-deployment seed script.
- `tests/EmployeeCrud.Tests` - NUnit tests for the application layer with coverlet coverage collection.

Open `EmployeeCrud.sln` in Visual Studio for the most compatible database-project experience.

## Run The Web App

```powershell
dotnet restore src\EmployeeCrud.Web\EmployeeCrud.Web.csproj --configfile NuGet.config
dotnet run --project src\EmployeeCrud.Web\EmployeeCrud.Web.csproj
```

The app defaults to SQLite and creates `employeecrud.db` under `src/EmployeeCrud.Web`. It seeds 10 employee records on first startup.

## Logging

Serilog is configured in `src/EmployeeCrud.Web/appsettings.json`.

- Console sink is enabled.
- File sink writes rolling logs to `src/EmployeeCrud.Web/logs`.
- Grafana Loki can be enabled by setting `GrafanaLoki:Enabled` to `true` and configuring `GrafanaLoki:Endpoint`.

## Database Publishing

Build the DACPAC:

```powershell
msbuild database\EmployeeCrud.Database\EmployeeCrud.Database.sqlproj /p:Configuration=Release /p:Platform="Any CPU"
```

Publish with SqlPackage:

```powershell
sqlpackage /Action:Publish /SourceFile:database\EmployeeCrud.Database\bin\Release\EmployeeCrud.Database.dacpac /Profile:database\EmployeeCrud.Database\PublishProfiles\Local.publish.xml
```

Set `Database:Provider` to `SqlServer` and update `Database:ConnectionString` if the web app should use the published SQL Server database.

## Tests And Coverage

```powershell
dotnet test tests\EmployeeCrud.Tests\EmployeeCrud.Tests.csproj --configuration Release --collect:"XPlat Code Coverage"
```

The current suite covers the application layer at 100% line and branch coverage.
