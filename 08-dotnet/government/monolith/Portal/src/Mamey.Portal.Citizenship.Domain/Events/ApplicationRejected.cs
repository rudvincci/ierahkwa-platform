using Mamey.CQRS;

namespace Mamey.Portal.Citizenship.Domain.Events;

public sealed record ApplicationRejected(
    Guid ApplicationId,
    string ApplicationNumber,
    string Reason,
    DateTimeOffset RejectedAt) : IDomainEvent;
