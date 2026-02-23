using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Aftercare.Application.Events;
using Pupitre.Aftercare.Domain.Events;

namespace Pupitre.Aftercare.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // AftercarePlan
            AftercarePlanCreated e => null, // Event published thru handler
            AftercarePlanModified e => new AftercarePlanUpdated(e.AftercarePlan.Id),
            AftercarePlanRemoved e => new AftercarePlanDeleted(e.AftercarePlan.Id),
            _ => null
        };
}

