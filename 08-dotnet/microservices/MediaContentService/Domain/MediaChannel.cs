namespace MediaContentService.Domain;

public class MediaChannel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public Guid TenantId { get; set; }

    // Domain-specific
    public string ChannelType { get; set; } = string.Empty;
    public long SubscriberCount { get; set; }
    public string StreamUrl { get; set; } = string.Empty;
}
