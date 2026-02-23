using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Notifications.Core.Events;

internal class SMSNotificationSent(Guid NotificationId, Guid UserId) : IEvent;