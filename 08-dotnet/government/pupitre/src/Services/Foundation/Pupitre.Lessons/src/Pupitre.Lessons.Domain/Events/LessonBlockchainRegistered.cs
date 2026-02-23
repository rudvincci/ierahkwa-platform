using Mamey.CQRS;

namespace Pupitre.Lessons.Domain.Events;

internal record LessonBlockchainRegistered(Guid LessonId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
