using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Entities;

/// <summary>
/// Tenant invitation for onboarding new users to a tenant.
/// </summary>
internal class TenantInvite
{
    private TenantInvite() { }

    public TenantInvite(
        Guid id,
        TenantId tenantId,
        string email,
        string? role = null,
        DateTime? expiresAt = null)
    {
        Id = id;
        TenantId = tenantId;
        Email = email;
        Role = role;
        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7); // Default 7 days
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;
    }

    public Guid Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Role { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
