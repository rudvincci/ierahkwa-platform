namespace Ierahkwa.SmartFactoryService.Domain;

public class QualityCheck
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
    public Guid WorkOrderId { get; set; }
    public string InspectorId { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string DefectNotes { get; set; } = string.Empty;
}
