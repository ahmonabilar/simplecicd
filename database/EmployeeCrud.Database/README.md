# EmployeeCrud.Database

This Visual Studio SSDT SQL project builds a DACPAC for publishing the Employee CRUD schema and seed data to SQL Server or Azure SQL.

Open the root `EmployeeCrud.sln` in Visual Studio. The database project uses the original SSDT project format for broad Visual Studio compatibility.

Build:

```powershell
msbuild database\EmployeeCrud.Database\EmployeeCrud.Database.sqlproj /p:Configuration=Release /p:Platform="Any CPU"
```

Publish with SqlPackage:

```powershell
sqlpackage /Action:Publish /SourceFile:database\EmployeeCrud.Database\bin\Release\EmployeeCrud.Database.dacpac /Profile:database\EmployeeCrud.Database\PublishProfiles\Local.publish.xml
```

The web app uses SQLite by default for local simplicity. Set `Database:Provider` to `SqlServer` and provide a SQL Server connection string in `appsettings.json` when pointing the app at a published SQL database.
