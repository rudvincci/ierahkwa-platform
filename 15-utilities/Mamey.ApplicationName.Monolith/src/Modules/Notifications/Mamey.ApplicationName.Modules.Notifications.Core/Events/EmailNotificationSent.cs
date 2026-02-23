using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events;

internal record EmailNotificationSent(Guid NotificationId, Guid UserId) : IEvent;