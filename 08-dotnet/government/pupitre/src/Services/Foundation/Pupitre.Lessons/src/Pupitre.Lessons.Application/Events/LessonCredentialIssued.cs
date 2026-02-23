using Mamey.CQRS.Events;

namespace Pupitre.Lessons.Application.Events;

internal record LessonCredentialIssued(Guid LessonId, string IdentityId, string? LedgerTransactionId) : IEvent;
