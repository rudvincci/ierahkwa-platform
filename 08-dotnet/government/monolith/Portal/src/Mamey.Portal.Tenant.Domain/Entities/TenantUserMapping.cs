using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class TenantUserMapping
{
    public string Issuer { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public TenantId TenantId { get; private set; }
    public string? Email { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TenantUserMapping() { }

    public TenantUserMapping(
        string issuer,
        string subject,
        TenantId tenantId,
        string? email,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Issuer = issuer;
        Subject = subject;
        TenantId = tenantId;
        Email = email;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
