using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Documents;

internal class EmailNotificationDocument : NotificationDocument
{
    public EmailNotificationDocument(EmailNotification notification) : base(notification)
    {
        UserId = notification.UserId;
        Subject = notification.Subject;
        ToList = notification.ToList;
        CcList = notification.CcList;
        BccList = notification.BccList;
    }
    public new Guid UserId { get; set; }
    public string Subject { get; set; }
    public IEnumerable<string> ToList { get; set; }
    public IEnumerable<string>? CcList { get; set; }
    public IEnumerable<string>? BccList { get; set; }

    public new EmailNotification AsEntity()
        => new EmailNotification(Id, UserId, Icon, Title, Message, Category.ToEnum<NotificationCategory>(),
            Timestamp.GetDate(), Subject, ToList, CcList, BccList);

    public new EmailNotificationDetailsDto AsDetailsDto()
        => new EmailNotificationDetailsDto(Id, UserId.ToString(), Icon, Title, Message, Category, Timestamp.GetDate(), 
            IsRead, Subject, ToList, CcList, BccList);
}