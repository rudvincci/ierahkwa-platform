using Mamey.Government.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.Government.Modules.Notifications.Core.Mongo.Documents;

internal class SMSNotificationDocument : NotificationDocument
{
    public SMSNotificationDocument(SMSNotification notification) 
        : base(notification)
    {
        UserId = notification.UserId;
        From = notification.From;
        To = notification.To;
    }
    public new Guid UserId { get; set; }
    public string From { get; set; }
    public string To { get; set; }

    public new SMSNotification AsEntity()
        => new SMSNotification(Id, UserId, Icon, Title, Message, 
            Category.ToEnum<NotificationCategory>(), Timestamp.GetDate(), From, To);

    public new SMSNotificationDetailsDto AsDetailsDto()
        => new SMSNotificationDetailsDto(Id, UserId.ToString(), Icon, Title, From, To, Message);
}