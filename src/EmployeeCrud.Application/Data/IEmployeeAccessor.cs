using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Application.Data;

public interface IEmployeeAccessor
{
    Task<IReadOnlyList<Employee>> ListAsync(CancellationToken cancellationToken = default);

    Task<Employee?> FindAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, int? excludingEmployeeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<bool> HasAnyAsync(CancellationToken cancellationToken = default);

    Task<int> AddRangeAsync(IEnumerable<Employee> employees, CancellationToken cancellationToken = default);
}
