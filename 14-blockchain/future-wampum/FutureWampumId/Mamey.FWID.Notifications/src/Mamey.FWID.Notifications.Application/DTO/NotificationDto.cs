namespace Mamey.FWID.Notifications.Application.DTO;

/// <summary>
/// DTO for notification data.
/// </summary>
internal class NotificationDto
{
    public Guid Id { get; set; }
    public Guid IdentityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
}







