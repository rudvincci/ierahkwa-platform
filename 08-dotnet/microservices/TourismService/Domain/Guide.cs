namespace TourismService.Domain;

public class Guide
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public Guid TenantId { get; set; }

    // Domain-specific
    public string Languages { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
}
