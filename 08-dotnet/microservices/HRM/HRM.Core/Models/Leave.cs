namespace HRM.Core.Models;

public class Leave
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty; // Annual, Sick, Casual, Maternity, Unpaid
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Reason { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
