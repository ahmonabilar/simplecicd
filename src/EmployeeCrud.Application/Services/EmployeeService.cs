using System.ComponentModel.DataAnnotations;
using EmployeeCrud.Application.Data;
using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Application.Services;

public sealed class EmployeeService(IEmployeeAccessor employeeAccessor) : IEmployeeService
{
    public Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return employeeAccessor.ListAsync(cancellationToken);
    }

    public Task<Employee?> GetEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        return employeeAccessor.FindAsync(id, cancellationToken);
    }

    public async Task<Employee> CreateEmployeeAsync(EmployeeInput input, CancellationToken cancellationToken = default)
    {
        ValidateInput(input);
        await EnsureEmailIsAvailableAsync(input.Email, excludingEmployeeId: null, cancellationToken).ConfigureAwait(false);

        var employee = ToEmployee(input);
        await employeeAccessor.AddAsync(employee, cancellationToken).ConfigureAwait(false);

        return employee;
    }

    public async Task<Employee?> UpdateEmployeeAsync(int id, EmployeeInput input, CancellationToken cancellationToken = default)
    {
        ValidateInput(input);

        var employee = await employeeAccessor.FindAsync(id, cancellationToken).ConfigureAwait(false);
        if (employee is null)
        {
            return null;
        }

        await EnsureEmailIsAvailableAsync(input.Email, id, cancellationToken).ConfigureAwait(false);
        employee.Update(input.FirstName, input.LastName, input.Email, input.JobTitle, input.Department, input.HireDate, input.Salary);
        await employeeAccessor.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return employee;
    }

    public async Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeAccessor.FindAsync(id, cancellationToken).ConfigureAwait(false);
        if (employee is null)
        {
            return false;
        }

        await employeeAccessor.DeleteAsync(employee, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<int> SeedEmployeesAsync(CancellationToken cancellationToken = default)
    {
        if (await employeeAccessor.HasAnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return 0;
        }

        return await employeeAccessor.AddRangeAsync(EmployeeSeedData.CreateEmployees(), cancellationToken).ConfigureAwait(false);
    }

    private static Employee ToEmployee(EmployeeInput input)
    {
        return new Employee(input.FirstName, input.LastName, input.Email, input.JobTitle, input.Department, input.HireDate, input.Salary);
    }

    private static void ValidateInput(EmployeeInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Validator.ValidateObject(input, new ValidationContext(input), validateAllProperties: true);
    }

    private async Task EnsureEmailIsAvailableAsync(string email, int? excludingEmployeeId, CancellationToken cancellationToken)
    {
        if (await employeeAccessor.EmailExistsAsync(email, excludingEmployeeId, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"An employee with email '{email.Trim()}' already exists.");
        }
    }
}
