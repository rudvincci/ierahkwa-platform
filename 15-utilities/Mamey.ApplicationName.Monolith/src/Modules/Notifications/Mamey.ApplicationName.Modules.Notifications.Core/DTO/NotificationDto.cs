namespace Mamey.ApplicationName.Modules.Notifications.Core.DTO
{
    public class NotificationDto
    {
        public NotificationDto(){}
        public NotificationDto(Guid id, string icon, string title, string? userId)
        {
            Id = id;
            Icon = icon;
            Title = title;
            UserId = userId;
        }

        public Guid Id { get; set; } = Guid.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        
    }

    public class NotificationDetailsDto : NotificationDto
    {
        public NotificationDetailsDto(){}

        public NotificationDetailsDto(Guid id, string icon, string title, string message, string category, DateTime timestamp, bool isRead, string? userId = null) 
            : base(id, icon, title, userId)
        {
            Message = message;
            Timestamp = timestamp;
            IsRead = isRead;
            Category = category;
        }

        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string Category { get; set; } = string.Empty;
        
        
        
    }
}