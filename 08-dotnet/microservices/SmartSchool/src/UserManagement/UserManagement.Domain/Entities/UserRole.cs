using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    
    public virtual ApplicationUser? User { get; set; }
    public virtual Role? Role { get; set; }
}
