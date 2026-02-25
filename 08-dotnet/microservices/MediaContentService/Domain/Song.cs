namespace MediaContentService.Domain;

public class Song
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
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Genre { get; set; } = string.Empty;
}
