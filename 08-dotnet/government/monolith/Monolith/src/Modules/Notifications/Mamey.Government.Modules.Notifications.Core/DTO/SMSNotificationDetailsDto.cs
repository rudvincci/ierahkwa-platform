using Mamey.Government.Modules.Notifications.Core.DTO;

internal class SMSNotificationDetailsDto : NotificationDto
{
    public SMSNotificationDetailsDto()
    {
    }

    public SMSNotificationDetailsDto(Guid id, string userId, string icon, string title, string from, string to, string message) 
        : base(id, icon, title, userId)
    {
        From = from;
        To = to;
        Message = message;
    }

    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}