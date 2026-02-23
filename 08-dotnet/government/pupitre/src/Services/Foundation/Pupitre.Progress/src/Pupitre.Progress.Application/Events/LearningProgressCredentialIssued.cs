using Mamey.CQRS.Events;

namespace Pupitre.Progress.Application.Events;

internal record LearningProgressCredentialIssued(Guid LearningProgressId, string IdentityId, string? LedgerTransactionId) : IEvent;
