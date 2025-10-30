using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("tblStudent");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            // Add indexes for performance
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("IX_Student_Username");

            entity.HasIndex(e => e.Role)
                .HasDatabaseName("IX_Student_Role");

            entity.HasIndex(e => e.DepartmentId)
                .HasDatabaseName("IX_Student_DepartmentId");

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Student");

            entity.Property(e => e.Course)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CGPA)
                .HasColumnType("decimal(3,2)");

            entity.Property(e => e.Hometown)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            // Configure foreign key relationship
            entity.HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("tblDepartment");
            entity.HasKey(e => e.DepartmentId);

            entity.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Configure Admin entity
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("tblAdmin");
            entity.HasKey(e => e.AdminId);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Password)
                .IsRequired();

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("Admin");

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Add index for username for faster lookups
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("IX_Admin_Username");
        });

        // Seed data
        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, DepartmentName = "Computer Science" },
            new Department { DepartmentId = 2, DepartmentName = "Information Technology" },
            new Department { DepartmentId = 3, DepartmentName = "Electronics Engineering" },
            new Department { DepartmentId = 4, DepartmentName = "Mechanical Engineering" },
            new Department { DepartmentId = 5, DepartmentName = "Civil Engineering" }
        );
    }
}
