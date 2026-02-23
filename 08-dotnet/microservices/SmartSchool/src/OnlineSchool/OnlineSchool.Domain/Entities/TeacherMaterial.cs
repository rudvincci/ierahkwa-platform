using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class TeacherMaterial : TenantEntity
{
    public int TeacherId { get; set; }
    public int MaterialId { get; set; }
    
    public virtual Teacher? Teacher { get; set; }
    public virtual Material? Material { get; set; }
}
