using Mamey.CQRS;

namespace Mamey.Portal.Citizenship.Domain.Events;

public sealed record StatusProgressed(
    Guid CitizenshipStatusId,
    string TenantId,
    string Email,
    string Status,
    DateTimeOffset ProgressedAt) : IDomainEvent;
