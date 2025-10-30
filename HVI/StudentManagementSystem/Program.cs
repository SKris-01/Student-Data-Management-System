using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container with connection fallback
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Try different connection strings if the default fails
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("LocalDbConnection") ??
                          builder.Configuration.GetConnectionString("SqlServerExpressConnection") ??
                          "Server=localhost;Database=StudentManagementDB;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true";
    }

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    });

    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Relaxed password settings for development
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false; // Allow non-unique emails for testing
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Register the admin authorization filter
builder.Services.AddScoped<AdminAuthorizationFilter>();

var app = builder.Build();

// Ensure default admin user exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        Console.WriteLine("🔄 Initializing database...");

        // Test database connection first
        if (await context.Database.CanConnectAsync())
        {
            Console.WriteLine("✅ Database connection successful!");

            // Apply migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"🔄 Applying {pendingMigrations.Count()} pending migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("✅ Database migrations applied successfully!");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date!");
            }
        }
        else
        {
            Console.WriteLine("⚠️ Cannot connect to database, attempting to create...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✅ Database created successfully!");
        }

        // Create default admin user if it doesn't exist
        var adminUser = userManager.FindByNameAsync("admin").Result;
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@studentms.com",
                Name = "System Administrator",
                Role = "Admin"
            };

            var result = userManager.CreateAsync(adminUser, "Admin123!").Result;
            if (result.Succeeded)
            {
                Console.WriteLine("ASP.NET Identity admin user created successfully!");
            }
            else
            {
                Console.WriteLine("Failed to create ASP.NET Identity admin user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }

        // Also ensure admin exists in Student table for our custom authentication
        var adminStudent = context.Students.FirstOrDefault(s => s.Username == "admin");
        if (adminStudent == null)
        {
            adminStudent = new Student
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

            context.Students.Add(adminStudent);
            context.SaveChanges();

            Console.WriteLine("Student table admin user created successfully!");
            Console.WriteLine("Username: admin");
            Console.WriteLine("Password: Admin123!");
        }
        else
        {
            Console.WriteLine("Admin user already exists in Student table");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during database initialization: {ex.Message}");

        if (ex.InnerException != null)
        {
            Console.WriteLine($"❌ Inner exception: {ex.InnerException.Message}");
        }

        Console.WriteLine("🔧 Troubleshooting tips:");
        Console.WriteLine("1. Ensure SQL Server is running");
        Console.WriteLine("2. Check connection string in appsettings.json");
        Console.WriteLine("3. Verify database permissions");
        Console.WriteLine("4. Try running as Administrator");

        // Don't crash the application, continue with limited functionality
        Console.WriteLine("⚠️ Application will continue with limited database functionality");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
