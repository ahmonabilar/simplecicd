using System.ComponentModel.DataAnnotations;
using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Tests;

public sealed class EmployeeServiceTests
{
    [Test]
    public async Task SeedEmployeesAsync_AddsSeedRecordsOnce()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();

        var firstSeedCount = await database.Service.SeedEmployeesAsync();
        var secondSeedCount = await database.Service.SeedEmployeesAsync();
        var employees = await database.Service.GetEmployeesAsync();

        Assert.Multiple(() =>
        {
            Assert.That(firstSeedCount, Is.EqualTo(10));
            Assert.That(secondSeedCount, Is.Zero);
            Assert.That(employees, Has.Count.EqualTo(10));
            Assert.That(employees[0].LastName, Is.EqualTo("Brown"));
        });
    }

    [Test]
    public async Task CreateEmployeeAsync_PersistsEmployee()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var input = ValidInput();

        var createdEmployee = await database.Service.CreateEmployeeAsync(input);
        var persistedEmployee = await database.Service.GetEmployeeAsync(createdEmployee.Id);

        Assert.Multiple(() =>
        {
            Assert.That(createdEmployee.Id, Is.GreaterThan(0));
            Assert.That(persistedEmployee, Is.Not.Null);
            Assert.That(persistedEmployee?.Email, Is.EqualTo("olivia.chen@example.com"));
        });
    }

    [Test]
    public async Task CreateEmployeeAsync_RejectsNullInput()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();

        Assert.ThrowsAsync<ArgumentNullException>(() => database.Service.CreateEmployeeAsync(null!));
    }

    [Test]
    public async Task CreateEmployeeAsync_RejectsInvalidInput()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var input = ValidInput();
        input.Email = "invalid";

        Assert.ThrowsAsync<ValidationException>(() => database.Service.CreateEmployeeAsync(input));
    }

    [Test]
    public async Task CreateEmployeeAsync_RejectsDuplicateEmail()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        await database.Service.CreateEmployeeAsync(ValidInput(email: "olivia.chen@example.com"));

        var duplicate = ValidInput(email: "  OLIVIA.CHEN@example.com  ");

        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => database.Service.CreateEmployeeAsync(duplicate));
        Assert.That(exception?.Message, Does.Contain("olivia.chen@example.com").IgnoreCase);
    }

    [Test]
    public async Task UpdateEmployeeAsync_UpdatesExistingEmployee()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var employee = await database.Service.CreateEmployeeAsync(ValidInput());
        var update = ValidInput(email: "olivia.chen.updated@example.com");
        update.FirstName = "Liv";
        update.Department = "Platform";
        update.Salary = 132000m;

        var updatedEmployee = await database.Service.UpdateEmployeeAsync(employee.Id, update);

        Assert.Multiple(() =>
        {
            Assert.That(updatedEmployee, Is.Not.Null);
            Assert.That(updatedEmployee?.FirstName, Is.EqualTo("Liv"));
            Assert.That(updatedEmployee?.Email, Is.EqualTo("olivia.chen.updated@example.com"));
            Assert.That(updatedEmployee?.Department, Is.EqualTo("Platform"));
            Assert.That(updatedEmployee?.Salary, Is.EqualTo(132000m));
        });
    }

    [Test]
    public async Task UpdateEmployeeAsync_ReturnsNullForMissingEmployee()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();

        var result = await database.Service.UpdateEmployeeAsync(999, ValidInput());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateEmployeeAsync_RejectsDuplicateEmail()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var first = await database.Service.CreateEmployeeAsync(ValidInput(email: "first@example.com"));
        var second = await database.Service.CreateEmployeeAsync(ValidInput(email: "second@example.com", firstName: "Second"));
        var update = ValidInput(email: first.Email, firstName: "Updated");

        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => database.Service.UpdateEmployeeAsync(second.Id, update));

        Assert.That(exception?.Message, Does.Contain(first.Email));
    }

    [Test]
    public async Task UpdateEmployeeAsync_AllowsCurrentEmployeeEmail()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var employee = await database.Service.CreateEmployeeAsync(ValidInput(email: "same@example.com"));
        var update = ValidInput(email: "same@example.com", firstName: "Same");

        var updatedEmployee = await database.Service.UpdateEmployeeAsync(employee.Id, update);

        Assert.That(updatedEmployee?.FirstName, Is.EqualTo("Same"));
    }

    [Test]
    public async Task UpdateEmployeeAsync_RejectsInvalidInput()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var employee = await database.Service.CreateEmployeeAsync(ValidInput());
        var update = ValidInput();
        update.Salary = -1m;

        Assert.ThrowsAsync<ValidationException>(() => database.Service.UpdateEmployeeAsync(employee.Id, update));
    }

    [Test]
    public async Task DeleteEmployeeAsync_RemovesExistingEmployee()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var employee = await database.Service.CreateEmployeeAsync(ValidInput());

        var deleted = await database.Service.DeleteEmployeeAsync(employee.Id);
        var missingEmployee = await database.Service.GetEmployeeAsync(employee.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.True);
            Assert.That(missingEmployee, Is.Null);
        });
    }

    [Test]
    public async Task DeleteEmployeeAsync_ReturnsFalseForMissingEmployee()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();

        var deleted = await database.Service.DeleteEmployeeAsync(999);

        Assert.That(deleted, Is.False);
    }

    private static EmployeeInput ValidInput(string email = "olivia.chen@example.com", string firstName = "Olivia")
    {
        return new EmployeeInput
        {
            FirstName = firstName,
            LastName = "Chen",
            Email = email,
            JobTitle = "Principal Engineer",
            Department = "Engineering",
            HireDate = new DateTime(2024, 1, 10),
            Salary = 128000m
        };
    }
}
