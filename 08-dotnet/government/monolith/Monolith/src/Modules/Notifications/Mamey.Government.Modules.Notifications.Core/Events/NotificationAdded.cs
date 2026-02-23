using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Notifications.Core.Events
{
    internal record NotificationAdded(Guid Id, Guid UserId) : IEvent;
}