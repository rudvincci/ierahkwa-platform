using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Parent : TenantEntity
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public string? Relation { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
