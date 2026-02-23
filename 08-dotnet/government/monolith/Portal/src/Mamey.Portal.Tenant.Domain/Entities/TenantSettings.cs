using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class TenantSettings
{
    public TenantId TenantId { get; private set; }
    public string BrandingJson { get; private set; } = "{}";
    public string? ActivationJson { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TenantSettings() { }

    public TenantSettings(
        TenantId tenantId,
        string brandingJson,
        string? activationJson,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        TenantId = tenantId;
        BrandingJson = brandingJson;
        ActivationJson = activationJson;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
