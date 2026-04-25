using EmployeeCrud.Application.Employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Application.Data;

public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(employee => employee.Id);

            entity.Property(employee => employee.FirstName)
                .HasMaxLength(Employee.FirstNameMaxLength)
                .IsRequired();

            entity.Property(employee => employee.LastName)
                .HasMaxLength(Employee.LastNameMaxLength)
                .IsRequired();

            entity.Property(employee => employee.Email)
                .HasMaxLength(Employee.EmailMaxLength)
                .IsRequired();

            entity.HasIndex(employee => employee.Email)
                .IsUnique();

            entity.Property(employee => employee.JobTitle)
                .HasMaxLength(Employee.JobTitleMaxLength)
                .IsRequired();

            entity.Property(employee => employee.Department)
                .HasMaxLength(Employee.DepartmentMaxLength)
                .IsRequired();

            entity.Property(employee => employee.HireDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(employee => employee.Salary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}
