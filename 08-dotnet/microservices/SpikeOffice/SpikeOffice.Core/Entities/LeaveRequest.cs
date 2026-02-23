using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

public class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Days { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? RejectedReason { get; set; }
    public Guid? ApprovedById { get; set; }
}
