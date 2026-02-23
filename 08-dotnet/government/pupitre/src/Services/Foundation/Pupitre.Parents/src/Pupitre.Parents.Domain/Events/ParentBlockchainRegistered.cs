using Mamey.CQRS;

namespace Pupitre.Parents.Domain.Events;

internal record ParentBlockchainRegistered(Guid ParentId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
