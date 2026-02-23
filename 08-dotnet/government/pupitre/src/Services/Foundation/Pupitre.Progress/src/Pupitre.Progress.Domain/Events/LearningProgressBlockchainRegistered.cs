using Mamey.CQRS;

namespace Pupitre.Progress.Domain.Events;

internal record LearningProgressBlockchainRegistered(Guid LearningProgressId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
