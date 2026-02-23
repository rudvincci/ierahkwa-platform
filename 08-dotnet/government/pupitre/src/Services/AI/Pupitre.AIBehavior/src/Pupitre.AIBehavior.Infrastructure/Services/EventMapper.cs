using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIBehavior.Application.Events;
using Pupitre.AIBehavior.Domain.Events;

namespace Pupitre.AIBehavior.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Behavior
            BehaviorCreated e => null, // Event published thru handler
            BehaviorModified e => new BehaviorUpdated(e.Behavior.Id),
            BehaviorRemoved e => new BehaviorDeleted(e.Behavior.Id),
            _ => null
        };
}

