using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class TenantDocumentNaming
{
    public TenantId TenantId { get; private set; }
    public string PatternJson { get; private set; } = "{}";
    public DateTimeOffset UpdatedAt { get; private set; }

    private TenantDocumentNaming() { }

    public TenantDocumentNaming(TenantId tenantId, string patternJson, DateTimeOffset updatedAt)
    {
        TenantId = tenantId;
        PatternJson = patternJson;
        UpdatedAt = updatedAt;
    }
}
