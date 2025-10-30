using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers;

public class DatabaseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> CreateAdminTable()
    {
        try
        {
            // First check if table exists
            var checkSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblAdmin'";
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = checkSql;
            var tableExists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;

            if (tableExists)
            {
                await connection.CloseAsync();
                return Json(new { success = true, message = "Admin table already exists!" });
            }

            // Create the simple table
            var createTableSql = @"
                CREATE TABLE dbo.tblAdmin (
                    AdminId INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Username NVARCHAR(50) NOT NULL UNIQUE,
                    Password NVARCHAR(MAX) NOT NULL,
                    Role NVARCHAR(10) NOT NULL DEFAULT 'Admin',
                    PhoneNumber NVARCHAR(15) NULL,
                    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
                );

                CREATE INDEX IX_tblAdmin_Username ON dbo.tblAdmin(Username);
            ";

            using var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            await command.ExecuteNonQueryAsync();

            await connection.CloseAsync();

            return Json(new { success = true, message = "Admin table created successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error creating Admin table: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckAdminTable()
    {
        try
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblAdmin'";
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var result = await command.ExecuteScalarAsync();
            
            await connection.CloseAsync();
            
            var tableExists = Convert.ToInt32(result) > 0;
            return Json(new { exists = tableExists, message = tableExists ? "Admin table exists" : "Admin table does not exist" });
        }
        catch (Exception ex)
        {
            return Json(new { exists = false, message = $"Error checking Admin table: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateTestAdmin()
    {
        try
        {
            // Check if test admin already exists
            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == "admin");
            var existingUser = await _userManager.FindByNameAsync("admin");

            if (existingAdmin != null || existingUser != null)
            {
                return Json(new { success = true, message = "Test admin already exists! Username: admin, Password: Admin123!" });
            }

            // Create ApplicationUser for authentication
            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@system.com",
                Name = "System Administrator",
                Role = "Admin"
            };

            var result = await _userManager.CreateAsync(user, "Admin123!");

            if (result.Succeeded)
            {
                // Create Admin record in the database
                var admin = new Admin
                {
                    Name = "System Administrator",
                    Username = "admin",
                    Role = "Admin",
                    PhoneNumber = "1234567890",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!")
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return Json(new {
                    success = true,
                    message = "Test admin created successfully! Username: admin, Password: Admin123!",
                    username = "admin",
                    password = "Admin123!"
                });
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Failed to create admin user: {errors}" });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error creating test admin: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddAdministrationDepartment()
    {
        try
        {
            // Check if Administration department already exists
            var existingDept = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName == "Administration");

            if (existingDept != null)
            {
                return Json(new { success = true, message = "Administration department already exists!" });
            }

            // Add Administration department
            var adminDept = new Department
            {
                DepartmentName = "Administration"
            };

            _context.Departments.Add(adminDept);
            await _context.SaveChangesAsync();

            return Json(new {
                success = true,
                message = "Administration department added successfully!",
                departmentId = adminDept.DepartmentId
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error adding Administration department: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListDepartments()
    {
        try
        {
            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentId)
                .Select(d => new { d.DepartmentId, d.DepartmentName })
                .ToListAsync();

            return Json(new {
                success = true,
                message = "Departments retrieved successfully",
                departments = departments
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error retrieving departments: {ex.Message}" });
        }
    }
}
