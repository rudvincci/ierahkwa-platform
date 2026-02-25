namespace Ierahkwa.SportsService.Domain;

public class Athlete
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
    public Guid TeamId { get; set; }
    public string Position { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
}
