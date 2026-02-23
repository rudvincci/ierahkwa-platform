using Mamey.CQRS.Events;

namespace Pupitre.Parents.Application.Events;

internal record ParentCredentialIssued(Guid ParentId, string IdentityId, string? LedgerTransactionId) : IEvent;
