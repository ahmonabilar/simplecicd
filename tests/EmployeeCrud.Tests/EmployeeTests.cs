using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Tests;

public sealed class EmployeeTests
{
    [Test]
    public void Constructor_NormalizesValues()
    {
        var employee = new Employee(
            "  Jane  ",
            "  Doe  ",
            "  JANE.DOE@EXAMPLE.COM  ",
            "  Engineering Manager  ",
            "  Engineering  ",
            new DateTime(2024, 4, 20, 14, 30, 0),
            125000m);

        Assert.Multiple(() =>
        {
            Assert.That(employee.FirstName, Is.EqualTo("Jane"));
            Assert.That(employee.LastName, Is.EqualTo("Doe"));
            Assert.That(employee.Email, Is.EqualTo("jane.doe@example.com"));
            Assert.That(employee.JobTitle, Is.EqualTo("Engineering Manager"));
            Assert.That(employee.Department, Is.EqualTo("Engineering"));
            Assert.That(employee.HireDate, Is.EqualTo(new DateTime(2024, 4, 20)));
            Assert.That(employee.Salary, Is.EqualTo(125000m));
            Assert.That(employee.FullName, Is.EqualTo("Jane Doe"));
        });
    }

    [Test]
    public void Constructor_RejectsMissingRequiredText()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Employee("", "Doe", "jane.doe@example.com", "Engineer", "Engineering", DateTime.Today, 100000m));

        Assert.That(exception?.ParamName, Is.EqualTo("firstName"));
    }

    [Test]
    public void Constructor_RejectsOverMaxLengthText()
    {
        var firstName = new string('A', Employee.FirstNameMaxLength + 1);

        var exception = Assert.Throws<ArgumentException>(() =>
            new Employee(firstName, "Doe", "jane.doe@example.com", "Engineer", "Engineering", DateTime.Today, 100000m));

        Assert.That(exception?.Message, Does.Contain(Employee.FirstNameMaxLength.ToString(CultureInfo.InvariantCulture)));
    }

    [Test]
    public void Constructor_RejectsInvalidEmail()
    {
        Assert.Throws<ValidationException>(() =>
            new Employee("Jane", "Doe", "not-an-email", "Engineer", "Engineering", DateTime.Today, 100000m));
    }

    [Test]
    public void Constructor_RejectsInvalidSalary()
    {
        Assert.Throws<ValidationException>(() =>
            new Employee("Jane", "Doe", "jane.doe@example.com", "Engineer", "Engineering", DateTime.Today, 0m));
    }

    [Test]
    public void SeedData_ContainsTenUniqueEmployees()
    {
        var employees = EmployeeSeedData.CreateEmployees();
        var distinctEmails = employees.Select(employee => employee.Email).Distinct().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(employees, Has.Count.EqualTo(10));
            Assert.That(distinctEmails, Has.Count.EqualTo(10));
            Assert.That(employees.All(employee => employee.Salary > 0), Is.True);
        });
    }
}
