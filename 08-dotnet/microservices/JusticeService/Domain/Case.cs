namespace Ierahkwa.JusticeService.Domain;

public class Case
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
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public Guid PlaintiffId { get; set; }
    public Guid DefendantId { get; set; }
    public string CourtDistrict { get; set; } = string.Empty;
}
