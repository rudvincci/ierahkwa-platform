namespace Ierahkwa.GovernanceService.Domain;

public class Vote
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
    public Guid BillId { get; set; }
    public Guid SessionId { get; set; }
    public int YesCount { get; set; }
    public int NoCount { get; set; }
    public int AbstainCount { get; set; }
}
