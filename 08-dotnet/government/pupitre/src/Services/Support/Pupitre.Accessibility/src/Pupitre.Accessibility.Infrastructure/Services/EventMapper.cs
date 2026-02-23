using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Accessibility.Application.Events;
using Pupitre.Accessibility.Domain.Events;

namespace Pupitre.Accessibility.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // AccessProfile
            AccessProfileCreated e => null, // Event published thru handler
            AccessProfileModified e => new AccessProfileUpdated(e.AccessProfile.Id),
            AccessProfileRemoved e => new AccessProfileDeleted(e.AccessProfile.Id),
            _ => null
        };
}

