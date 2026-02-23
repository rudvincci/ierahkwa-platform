using Mamey.CQRS.Events;

namespace Mamey.ApplicatGovernmentionName.Modules.Notifications.Core.Events;

internal record EmailNotificationSent(Guid NotificationId, Guid UserId) : IEvent;