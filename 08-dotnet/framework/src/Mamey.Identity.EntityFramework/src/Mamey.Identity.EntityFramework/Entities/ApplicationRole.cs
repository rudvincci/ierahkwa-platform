using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mamey.Identity.EntityFramework.Entities;

public class ApplicationRole : IdentityRole
{
    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = new List<ApplicationRoleClaim>();
}
