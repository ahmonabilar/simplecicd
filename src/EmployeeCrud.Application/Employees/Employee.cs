using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Application.Employees;

public sealed class Employee
{
    public const int FirstNameMaxLength = 80;
    public const int LastNameMaxLength = 80;
    public const int EmailMaxLength = 256;
    public const int JobTitleMaxLength = 120;
    public const int DepartmentMaxLength = 120;

    private Employee()
    {
    }

    public Employee(string firstName, string lastName, string email, string jobTitle, string department, DateTime hireDate, decimal salary)
    {
        Update(firstName, lastName, email, jobTitle, department, hireDate, salary);
    }

    public int Id { get; private set; }

    [Required]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; private set; } = string.Empty;

    [Required]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; private set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(EmailMaxLength)]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [MaxLength(JobTitleMaxLength)]
    public string JobTitle { get; private set; } = string.Empty;

    [Required]
    [MaxLength(DepartmentMaxLength)]
    public string Department { get; private set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime HireDate { get; private set; }

    [Range(0.01, 10000000)]
    public decimal Salary { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public void Update(string firstName, string lastName, string email, string jobTitle, string department, DateTime hireDate, decimal salary)
    {
        FirstName = CleanRequired(firstName, nameof(firstName), FirstNameMaxLength);
        LastName = CleanRequired(lastName, nameof(lastName), LastNameMaxLength);
        Email = CleanRequired(email, nameof(email), EmailMaxLength).ToLowerInvariant();
        JobTitle = CleanRequired(jobTitle, nameof(jobTitle), JobTitleMaxLength);
        Department = CleanRequired(department, nameof(department), DepartmentMaxLength);
        HireDate = hireDate.Date;
        Salary = salary;

        Validate();
    }

    private void Validate()
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this);

        if (!Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true))
        {
            throw new ValidationException(validationResults[0].ErrorMessage);
        }
    }

    private static string CleanRequired(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", parameterName);
        }

        var trimmedValue = value.Trim();
        if (trimmedValue.Length > maxLength)
        {
            throw new ArgumentException($"The value cannot exceed {maxLength} characters.", parameterName);
        }

        return trimmedValue;
    }
}
