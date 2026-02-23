using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class Policy : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Area { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();
}
