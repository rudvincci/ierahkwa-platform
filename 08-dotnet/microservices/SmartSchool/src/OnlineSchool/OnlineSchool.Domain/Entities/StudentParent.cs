using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class StudentParent : TenantEntity
{
    public int StudentId { get; set; }
    public int ParentId { get; set; }
    
    public virtual Student? Student { get; set; }
    public virtual Parent? Parent { get; set; }
}
