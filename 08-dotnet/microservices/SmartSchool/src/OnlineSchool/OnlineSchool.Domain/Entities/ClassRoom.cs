using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class ClassRoom : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public int Capacity { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    
    public virtual Grade? Grade { get; set; }
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
