namespace EmployeeCrud.Application.Employees;

public static class EmployeeSeedData
{
    public static IReadOnlyList<Employee> CreateEmployees()
    {
        return
        [
            new Employee("Ava", "Patel", "ava.patel@example.com", "Software Engineer", "Engineering", new DateTime(2021, 2, 15), 92000m),
            new Employee("Noah", "Kim", "noah.kim@example.com", "Product Manager", "Product", new DateTime(2020, 8, 3), 108000m),
            new Employee("Mia", "Garcia", "mia.garcia@example.com", "UX Designer", "Design", new DateTime(2022, 5, 9), 87000m),
            new Employee("Liam", "Johnson", "liam.johnson@example.com", "QA Analyst", "Quality Assurance", new DateTime(2019, 11, 18), 76000m),
            new Employee("Sophia", "Brown", "sophia.brown@example.com", "HR Generalist", "People", new DateTime(2023, 1, 23), 68000m),
            new Employee("Ethan", "Davis", "ethan.davis@example.com", "DevOps Engineer", "Platform", new DateTime(2018, 7, 30), 115000m),
            new Employee("Isabella", "Miller", "isabella.miller@example.com", "Data Analyst", "Analytics", new DateTime(2022, 9, 12), 83000m),
            new Employee("Lucas", "Wilson", "lucas.wilson@example.com", "Account Executive", "Sales", new DateTime(2021, 4, 5), 79000m),
            new Employee("Amelia", "Moore", "amelia.moore@example.com", "Finance Specialist", "Finance", new DateTime(2020, 12, 1), 81000m),
            new Employee("Mason", "Taylor", "mason.taylor@example.com", "Customer Success Lead", "Customer Success", new DateTime(2019, 3, 25), 88000m),
        ];
    }
}
