namespace Ierahkwa.HealthService.Domain;

public class MentalHealthRecord
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
    // Domain-specific
    public Guid PatientId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string TherapyType { get; set; } = string.Empty;
    public int SessionCount { get; set; }
    public string Severity { get; set; } = "Moderate";
}
