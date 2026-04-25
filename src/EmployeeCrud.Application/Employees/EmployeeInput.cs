using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Application.Employees;

public sealed class EmployeeInput
{
    [Required]
    [StringLength(Employee.FirstNameMaxLength)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(Employee.LastNameMaxLength)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(Employee.EmailMaxLength)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(Employee.JobTitleMaxLength)]
    [Display(Name = "Job title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(Employee.DepartmentMaxLength)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Hire date")]
    public DateTime HireDate { get; set; } = DateTime.Today;

    [Range(0.01, 10000000)]
    public decimal Salary { get; set; }
}
