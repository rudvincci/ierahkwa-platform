using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AISafety.Application.Events;
using Pupitre.AISafety.Domain.Events;

namespace Pupitre.AISafety.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // SafetyCheck
            SafetyCheckCreated e => null, // Event published thru handler
            SafetyCheckModified e => new SafetyCheckUpdated(e.SafetyCheck.Id),
            SafetyCheckRemoved e => new SafetyCheckDeleted(e.SafetyCheck.Id),
            _ => null
        };
}

