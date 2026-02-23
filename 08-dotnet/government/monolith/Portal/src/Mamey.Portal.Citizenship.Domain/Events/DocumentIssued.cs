using Mamey.CQRS;

namespace Mamey.Portal.Citizenship.Domain.Events;

public sealed record DocumentIssued(
    Guid IssuedDocumentId,
    Guid ApplicationId,
    string DocumentKind,
    DateTimeOffset IssuedAt) : IDomainEvent;
