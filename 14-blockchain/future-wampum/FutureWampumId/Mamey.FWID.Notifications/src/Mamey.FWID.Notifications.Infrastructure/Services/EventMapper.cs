using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Notifications.Application.Events.Integration.Notifications;
using Mamey.FWID.Notifications.Domain.Events;

namespace Mamey.FWID.Notifications.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // NotificationCreated is handled internally, not published as integration event
            NotificationCreated e => null,
            // NotificationSent is published as integration event
            NotificationSent e => new NotificationSentIntegrationEvent(
                e.NotificationId.Value,
                e.NotificationId.IdentityId.Value,
                e.Type.ToString(),
                e.SentAt),
            // NotificationRead is published as integration event
            NotificationRead e => new NotificationReadIntegrationEvent(
                e.NotificationId.Value,
                e.NotificationId.IdentityId.Value,
                e.ReadAt),
            // NotificationFailed is internal event, not published
            NotificationFailed e => null,
            _ => null
        };
}

