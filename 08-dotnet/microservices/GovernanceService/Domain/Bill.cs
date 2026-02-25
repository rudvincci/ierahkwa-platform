namespace Ierahkwa.GovernanceService.Domain;

public class Bill
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
    public string BillNumber { get; set; } = string.Empty;
    public string Sponsor { get; set; } = string.Empty;
    public DateTime IntroducedDate { get; set; }
    public string Committee { get; set; } = string.Empty;
    public int ReadingStage { get; set; }
}
