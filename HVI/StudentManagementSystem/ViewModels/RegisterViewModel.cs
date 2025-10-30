using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels;

public class RegisterViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Course")]
    public string Course { get; set; } = string.Empty;

    [Required]
    [Range(1, 8)]
    [Display(Name = "Semester")]
    public int Semester { get; set; }

    [Required]
    [Range(0.0, 4.0)]
    [Display(Name = "CGPA")]
    public decimal CGPA { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DOB { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Hometown")]
    public string Hometown { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }
}

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
