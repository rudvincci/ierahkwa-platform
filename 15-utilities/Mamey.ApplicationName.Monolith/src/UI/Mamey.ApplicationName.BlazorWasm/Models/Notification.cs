namespace Mamey.ApplicationName.BlazorWasm.Models;

public class Notification
{
    public Notification(Guid id, string icon, string title, string message, DateTime timestamp, bool isRead,
        string category, Guid? userId = null)
    {
        Id = id;
        UserId = userId;
        Icon = icon;
        Title = title;
        Message = message;
        Timestamp = timestamp;
        IsRead = isRead;
        Category = category;
    }

    public Guid Id { get; set; } = Guid.Empty;

    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }

    public string Category { get; set;  }
    
    
}