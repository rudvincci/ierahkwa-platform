using Mamey.CQRS;

namespace Pupitre.Rewards.Domain.Events;

internal record RewardBlockchainRegistered(Guid RewardId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
