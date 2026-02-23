using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Operations.Application.Events;
using Pupitre.Operations.Domain.Events;

namespace Pupitre.Operations.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // OperationMetric
            OperationMetricCreated e => null, // Event published thru handler
            OperationMetricModified e => new OperationMetricUpdated(e.OperationMetric.Id),
            OperationMetricRemoved e => new OperationMetricDeleted(e.OperationMetric.Id),
            _ => null
        };
}

