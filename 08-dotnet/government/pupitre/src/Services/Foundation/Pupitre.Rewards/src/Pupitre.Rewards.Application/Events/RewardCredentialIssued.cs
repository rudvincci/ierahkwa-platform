using Mamey.CQRS.Events;

namespace Pupitre.Rewards.Application.Events;

internal record RewardCredentialIssued(Guid RewardId, string IdentityId, string? LedgerTransactionId) : IEvent;
