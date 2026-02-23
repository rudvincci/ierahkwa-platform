namespace SpikeOffice.Core.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
