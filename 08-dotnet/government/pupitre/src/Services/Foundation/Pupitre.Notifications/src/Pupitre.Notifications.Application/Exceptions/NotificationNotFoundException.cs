using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Notifications.Domain.Entities;

namespace Pupitre.Notifications.Application.Exceptions;

internal class NotificationNotFoundException : MameyException
{
    public NotificationNotFoundException(NotificationId notificationId)
        : base($"Notification with ID: '{notificationId.Value}' was not found.")
        => NotificationId = notificationId;

    public NotificationId NotificationId { get; }
}

