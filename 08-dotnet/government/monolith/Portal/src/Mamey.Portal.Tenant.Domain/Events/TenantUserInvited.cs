using Mamey.CQRS;

namespace Mamey.Portal.Tenant.Domain.Events;

public sealed record TenantUserInvited(
    string TenantId,
    string Email,
    DateTimeOffset InvitedAt) : IDomainEvent;
