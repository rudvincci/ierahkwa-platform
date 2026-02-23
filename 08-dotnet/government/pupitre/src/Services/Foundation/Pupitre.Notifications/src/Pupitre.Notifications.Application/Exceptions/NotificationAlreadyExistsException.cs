using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Notifications.Domain.Entities;

namespace Pupitre.Notifications.Application.Exceptions;

internal class NotificationAlreadyExistsException : MameyException
{
    public NotificationAlreadyExistsException(NotificationId notificationId)
        : base($"Notification with ID: '{notificationId.Value}' already exists.")
        => NotificationId = notificationId;

    public NotificationId NotificationId { get; }
}
