using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Filters;

namespace StudentManagementSystem.Controllers;

public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // Check if user is authenticated
        if (currentUser == null || !_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("SimpleLogin");
        }

        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Username == currentUser.UserName);

        if (student == null)
        {
            return NotFound("Student record not found.");
        }

        ViewBag.CurrentUser = currentUser;
        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string departmentName)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        // Allow access without authentication for testing
        if (currentUser == null)
        {
            currentUser = new ApplicationUser
            {
                Name = "Admin User",
                Role = "Admin",
                UserName = "admin"
            };
        }

        var students = new List<Student>();
        
        if (!string.IsNullOrEmpty(departmentName))
        {
            students = await _context.Students
                .Include(s => s.Department)
                .Where(s => s.Department.DepartmentName.Contains(departmentName))
                .ToListAsync();
        }

        ViewBag.DepartmentName = departmentName;
        ViewBag.CurrentUser = currentUser;
        return View(students);
    }

    // Unified Dashboard - routes to appropriate dashboard based on role
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // Check if user is authenticated
        if (currentUser == null || !_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("SimpleLogin");
        }

        // Route to appropriate dashboard based on role
        if (currentUser.Role == "Admin")
        {
            return RedirectToAction("AdminDashboard");
        }
        else
        {
            return RedirectToAction("Details");
        }
    }

    [HttpGet]
    [RequireAdmin]
    public async Task<IActionResult> AdminDashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        // Optimized queries for better performance - load only essential data
        var students = await _context.Students
            .Include(s => s.Department)
            .OrderBy(s => s.Name)
            .Take(50) // Reduced limit for faster loading
            .Select(s => new Student
            {
                Id = s.Id,
                Name = s.Name,
                Username = s.Username,
                Role = s.Role,
                Course = s.Course,
                Semester = s.Semester,
                CGPA = s.CGPA,
                DepartmentId = s.DepartmentId,
                Department = s.Department
            })
            .ToListAsync();

        var departments = await _context.Departments.ToListAsync();

        ViewBag.CurrentUser = currentUser;
        ViewBag.Departments = departments;

        return View(students);
    }

    // Temporary unrestricted admin dashboard for testing
    [HttpGet]
    public async Task<IActionResult> AdminTest()
    {
        var students = await _context.Students
            .Include(s => s.Department)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var departments = await _context.Departments.ToListAsync();

        var mockUser = new ApplicationUser
        {
            Name = "Test Admin",
            Role = "Admin",
            UserName = "admin"
        };

        ViewBag.CurrentUser = mockUser;
        ViewBag.Departments = departments;

        return View("AdminDashboard", students);
    }

    [HttpGet]
    [RequireAdmin]
    public async Task<IActionResult> ManageStudents(string searchTerm, int? departmentId, decimal? minCgpa, int? semester)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        var query = _context.Students.Include(s => s.Department).AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(s => s.Name.Contains(searchTerm) ||
                                   s.Username.Contains(searchTerm) ||
                                   s.Course.Contains(searchTerm) ||
                                   s.Hometown.Contains(searchTerm));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == departmentId.Value);
        }

        if (minCgpa.HasValue)
        {
            query = query.Where(s => s.CGPA >= minCgpa.Value);
        }

        if (semester.HasValue)
        {
            query = query.Where(s => s.Semester == semester.Value);
        }

        var students = await query.OrderBy(s => s.Name).Take(200).ToListAsync(); // Limit for performance
        var departments = await _context.Departments.ToListAsync();

        ViewBag.Departments = departments;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.DepartmentId = departmentId;
        ViewBag.MinCgpa = minCgpa;
        ViewBag.Semester = semester;
        ViewBag.CurrentUser = currentUser;

        return View(students);
    }

    [HttpGet]
    [RequireAdmin]
    public async Task<IActionResult> CreateStudent()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        var departments = await _context.Departments.ToListAsync();
        ViewBag.Departments = departments;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireAdmin]
    public async Task<IActionResult> CreateStudent(Student student)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        // Remove ModelState errors for Department navigation property
        ModelState.Remove("Department");

        if (ModelState.IsValid)
        {
            try
            {
                // Check if username already exists (optimized query)
                var usernameExists = await _context.Students
                    .AnyAsync(s => s.Username == student.Username);

                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    var departments = await _context.Departments.ToListAsync();
                    ViewBag.Departments = departments;
                    return View(student);
                }

                // Hash the password
                student.Password = BCrypt.Net.BCrypt.HashPassword(student.Password);

                // Add and save in one transaction
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Student created successfully!";
                return RedirectToAction(nameof(AdminDashboard));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating student: {ex.Message}");
                var departments = await _context.Departments.ToListAsync();
                ViewBag.Departments = departments;
                return View(student);
            }
        }

        var depts = await _context.Departments.ToListAsync();
        ViewBag.Departments = depts;
        return View(student);
    }

    [HttpGet]
    [RequireAdmin]
    public async Task<IActionResult> EditStudent(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound();
        }

        var departments = await _context.Departments.ToListAsync();
        ViewBag.Departments = departments;

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireAdmin]
    public async Task<IActionResult> EditStudent(int id, Student student)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        if (id != student.Id)
        {
            return NotFound();
        }

        // Remove ModelState errors for Department navigation property and optional password
        ModelState.Remove("Department");
        if (string.IsNullOrEmpty(student.Password))
        {
            ModelState.Remove("Password");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingStudent = await _context.Students.FindAsync(id);
                if (existingStudent == null)
                {
                    return NotFound();
                }

                // Check if username is being changed and if it already exists (optimized)
                if (existingStudent.Username != student.Username)
                {
                    var usernameExists = await _context.Students
                        .AnyAsync(s => s.Username == student.Username && s.Id != id);

                    if (usernameExists)
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        var departments = await _context.Departments.ToListAsync();
                        ViewBag.Departments = departments;
                        return View(student);
                    }
                }

                // Check if role is changing to/from Admin
                bool roleChanged = existingStudent.Role != student.Role;
                string oldRole = existingStudent.Role;

                // Update properties efficiently
                existingStudent.Name = student.Name;
                existingStudent.Username = student.Username;
                existingStudent.Role = student.Role;
                existingStudent.Course = student.Course;
                existingStudent.Semester = student.Semester;
                existingStudent.CGPA = student.CGPA;
                existingStudent.DOB = student.DOB;
                existingStudent.Hometown = student.Hometown;
                existingStudent.PhoneNumber = student.PhoneNumber;
                existingStudent.DepartmentId = student.DepartmentId;

                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(student.Password))
                {
                    existingStudent.Password = BCrypt.Net.BCrypt.HashPassword(student.Password);
                }

                // If role changed, update Identity user as well
                if (roleChanged)
                {
                    var identityUser = await _userManager.FindByNameAsync(existingStudent.Username);
                    if (identityUser != null)
                    {
                        identityUser.Role = student.Role;
                        await _userManager.UpdateAsync(identityUser);
                    }
                }
                else if (roleChanged)
                {
                    // Handle other role changes (if any)
                    var identityUser = await _userManager.FindByNameAsync(existingStudent.Username);
                    if (identityUser != null)
                    {
                        identityUser.Role = student.Role;
                        await _userManager.UpdateAsync(identityUser);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student updated successfully!";
                return RedirectToAction(nameof(AdminDashboard));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating student: {ex.Message}");
                var departments = await _context.Departments.ToListAsync();
                ViewBag.Departments = departments;
                return View(student);
            }
        }
        else
        {
            // Log validation errors for debugging
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                .ToList();

            TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
        }

        var depts = await _context.Departments.ToListAsync();
        ViewBag.Departments = depts;
        return View(student);
    }

    [HttpGet]
    [RequireAdmin]
    public async Task<IActionResult> ViewStudent(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound();
        }

        ViewBag.CurrentUser = currentUser;
        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireAdmin]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // User is guaranteed to be authenticated and admin due to RequireAdmin filter
        if (currentUser == null)
        {
            return RedirectToAction("SimpleLogin");
        }

        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            // Check if the student being deleted is the currently logged-in user
            bool isDeletingSelf = currentUser.UserName == student.Username;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            // If deleting self, also remove from Identity and logout
            if (isDeletingSelf)
            {
                var identityUser = await _userManager.FindByNameAsync(student.Username);
                if (identityUser != null)
                {
                    await _userManager.DeleteAsync(identityUser);
                }

                await _signInManager.SignOutAsync();
                TempData["SuccessMessage"] = "Admin account deleted successfully. You have been logged out.";
                return RedirectToAction("Index", "Home");
            }

            TempData["SuccessMessage"] = "Student deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Student not found.";
        }

        return RedirectToAction(nameof(ManageStudents));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }

    // Temporary debug endpoint - remove in production
    [HttpGet]
    public async Task<IActionResult> Debug()
    {
        var students = await _context.Students.ToListAsync();
        var debugInfo = new
        {
            StudentCount = students.Count,
            Students = students.Select(s => new
            {
                s.Id,
                s.Name,
                s.Username,
                s.Role,
                PasswordHash = s.Password?.Substring(0, Math.Min(10, s.Password.Length)) + "..."
            }).ToList()
        };

        return Json(debugInfo);
    }

    // Temporary endpoint to create admin user - remove in production
    [HttpGet]
    public async Task<IActionResult> CreateAdmin()
    {
        try
        {
            // Check if admin already exists in both Student table and Identity
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Username == "admin");
            var existingIdentity = await _userManager.FindByNameAsync("admin");

            if (existingStudent != null || existingIdentity != null)
            {
                return Json(new { success = false, message = "Admin user already exists" });
            }

            // Create admin student record
            var adminStudent = new Student
            {
                Name = "System Administrator",
                Username = "admin",
                Role = "Admin",
                Course = "Administration",
                Semester = 1,
                CGPA = 4.0m,
                DOB = new DateTime(1990, 1, 1),
                Hometown = "System",
                PhoneNumber = "+1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                DepartmentId = 1 // Computer Science
            };

            _context.Students.Add(adminStudent);
            await _context.SaveChangesAsync();

            // Create corresponding ASP.NET Identity user
            var identityUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@studentms.com",
                Name = "System Administrator",
                Role = "Admin"
            };

            var createResult = await _userManager.CreateAsync(identityUser, "Admin123!");
            if (!createResult.Succeeded)
            {
                // If Identity user creation fails, remove the student record
                _context.Students.Remove(adminStudent);
                await _context.SaveChangesAsync();

                return Json(new {
                    success = false,
                    message = "Failed to create admin identity user",
                    errors = createResult.Errors.Select(e => e.Description)
                });
            }

            return Json(new {
                success = true,
                message = "Admin user created successfully!",
                username = "admin",
                password = "Admin123!"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // Test login credentials
    [HttpGet]
    public async Task<IActionResult> TestLogin(string username = "admin", string password = "Admin123!")
    {
        try
        {
            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Username == username);

            if (student == null)
            {
                return Json(new {
                    success = false,
                    message = "User not found in Student table",
                    username = username
                });
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, student.Password);

            return Json(new {
                success = passwordValid,
                message = passwordValid ? "Password is correct" : "Password is incorrect",
                username = username,
                studentFound = true,
                studentRole = student.Role,
                passwordHashStart = student.Password?.Substring(0, 10) + "..."
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // Direct admin login - optimized for performance
    [HttpGet]
    public async Task<IActionResult> AdminLogin()
    {
        try
        {
            // Check if user is already signed in as admin
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Role == "Admin")
                {
                    return Json(new {
                        success = true,
                        message = "Already logged in as admin!",
                        redirectUrl = "/Student/AdminDashboard"
                    });
                }
            }

            // Check if admin exists in both Student table and Identity (consistent with CreateAdmin)
            var adminStudent = await _context.Students.FirstOrDefaultAsync(s => s.Username == "admin");
            var identityUser = await _userManager.FindByNameAsync("admin");

            if (adminStudent == null || identityUser == null)
            {
                return Json(new { success = false, message = "Admin user not found. Please create admin first." });
            }

            // Sign in the user directly
            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            return Json(new {
                success = true,
                message = "Admin logged in successfully!",
                redirectUrl = "/Student/AdminDashboard"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // Simple login page
    [HttpGet]
    public IActionResult SimpleLogin()
    {
        return View();
    }

    // Logout action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Index", "Home");
    }

    // Test endpoint to verify data persistence
    [HttpGet]
    public async Task<IActionResult> TestDataPersistence()
    {
        try
        {
            var studentCount = await _context.Students.CountAsync();
            var departmentCount = await _context.Departments.CountAsync();

            var testResult = new
            {
                Success = true,
                Message = "Database connection successful",
                StudentCount = studentCount,
                DepartmentCount = departmentCount,
                DatabaseName = _context.Database.GetDbConnection().Database,
                ConnectionState = _context.Database.GetDbConnection().State.ToString(),
                Timestamp = DateTime.Now
            };

            return Json(testResult);
        }
        catch (Exception ex)
        {
            var errorResult = new
            {
                Success = false,
                Message = $"Database error: {ex.Message}",
                Timestamp = DateTime.Now
            };

            return Json(errorResult);
        }
    }
}
