using System.Globalization;
using EmployeeCrud.Application.Data;
using EmployeeCrud.Application.Services;
using EmployeeCrud.Web.Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();

        var grafanaOptions = context.Configuration.GetSection(GrafanaLokiOptions.SectionName).Get<GrafanaLokiOptions>();
        if (grafanaOptions?.Enabled == true)
        {
            loggerConfiguration.WriteTo.Sink(new GrafanaLokiSink(grafanaOptions));
        }
    });

    builder.Services.AddRazorPages();
    builder.Services.AddHealthChecks();
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")));
    builder.Services.AddDbContext<EmployeeDbContext>(options => ConfigureDatabase(options, builder.Configuration));
    builder.Services.AddScoped<IEmployeeAccessor, EmployeeAccessor>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();

    var app = builder.Build();

    await InitializeDatabaseAsync(app);

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    if (app.Configuration.GetValue("HttpsRedirection:Enabled", false))
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();

    app.UseSerilogRequestLogging();
    app.UseRouting();

    app.MapHealthChecks("/healthz");
    app.MapRazorPages();

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "The application failed to start.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void ConfigureDatabase(DbContextOptionsBuilder options, IConfiguration configuration)
{
    var provider = configuration.GetValue<string>("Database:Provider") ?? "Sqlite";
    var connectionString = configuration.GetValue<string>("Database:ConnectionString")
        ?? "Data Source=employeecrud.db";

    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
        return;
    }

    options.UseSqlite(connectionString);
}

static async Task InitializeDatabaseAsync(WebApplication app)
{
    var ensureCreated = app.Configuration.GetValue("Database:EnsureCreatedOnStartup", true);
    if (!ensureCreated)
    {
        return;
    }

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
    var seededCount = await employeeService.SeedEmployeesAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
    ApplicationLog.DatabaseReady(logger, seededCount);
}

internal static class ApplicationLog
{
    private static readonly Action<Microsoft.Extensions.Logging.ILogger, int, Exception?> DatabaseReadyMessage =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(2000, nameof(DatabaseReady)), "Database is ready. Seeded {SeededCount} employee records.");

    public static void DatabaseReady(Microsoft.Extensions.Logging.ILogger logger, int seededCount)
    {
        DatabaseReadyMessage(logger, seededCount, null);
    }
}
