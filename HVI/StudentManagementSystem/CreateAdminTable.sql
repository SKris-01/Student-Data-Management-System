-- Create Admin Table in SQL Server
-- This script creates the tblAdmin table with proper structure and indexes

-- Check if table exists and drop if it does (for clean recreation)
IF OBJECT_ID('dbo.tblAdmin', 'U') IS NOT NULL
    DROP TABLE dbo.tblAdmin;

-- Create the Admin table
CREATE TABLE dbo.tblAdmin (
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(10) NOT NULL DEFAULT 'Admin',
    PhoneNumber NVARCHAR(15) NULL,
    OriginalStudentId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign key constraint to Student table
    CONSTRAINT FK_tblAdmin_tblStudent_OriginalStudentId 
        FOREIGN KEY (OriginalStudentId) 
        REFERENCES dbo.tblStudent(Id) 
        ON DELETE SET NULL
);

-- Create indexes for performance
CREATE UNIQUE INDEX IX_Admin_Username ON dbo.tblAdmin(Username);
CREATE INDEX IX_Admin_Role ON dbo.tblAdmin(Role);
CREATE INDEX IX_Admin_OriginalStudentId ON dbo.tblAdmin(OriginalStudentId);

-- Add a trigger to update UpdatedAt timestamp
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

PRINT 'Admin table created successfully with indexes and triggers.';
