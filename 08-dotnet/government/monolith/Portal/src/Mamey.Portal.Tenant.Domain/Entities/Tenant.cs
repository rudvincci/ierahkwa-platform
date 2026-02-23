using Mamey.Portal.Tenant.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class Tenant : AggregateRoot<string>
{
    public string DisplayName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Tenant() { }

    public Tenant(ValueObjects.TenantId tenantId, string displayName, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        : base(tenantId.Value)
    {
        DisplayName = displayName;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
