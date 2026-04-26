using EmployeeCrud.Application.Employees;
using EmployeeCrud.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeCrud.Web.Pages;

public sealed class IndexModel(IEmployeeService employeeService, ILogger<IndexModel> logger) : PageModel
{
    private static readonly Action<ILogger, int, Exception?> EmployeeUpdated =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(1000, nameof(EmployeeUpdated)), "Updated employee {EmployeeId}.");

    private static readonly Action<ILogger, int, Exception?> EmployeeCreated =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(1001, nameof(EmployeeCreated)), "Created employee {EmployeeId}.");

    private static readonly Action<ILogger, int, Exception?> EmployeeDeleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(1002, nameof(EmployeeDeleted)), "Deleted employee {EmployeeId}.");

    private static readonly Action<ILogger, Exception?> EmployeeSaveFailed =
        LoggerMessage.Define(LogLevel.Warning, new EventId(1003, nameof(EmployeeSaveFailed)), "Employee save failed.");

    public IReadOnlyList<Employee> Employees { get; private set; } = [];

    public Employee? SelectedEmployee { get; private set; }

    public bool IsEditing => EmployeeId.HasValue;

    [BindProperty]
    public int? EmployeeId { get; set; }

    [BindProperty]
    public EmployeeInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(int? editId, int? viewId)
    {
        await LoadPageDataAsync(editId, viewId);
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync(EmployeeId, viewId: null, preserveInput: true);
            return Page();
        }

        try
        {
            if (EmployeeId is int employeeId)
            {
                var updatedEmployee = await employeeService.UpdateEmployeeAsync(employeeId, Input);
                if (updatedEmployee is null)
                {
                    ErrorMessage = "The selected employee could not be found.";
                    return RedirectToPage();
                }

                EmployeeUpdated(logger, employeeId, null);
                StatusMessage = $"{updatedEmployee.FullName} was updated.";
                return RedirectToPage(new { viewId = employeeId });
            }

            var createdEmployee = await employeeService.CreateEmployeeAsync(Input);
            EmployeeCreated(logger, createdEmployee.Id, null);
            StatusMessage = $"{createdEmployee.FullName} was added.";
            return RedirectToPage(new { viewId = createdEmployee.Id });
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or System.ComponentModel.DataAnnotations.ValidationException)
        {
            EmployeeSaveFailed(logger, exception);
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadPageDataAsync(EmployeeId, viewId: null, preserveInput: true);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var deleted = await employeeService.DeleteEmployeeAsync(id);
        if (deleted)
        {
            EmployeeDeleted(logger, id, null);
            StatusMessage = "The employee was deleted.";
        }
        else
        {
            ErrorMessage = "The selected employee could not be found.";
        }

        return RedirectToPage();
    }

    private async Task LoadPageDataAsync(int? editId = null, int? viewId = null, bool preserveInput = false)
    {
        Employees = await employeeService.GetEmployeesAsync();

        if (!preserveInput)
        {
            Input = new EmployeeInput { HireDate = DateTime.Today };
            EmployeeId = null;
        }

        if (editId is int employeeId)
        {
            var employee = await employeeService.GetEmployeeAsync(employeeId);
            if (employee is null)
            {
                ErrorMessage = "The selected employee could not be found.";
                return;
            }

            EmployeeId = employee.Id;
            SelectedEmployee = employee;

            if (!preserveInput)
            {
                Input = ToInput(employee);
            }

            return;
        }

        if (viewId is int selectedEmployeeId)
        {
            SelectedEmployee = await employeeService.GetEmployeeAsync(selectedEmployeeId);
            if (SelectedEmployee is null)
            {
                ErrorMessage = "The selected employee could not be found.";
            }
        }
    }

    private static EmployeeInput ToInput(Employee employee)
    {
        return new EmployeeInput
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            JobTitle = employee.JobTitle,
            Department = employee.Department,
            HireDate = employee.HireDate,
            Salary = employee.Salary
        };
    }
}
