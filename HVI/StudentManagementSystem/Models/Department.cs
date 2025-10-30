using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models;

public class Department
{
    [Key]
    public int DepartmentId { get; set; }

    [Required]
    [StringLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
