using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Application.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeAsync(int id, CancellationToken cancellationToken = default);

    Task<Employee> CreateEmployeeAsync(EmployeeInput input, CancellationToken cancellationToken = default);

    Task<Employee?> UpdateEmployeeAsync(int id, EmployeeInput input, CancellationToken cancellationToken = default);

    Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default);

    Task<int> SeedEmployeesAsync(CancellationToken cancellationToken = default);
}
