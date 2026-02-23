using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events;

internal class SMSNotificationSent(Guid NotificationId, Guid UserId) : IEvent;