CREATE TABLE [dbo].[Employees]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(80) NOT NULL,
    [LastName] NVARCHAR(80) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [JobTitle] NVARCHAR(120) NOT NULL,
    [Department] NVARCHAR(120) NOT NULL,
    [HireDate] DATE NOT NULL,
    [Salary] DECIMAL(18,2) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_Employees_Email] UNIQUE ([Email])
);
