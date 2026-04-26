using EmployeeCrud.Application.Employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Application.Data;

public sealed class EmployeeAccessor(EmployeeDbContext dbContext) : IEmployeeAccessor
{
    public async Task<IReadOnlyList<Employee>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<Employee?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludingEmployeeId = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return dbContext.Employees.AnyAsync(
            employee => employee.Email == normalizedEmail
                && (!excludingEmployeeId.HasValue || employee.Id != excludingEmployeeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<bool> HasAnyAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AnyAsync(cancellationToken);
    }

    public async Task<int> AddRangeAsync(IEnumerable<Employee> employees, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employees);

        var employeeList = employees.ToList();
        dbContext.Employees.AddRange(employeeList);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return employeeList.Count;
    }
}
