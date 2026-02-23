using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Teacher : TenantEntity
{
    public int UserId { get; set; }
    public string? EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Qualification { get; set; }
    public string? Experience { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<TeacherMaterial> TeacherMaterials { get; set; } = new List<TeacherMaterial>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
