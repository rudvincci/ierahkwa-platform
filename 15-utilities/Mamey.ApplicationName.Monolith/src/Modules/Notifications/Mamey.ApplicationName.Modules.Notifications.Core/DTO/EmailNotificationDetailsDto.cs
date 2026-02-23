using Mamey.ApplicationName.Modules.Notifications.Core.DTO;

internal class EmailNotificationDetailsDto : NotificationDetailsDto
{
    public EmailNotificationDetailsDto()
    {
    }

    public EmailNotificationDetailsDto(Guid id, string userId, string icon, string title, 
        string message, string category, DateTime timestamp, bool isRead, string subject, IEnumerable<string> toList,
        IEnumerable<string>? ccList = null, IEnumerable<string>? bccList = null) 
        : base(id, icon, title, message, category, timestamp, isRead, userId)
    {
        Subject = subject;
        ToList = toList ?? Enumerable.Empty<string>();
        CcList = ccList;
        BccList = bccList;
    }
    public string Subject { get; set; }
    public IEnumerable<string> ToList { get; set; }
    public IEnumerable<string>? CcList { get; set; }
    public IEnumerable<string>? BccList { get; set; }
}