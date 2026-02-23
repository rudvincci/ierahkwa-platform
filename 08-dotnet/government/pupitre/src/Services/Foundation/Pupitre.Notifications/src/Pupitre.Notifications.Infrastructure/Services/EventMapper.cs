using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Notifications.Application.Events;
using Pupitre.Notifications.Domain.Events;

namespace Pupitre.Notifications.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Notification
            NotificationCreated e => null, // Event published thru handler
            NotificationModified e => new NotificationUpdated(e.Notification.Id),
            NotificationRemoved e => new NotificationDeleted(e.Notification.Id),
            NotificationBlockchainRegistered e => new NotificationCredentialIssued(e.NotificationId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

