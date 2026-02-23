using Mamey.CQRS;

namespace Mamey.Portal.Tenant.Domain.Events;

public sealed record TenantSettingsUpdated(
    string TenantId,
    DateTimeOffset UpdatedAt) : IDomainEvent;
