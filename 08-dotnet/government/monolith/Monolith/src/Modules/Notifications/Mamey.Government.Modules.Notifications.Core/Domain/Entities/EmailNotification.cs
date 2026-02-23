using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Entities;

internal class EmailNotification : Notification
{
    public EmailNotification(Guid id, UserId userId, string icon, string title, 
        string message, NotificationCategory category, DateTime timestamp, string subject, IEnumerable<string>? toList = null,
        IEnumerable<string>? ccList = null, IEnumerable<string>? bccList = null) 
        : base(id, icon, title, message, category, timestamp, userId)
    {
        UserId = userId;
        if (userId is null)
        {
            throw new ArgumentNullException(nameof(userId));
        }

        if (string.IsNullOrEmpty(icon))
        {
            throw new ArgumentException($"'{nameof(icon)}' cannot be null or empty.", nameof(icon));
        }

        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or empty.", nameof(message));
        }

        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));
        }

        Subject = subject;
        ToList = toList ?? Enumerable.Empty<string>();
        CcList = ccList;
        BccList = bccList;
    }
    
    public new UserId UserId { get; set; }
    public string Subject { get; set; }
    public IEnumerable<string> ToList { get; set; }
    public IEnumerable<string>? CcList { get; set; }
    public IEnumerable<string>? BccList { get; set; }

    public static EmailNotification Create(Guid id, Guid userId, string title, string description, string icon, string subject, NotificationCategory category, IEnumerable<string>? toList = null,
        IEnumerable<string>? ccList = null, IEnumerable<string>? bccList = null)
    {
        var notification = new EmailNotification(id, userId, icon, title, description, category, DateTime.UtcNow, subject, toList, ccList, bccList);
        return notification;
    }
}