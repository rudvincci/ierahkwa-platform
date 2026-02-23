using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Analytics.Application.Events;
using Pupitre.Analytics.Domain.Events;

namespace Pupitre.Analytics.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Analytic
            AnalyticCreated e => null, // Event published thru handler
            AnalyticModified e => new AnalyticUpdated(e.Analytic.Id),
            AnalyticRemoved e => new AnalyticDeleted(e.Analytic.Id),
            _ => null
        };
}

