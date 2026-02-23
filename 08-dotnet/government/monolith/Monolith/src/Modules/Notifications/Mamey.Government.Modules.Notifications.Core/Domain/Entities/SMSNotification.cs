using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Entities;

internal class SMSNotification : Notification
{
    public SMSNotification(Guid id, UserId userId, string icon, string title, string message, NotificationCategory category, DateTime timestamp, 
        string from, string to) 
        : base(id, icon, title, message, category, timestamp, userId)
    {
        UserId = userId;
    }
    public static SMSNotification Create(Guid id, Guid userId, string title, NotificationCategory category, string icon, string from, string to, string message)
    {
        var notification = new SMSNotification(id, userId, icon, title, message, category, DateTime.UtcNow, from, to);
        return notification;
    }
    public new UserId UserId { get; set; }
    public string From { get; set; }
    public string To { get; set; }
}