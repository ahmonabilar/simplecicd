using EmployeeCrud.Application.Employees;

namespace EmployeeCrud.Tests;

public sealed class EmployeeAccessorTests
{
    [Test]
    public async Task EmailExistsAsync_HandlesMatchesExclusionsAndMisses()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();
        var employee = new Employee("Ava", "Stone", "ava.stone@example.com", "Engineer", "Engineering", DateTime.Today, 90000m);
        await database.Accessor.AddAsync(employee);

        var matchingEmailExists = await database.Accessor.EmailExistsAsync("AVA.STONE@example.com");
        var excludedEmailExists = await database.Accessor.EmailExistsAsync("ava.stone@example.com", employee.Id);
        var missingEmailExists = await database.Accessor.EmailExistsAsync("missing@example.com");

        Assert.Multiple(() =>
        {
            Assert.That(matchingEmailExists, Is.True);
            Assert.That(excludedEmailExists, Is.False);
            Assert.That(missingEmailExists, Is.False);
        });
    }

    [Test]
    public async Task AccessorMethods_RejectNullArguments()
    {
        await using var database = await TestEmployeeDatabase.CreateAsync();

        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => database.Accessor.AddAsync(null!));
            Assert.ThrowsAsync<ArgumentNullException>(() => database.Accessor.DeleteAsync(null!));
            Assert.ThrowsAsync<ArgumentNullException>(() => database.Accessor.AddRangeAsync(null!));
        });
    }
}
