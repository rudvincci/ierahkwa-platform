namespace Ierahkwa.SpaceService.Domain;

public class LaunchMission
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
    public string RocketType { get; set; } = string.Empty;
    public string LaunchSite { get; set; } = string.Empty;
    public DateTime ScheduledLaunch { get; set; }
    public string PayloadDescription { get; set; } = string.Empty;
    public double PayloadMassKg { get; set; }
}
