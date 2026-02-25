namespace Ierahkwa.EmergencyService.Domain;

public class EmergencyUnit
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
    public string UnitCode { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public int PersonnelCount { get; set; }
    public string BaseStation { get; set; } = string.Empty;
    public bool IsDeployed { get; set; }
}
