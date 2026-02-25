namespace DevToolsService.Domain;

public class BuildPipeline
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
    public string TriggerType { get; set; } = string.Empty;
    public string YamlDefinition { get; set; } = string.Empty;
    public DateTime? LastRunAt { get; set; }
}
