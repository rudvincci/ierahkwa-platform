using Mamey.CQRS;

namespace Mamey.Portal.Citizenship.Domain.Events;

public sealed record ApplicationApproved(
    Guid ApplicationId,
    string ApplicationNumber,
    DateTimeOffset ApprovedAt) : IDomainEvent;
