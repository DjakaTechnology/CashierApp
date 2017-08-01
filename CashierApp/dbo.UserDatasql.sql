CREATE TABLE [dbo].[userData]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [name] VARCHAR(50) NOT NULL, 
    [username] VARCHAR(50) NOT NULL, 
    [password] VARCHAR(50) NOT NULL, 
    [permission] NCHAR(10) NOT NULL DEFAULT User
)
