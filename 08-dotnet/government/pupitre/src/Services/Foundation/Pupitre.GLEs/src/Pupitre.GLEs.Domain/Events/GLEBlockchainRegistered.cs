using Mamey.CQRS;

namespace Pupitre.GLEs.Domain.Events;

internal record GLEBlockchainRegistered(Guid GLEId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
