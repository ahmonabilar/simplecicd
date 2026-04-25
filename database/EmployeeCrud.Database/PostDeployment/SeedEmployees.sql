SET IDENTITY_INSERT [dbo].[Employees] ON;

MERGE [dbo].[Employees] AS [Target]
USING
(
    VALUES
        (1, N'Ava', N'Patel', N'ava.patel@example.com', N'Software Engineer', N'Engineering', CAST('2021-02-15' AS DATE), CAST(92000.00 AS DECIMAL(18,2))),
        (2, N'Noah', N'Kim', N'noah.kim@example.com', N'Product Manager', N'Product', CAST('2020-08-03' AS DATE), CAST(108000.00 AS DECIMAL(18,2))),
        (3, N'Mia', N'Garcia', N'mia.garcia@example.com', N'UX Designer', N'Design', CAST('2022-05-09' AS DATE), CAST(87000.00 AS DECIMAL(18,2))),
        (4, N'Liam', N'Johnson', N'liam.johnson@example.com', N'QA Analyst', N'Quality Assurance', CAST('2019-11-18' AS DATE), CAST(76000.00 AS DECIMAL(18,2))),
        (5, N'Sophia', N'Brown', N'sophia.brown@example.com', N'HR Generalist', N'People', CAST('2023-01-23' AS DATE), CAST(68000.00 AS DECIMAL(18,2))),
        (6, N'Ethan', N'Davis', N'ethan.davis@example.com', N'DevOps Engineer', N'Platform', CAST('2018-07-30' AS DATE), CAST(115000.00 AS DECIMAL(18,2))),
        (7, N'Isabella', N'Miller', N'isabella.miller@example.com', N'Data Analyst', N'Analytics', CAST('2022-09-12' AS DATE), CAST(83000.00 AS DECIMAL(18,2))),
        (8, N'Lucas', N'Wilson', N'lucas.wilson@example.com', N'Account Executive', N'Sales', CAST('2021-04-05' AS DATE), CAST(79000.00 AS DECIMAL(18,2))),
        (9, N'Amelia', N'Moore', N'amelia.moore@example.com', N'Finance Specialist', N'Finance', CAST('2020-12-01' AS DATE), CAST(81000.00 AS DECIMAL(18,2))),
        (10, N'Mason', N'Taylor', N'mason.taylor@example.com', N'Customer Success Lead', N'Customer Success', CAST('2019-03-25' AS DATE), CAST(88000.00 AS DECIMAL(18,2)))
) AS [Source] ([Id], [FirstName], [LastName], [Email], [JobTitle], [Department], [HireDate], [Salary])
ON [Target].[Email] = [Source].[Email]
WHEN MATCHED THEN
    UPDATE SET
        [FirstName] = [Source].[FirstName],
        [LastName] = [Source].[LastName],
        [JobTitle] = [Source].[JobTitle],
        [Department] = [Source].[Department],
        [HireDate] = [Source].[HireDate],
        [Salary] = [Source].[Salary]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [FirstName], [LastName], [Email], [JobTitle], [Department], [HireDate], [Salary])
    VALUES ([Source].[Id], [Source].[FirstName], [Source].[LastName], [Source].[Email], [Source].[JobTitle], [Source].[Department], [Source].[HireDate], [Source].[Salary]);

SET IDENTITY_INSERT [dbo].[Employees] OFF;
