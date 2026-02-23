using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

public class Attendance : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public TimeOnly? ClockIn { get; set; }
    public TimeOnly? ClockOut { get; set; }
    public decimal? WorkHours { get; set; }
    public string? Notes { get; set; }
}
