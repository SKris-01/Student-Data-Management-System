-- Simple script to create Admin table in StudentManagementDB
-- Run this in SQL Server Management Studio

USE StudentManagementDB;
GO

-- Drop table if it exists
IF OBJECT_ID('dbo.tblAdmin', 'U') IS NOT NULL
    DROP TABLE dbo.tblAdmin;
GO

-- Create simple Admin table
CREATE TABLE dbo.tblAdmin (
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(10) NOT NULL DEFAULT 'Admin',
    PhoneNumber NVARCHAR(15) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Create index for faster username lookups
CREATE INDEX IX_tblAdmin_Username ON dbo.tblAdmin(Username);
GO

PRINT 'Admin table created successfully!';

-- Verify table was created
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'tblAdmin'
ORDER BY ORDINAL_POSITION;
GO
