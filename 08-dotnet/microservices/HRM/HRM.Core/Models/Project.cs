namespace HRM.Core.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Planning"; // Planning, InProgress, Completed, OnHold
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? LeadId { get; set; }
    public int? RewardPointsOnCompletion { get; set; }
    public decimal? Budget { get; set; }
    public int Progress { get; set; }
    public DateTime CreatedAt { get; set; }
}
