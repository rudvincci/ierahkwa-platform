namespace Ierahkwa.DevToolsService.Domain;

public class DevEnvironment
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string IdeType { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int MemoryMb { get; set; }
}
