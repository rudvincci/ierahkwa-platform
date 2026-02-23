using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Material : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<TeacherMaterial> TeacherMaterials { get; set; } = new List<TeacherMaterial>();
    public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}
