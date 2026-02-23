using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class RolePolicy : BaseEntity
{
    public int RoleId { get; set; }
    public int PolicyId { get; set; }
    
    public virtual Role? Role { get; set; }
    public virtual Policy? Policy { get; set; }
}
