using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AITutors.Application.Events;
using Pupitre.AITutors.Domain.Events;

namespace Pupitre.AITutors.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Tutor
            TutorCreated e => null, // Event published thru handler
            TutorModified e => new TutorUpdated(e.Tutor.Id),
            TutorRemoved e => new TutorDeleted(e.Tutor.Id),
            _ => null
        };
}

