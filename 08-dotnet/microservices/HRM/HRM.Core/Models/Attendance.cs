namespace HRM.Core.Models;

public class Attendance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Status { get; set; } // Present, Absent, Late, HalfDay, Leave, Holiday
    public string? Notes { get; set; }
    public int? LateMinutes { get; set; }
    public int? EarlyCloseMinutes { get; set; }
}
