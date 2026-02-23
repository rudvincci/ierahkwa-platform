namespace SpikeOffice.Core.Entities;

/// <summary>
/// Admin/Office users (not employees). RBAC.
/// </summary>
public class SystemUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
