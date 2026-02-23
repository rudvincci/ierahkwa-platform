using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to read a notification that has already been read.
/// </summary>
internal class NotificationAlreadyReadException : Exception
{
    public NotificationId NotificationId { get; }

    public NotificationAlreadyReadException(NotificationId notificationId)
        : base($"Notification {notificationId.Value} has already been read.")
    {
        NotificationId = notificationId;
    }
}







