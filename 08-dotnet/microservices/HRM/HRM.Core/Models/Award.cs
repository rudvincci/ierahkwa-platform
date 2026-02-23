namespace HRM.Core.Models;

public class Award
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string AwardName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Best Employee, Excellence, etc.
    public string? Description { get; set; }
    public DateTime AwardDate { get; set; }
    public string? PresentedBy { get; set; }
    public int? RewardPoints { get; set; }
    public DateTime CreatedAt { get; set; }
}
