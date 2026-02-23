namespace Mamey.FWID.Notifications.Domain.Entities;

/// <summary>
/// Notification type enum with flags support.
/// </summary>
[Flags]
internal enum NotificationType
{
    Email = 1 << 0,
    Sms = 1 << 1,
    Push = 1 << 2,
    InApp = 1 << 3,
    All = Email | Sms | Push | InApp
}







