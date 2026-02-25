namespace Ierahkwa.MilitaryService.Domain;

public class Unit
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
    public string Branch { get; set; } = string.Empty;
    public string BaseLocation { get; set; } = string.Empty;
    public int PersonnelCount { get; set; }
    public string CommandingOfficer { get; set; } = string.Empty;
}
