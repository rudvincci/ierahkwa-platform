namespace HRM.Core.Models;

public class Recruitment
{
    public Guid Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, InProgress, Filled, Cancelled
    public int Vacancies { get; set; }
    public int Applicants { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
