using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Student : TenantEntity
{
    public int UserId { get; set; }
    public string? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? ProfileImage { get; set; }
    public int? ClassRoomId { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    public virtual ICollection<HomeworkAnswer> HomeworkAnswers { get; set; } = new List<HomeworkAnswer>();
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
