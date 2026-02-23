namespace HRM.Core.Models;

public class RewardPoints
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public int Points { get; set; }
    public string Source { get; set; } = string.Empty; // Award, Project, Bonus
    public string? ReferenceId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
