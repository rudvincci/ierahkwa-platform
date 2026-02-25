namespace Ierahkwa.AIEngineService.Domain;

public class Dataset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string DatasetType { get; set; } = string.Empty;
    public long RecordCount { get; set; }
    public string StorageUrl { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
}
