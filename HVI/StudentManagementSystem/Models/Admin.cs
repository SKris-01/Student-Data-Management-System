using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models;

public class Admin
{
    [Key]
    public int AdminId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Role { get; set; } = "Admin";

    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
