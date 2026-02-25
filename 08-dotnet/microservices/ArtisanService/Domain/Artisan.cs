namespace Ierahkwa.ArtisanService.Domain;

public class Artisan
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
    public string TribeName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public string Region { get; set; } = string.Empty;
}
