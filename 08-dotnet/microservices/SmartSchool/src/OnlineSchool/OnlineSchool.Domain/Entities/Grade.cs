using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Grade : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
}
