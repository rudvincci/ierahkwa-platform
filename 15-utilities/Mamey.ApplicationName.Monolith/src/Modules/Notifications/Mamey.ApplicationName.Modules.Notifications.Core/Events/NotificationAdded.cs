using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events
{
    internal record NotificationAdded(Guid Id, Guid UserId) : IEvent;
}