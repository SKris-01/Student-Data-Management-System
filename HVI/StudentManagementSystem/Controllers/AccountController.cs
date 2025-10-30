using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Departments = _context.Departments.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create ApplicationUser for authentication
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                Role = "Student"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create Student record for the database
                var student = new Student
                {
                    Name = model.Name,
                    Username = model.Username,
                    Role = "Student", // Fixed role for student registration
                    Course = model.Course,
                    Semester = model.Semester,
                    CGPA = model.CGPA,
                    DOB = model.DOB,
                    Hometown = model.Hometown,
                    PhoneNumber = model.PhoneNumber,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    DepartmentId = model.DepartmentId
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Details", "Student");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ViewBag.Departments = _context.Departments.ToList();
        return View(model);
    }



    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // First try ASP.NET Identity authentication (faster for existing users)
            var result = await _signInManager.PasswordSignInAsync(
                model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user?.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Student");
                }
                return RedirectToAction("Details", "Student");
            }

            // If Identity authentication fails, try Admin table first, then Student table
            // Check Admin table
            var admin = await _context.Admins
                .AsNoTracking()
                .Select(a => new { a.Username, a.Password, a.Role, a.Name })
                .FirstOrDefaultAsync(a => a.Username == model.Username);

            if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            {
                // Check if Identity user exists
                var identityUser = await _userManager.FindByNameAsync(model.Username);
                if (identityUser == null)
                {
                    // Create ASP.NET Identity user for admin
                    identityUser = new ApplicationUser
                    {
                        UserName = admin.Username,
                        Email = $"{admin.Username}@admin.com",
                        Name = admin.Name,
                        Role = admin.Role
                    };

                    var createResult = await _userManager.CreateAsync(identityUser, model.Password);
                    if (!createResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user account.");
                        return View(model);
                    }
                }

                // Sign in the admin
                await _signInManager.SignInAsync(identityUser, model.RememberMe);
                return RedirectToAction("AdminDashboard", "Student");
            }

            // Check Student table (for legacy users)
            var student = await _context.Students
                .AsNoTracking()
                .Select(s => new { s.Username, s.Password, s.Role, s.Name })
                .FirstOrDefaultAsync(s => s.Username == model.Username);

            if (student != null && BCrypt.Net.BCrypt.Verify(model.Password, student.Password))
            {
                // Check if Identity user exists
                var identityUser = await _userManager.FindByNameAsync(model.Username);
                if (identityUser == null)
                {
                    // Create ASP.NET Identity user for legacy student
                    identityUser = new ApplicationUser
                    {
                        UserName = student.Username,
                        Email = $"{student.Username}@studentms.com",
                        Name = student.Name,
                        Role = student.Role
                    };

                    var createResult = await _userManager.CreateAsync(identityUser, model.Password);
                    if (!createResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user account.");
                        return View(model);
                    }
                }

                // Sign in the user
                await _signInManager.SignInAsync(identityUser, model.RememberMe);

                // Redirect based on role
                if (student.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Student");
                }
                else
                {
                    return RedirectToAction("Details", "Student");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
        }

        return View(model);
    }

    // Simple login bypass for testing - optimized for performance
    [HttpGet]
    public async Task<IActionResult> SimpleLogin(string username = "admin", string password = "Admin123!")
    {
        // First try Identity authentication (faster)
        var identityUser = await _userManager.FindByNameAsync(username);
        if (identityUser != null)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (signInResult.Succeeded)
            {
                if (identityUser.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Student");
                }
                else
                {
                    return RedirectToAction("Details", "Student");
                }
            }
        }

        // Fallback to Student table for legacy users
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Username == username);

        if (student != null && BCrypt.Net.BCrypt.Verify(password, student.Password))
        {
            // Create Identity user if it doesn't exist
            if (identityUser == null)
            {
                identityUser = new ApplicationUser
                {
                    UserName = student.Username,
                    Email = $"{student.Username}@studentms.com",
                    Name = student.Name,
                    Role = student.Role
                };

                var createResult = await _userManager.CreateAsync(identityUser, password);
                if (!createResult.Succeeded)
                {
                    return Json(new { success = false, message = "Error creating user account" });
                }
            }

            await _signInManager.SignInAsync(identityUser, false);

            if (student.Role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Student");
            }
            else
            {
                return RedirectToAction("Details", "Student");
            }
        }

        return Json(new { success = false, message = "Invalid credentials" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
