using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.ServiceName.Application.Events;
using Mamey.ServiceName.Domain.Events;

namespace Mamey.ServiceName.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // EntityName
            EntityNameCreated e => null, // Event published thru handler
            EntityNameModified e => new EntityNameUpdated(e.EntityName.Id),
            EntityNameRemoved e => new EntityNameDeleted(e.EntityName.Id),
            _ => null
        };
}

