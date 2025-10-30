-- Add Administration Department to existing StudentManagementDB database
USE StudentManagementDB;
GO

-- Check if Administration department already exists
IF NOT EXISTS (SELECT 1 FROM tblDepartment WHERE DepartmentName = 'Administration')
BEGIN
    -- Insert Administration department
    INSERT INTO tblDepartment (DepartmentName)
    VALUES ('Administration');
    
    PRINT 'Administration department added successfully!';
END
ELSE
BEGIN
    PRINT 'Administration department already exists.';
END
GO

-- Verify the department was added
SELECT DepartmentId, DepartmentName 
FROM tblDepartment 
ORDER BY DepartmentId;
GO
