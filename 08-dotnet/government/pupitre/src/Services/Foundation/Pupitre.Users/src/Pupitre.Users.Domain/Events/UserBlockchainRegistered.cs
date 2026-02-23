using Mamey.CQRS;

namespace Pupitre.Users.Domain.Events;

internal record UserBlockchainRegistered(Guid UserId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
