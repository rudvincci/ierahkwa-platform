namespace DevToolsService.Domain;

public class CodeSnippet
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

    // Domain-specific properties
    public string Language { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}
