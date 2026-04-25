using EmployeeCrud.Application.Data;
using EmployeeCrud.Application.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Tests;

internal sealed class TestEmployeeDatabase : IAsyncDisposable
{
    private TestEmployeeDatabase(SqliteConnection connection, EmployeeDbContext dbContext)
    {
        Connection = connection;
        DbContext = dbContext;
        Accessor = new EmployeeAccessor(dbContext);
        Service = new EmployeeService(Accessor);
    }

    public EmployeeDbContext DbContext { get; }

    public EmployeeAccessor Accessor { get; }

    public EmployeeService Service { get; }

    private SqliteConnection Connection { get; }

    public static async Task<TestEmployeeDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new EmployeeDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        return new TestEmployeeDatabase(connection, dbContext);
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
