CREATE TABLE [dbo].[Managers]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [phone] NVARCHAR(50) NULL, 
    [jurisdiction] NCHAR(10) NULL, 
    [identificationNumber] UNIQUEIDENTIFIER NULL, 
    [firstName] NVARCHAR(50) NULL, 
    [lastName] NVARCHAR(200) NULL
)