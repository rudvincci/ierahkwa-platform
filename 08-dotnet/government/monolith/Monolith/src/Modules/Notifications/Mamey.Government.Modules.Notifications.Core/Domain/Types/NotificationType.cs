namespace Mamey.Government.Modules.Notifications.Core.Domain.Types;

[Flags]
internal enum NotificationType
{
    Email = 1 << 0,
    Sms = 1 << 1,
    Client = 1 << 2,
    All = Email | Sms | Client
}