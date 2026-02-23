using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class TenantUserInvite
{
    public string Issuer { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public TenantId TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }

    private TenantUserInvite() { }

    public TenantUserInvite(
        string issuer,
        string email,
        TenantId tenantId,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        DateTimeOffset? usedAt)
    {
        Issuer = issuer;
        Email = email;
        TenantId = tenantId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UsedAt = usedAt;
    }
}
