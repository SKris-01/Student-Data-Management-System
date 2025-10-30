using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace StudentManagementSystem.Models;

// Custom IdentityUser for authentication
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "Student";
}

// Student entity for the database table
public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "Student";

    [Required]
    [StringLength(100)]
    public string Course { get; set; } = string.Empty;

    [Required]
    public int Semester { get; set; }

    [Range(0.0, 4.0, ErrorMessage = "CGPA must be between 0.0 and 4.0")]
    public decimal CGPA { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

    [Required]
    [StringLength(100)]
    public string Hometown { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    // Foreign key
    [Required(ErrorMessage = "Please select a department")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    // Navigation property
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }
}
