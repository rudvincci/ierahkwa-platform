using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class ApplicationUser : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public int? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
