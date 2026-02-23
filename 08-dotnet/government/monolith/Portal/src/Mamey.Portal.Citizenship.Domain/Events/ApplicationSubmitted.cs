using Mamey.CQRS;

namespace Mamey.Portal.Citizenship.Domain.Events;

public sealed record ApplicationSubmitted(
    Guid ApplicationId,
    string ApplicationNumber,
    DateTimeOffset SubmittedAt) : IDomainEvent;
