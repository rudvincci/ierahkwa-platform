namespace Ierahkwa.GenomicsService.Domain;

public class BiobankEntry
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
    public Guid SampleId { get; set; }
    public string StorageLocation { get; set; } = string.Empty;
    public decimal TemperatureCelsius { get; set; }
    public DateTime ExpiryDate { get; set; }
}
