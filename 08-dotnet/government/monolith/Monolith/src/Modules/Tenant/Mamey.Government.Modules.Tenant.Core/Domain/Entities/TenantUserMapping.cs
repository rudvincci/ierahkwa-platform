using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Entities;

/// <summary>
/// Maps Authentik users (issuer + subject) to tenants.
/// Used for tenant resolution during authentication.
/// </summary>
internal class TenantUserMapping
{
    private TenantUserMapping() { }

    public TenantUserMapping(
        string issuer,
        string subject,
        TenantId tenantId,
        string? email = null)
    {
        Issuer = issuer;
        Subject = subject;
        TenantId = tenantId;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Issuer { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public TenantId TenantId { get; private set; }
    public string? Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateEmail(string? email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}
