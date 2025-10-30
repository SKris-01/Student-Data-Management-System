-- Add Admin Table to existing StudentManagementDB database
USE StudentManagementDB;
GO

-- Check if Admin table already exists and drop it if it does
IF OBJECT_ID('dbo.tblAdmin', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.tblAdmin;
    PRINT 'Existing tblAdmin table dropped.';
END
GO

-- Create the Admin table
CREATE TABLE dbo.tblAdmin (
    AdminId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(10) NOT NULL DEFAULT 'Admin',
    PhoneNumber NVARCHAR(15) NULL,
    OriginalStudentId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_tblAdmin PRIMARY KEY (AdminId),
    CONSTRAINT FK_tblAdmin_tblStudent_OriginalStudentId 
        FOREIGN KEY (OriginalStudentId) 
        REFERENCES dbo.tblStudent(Id) 
        ON DELETE SET NULL
);
GO

-- Create indexes for performance
CREATE UNIQUE INDEX IX_tblAdmin_Username ON dbo.tblAdmin(Username);
CREATE INDEX IX_tblAdmin_Role ON dbo.tblAdmin(Role);
CREATE INDEX IX_tblAdmin_OriginalStudentId ON dbo.tblAdmin(OriginalStudentId);
GO

-- Create trigger to automatically update UpdatedAt timestamp
CREATE TRIGGER tr_tblAdmin_UpdatedAt
ON dbo.tblAdmin
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblAdmin 
    SET UpdatedAt = GETDATE()
    FROM dbo.tblAdmin a
    INNER JOIN inserted i ON a.AdminId = i.AdminId;
END;
GO

PRINT 'tblAdmin table created successfully with indexes and triggers.';

-- Verify the table was created
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'tblAdmin'
ORDER BY ORDINAL_POSITION;
GO
