using Mamey.CQRS;

namespace Mamey.Portal.Tenant.Domain.Events;

public sealed record TenantCreated(
    string TenantId,
    DateTimeOffset CreatedAt) : IDomainEvent;
