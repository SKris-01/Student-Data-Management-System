using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers;

public class TestController : Controller
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Test endpoint to verify database connectivity and data
    public async Task<IActionResult> DatabaseStatus()
    {
        try
        {
            var departments = await _context.Departments.ToListAsync();
            var students = await _context.Students.Include(s => s.Department).ToListAsync();
            var users = await _context.Users.ToListAsync();

            var status = new
            {
                DatabaseConnected = true,
                DepartmentCount = departments.Count,
                StudentCount = students.Count,
                UserCount = users.Count,
                Departments = departments.Select(d => new { d.DepartmentId, d.DepartmentName }),
                Students = students.Select(s => new 
                { 
                    s.Id, 
                    s.Name, 
                    s.Username, 
                    s.Role, 
                    s.Course, 
                    s.Semester, 
                    s.CGPA, 
                    s.Hometown,
                    Department = s.Department.DepartmentName 
                }),
                Users = users.Select(u => new { u.Id, u.Name, u.UserName, u.Role, u.Email })
            };

            return Json(status);
        }
        catch (Exception ex)
        {
            return Json(new { DatabaseConnected = false, Error = ex.Message });
        }
    }

    // Test endpoint to add sample student data
    public async Task<IActionResult> AddSampleData()
    {
        try
        {
            // Check if sample data already exists
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Username == "john.doe");
            if (existingStudent != null)
            {
                return Json(new { Success = false, Message = "Sample data already exists" });
            }

            // Add sample students
            var sampleStudents = new List<Student>
            {
                new Student
                {
                    Name = "John Doe",
                    Username = "john.doe",
                    Role = "Student",
                    Course = "Bachelor of Computer Science",
                    Semester = 6,
                    CGPA = 3.75m,
                    DOB = new DateTime(2001, 5, 15),
                    Hometown = "New York",
                    Password = "password123", // In real app, this should be hashed
                    DepartmentId = 1 // Computer Science
                },
                new Student
                {
                    Name = "Jane Smith",
                    Username = "jane.smith",
                    Role = "Student",
                    Course = "Bachelor of Information Technology",
                    Semester = 4,
                    CGPA = 3.90m,
                    DOB = new DateTime(2002, 8, 22),
                    Hometown = "California",
                    Password = "password123",
                    DepartmentId = 2 // Information Technology
                },
                new Student
                {
                    Name = "Admin User",
                    Username = "admin",
                    Role = "Admin",
                    Course = "Administrative",
                    Semester = 1,
                    CGPA = 4.00m,
                    DOB = new DateTime(1990, 1, 1),
                    Hometown = "Admin City",
                    Password = "admin123",
                    DepartmentId = 1 // Computer Science
                }
            };

            _context.Students.AddRange(sampleStudents);
            await _context.SaveChangesAsync();

            return Json(new { Success = true, Message = "Sample data added successfully", StudentsAdded = sampleStudents.Count });
        }
        catch (Exception ex)
        {
            return Json(new { Success = false, Error = ex.Message });
        }
    }

    // Test endpoint to update student data
    public async Task<IActionResult> UpdateStudentData(int studentId, decimal newCGPA, int newSemester)
    {
        try
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return Json(new { Success = false, Message = "Student not found" });
            }

            var oldCGPA = student.CGPA;
            var oldSemester = student.Semester;

            student.CGPA = newCGPA;
            student.Semester = newSemester;

            await _context.SaveChangesAsync();

            return Json(new 
            { 
                Success = true, 
                Message = "Student data updated successfully",
                StudentId = studentId,
                OldCGPA = oldCGPA,
                NewCGPA = newCGPA,
                OldSemester = oldSemester,
                NewSemester = newSemester
            });
        }
        catch (Exception ex)
        {
            return Json(new { Success = false, Error = ex.Message });
        }
    }

    // Test endpoint to verify data persistence
    public async Task<IActionResult> VerifyDataPersistence(int studentId)
    {
        try
        {
            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return Json(new { Success = false, Message = "Student not found" });
            }

            return Json(new 
            { 
                Success = true,
                Student = new
                {
                    student.Id,
                    student.Name,
                    student.Username,
                    student.Role,
                    student.Course,
                    student.Semester,
                    student.CGPA,
                    student.Hometown,
                    Department = student.Department.DepartmentName,
                    LastUpdated = DateTime.Now
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { Success = false, Error = ex.Message });
        }
    }
}
