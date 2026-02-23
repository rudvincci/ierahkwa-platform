namespace SpikeOffice.Core.Entities;

/// <summary>IP whitelist for clock in/out when IP restriction is enabled.</summary>
public class AllowedClockInIp : BaseEntity
{
    public string IpAddress { get; set; } = string.Empty;
    public string? Label { get; set; }
    public Guid? EmployeeId { get; set; } // null = allowed for all
    public Employee? Employee { get; set; }
}
