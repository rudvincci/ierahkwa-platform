using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Lessons.Application.Events;
using Pupitre.Lessons.Domain.Events;

namespace Pupitre.Lessons.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Lesson
            LessonCreated e => null, // Event published thru handler
            LessonModified e => new LessonUpdated(e.Lesson.Id),
            LessonRemoved e => new LessonDeleted(e.Lesson.Id),
            LessonBlockchainRegistered e => new LessonCredentialIssued(e.LessonId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

