namespace SpikeOffice.Core.Entities;

/// <summary>
/// Clock in/out with IP for restrictions.
/// </summary>
public class ClockInOut : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public bool IsClockIn { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }
    public bool IpAllowed { get; set; } = true;
}
