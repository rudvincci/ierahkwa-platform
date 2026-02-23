using Common.Domain.Entities;

namespace Receptionist.Domain.Entities;

public class VisitorBook : TenantEntity
{
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IdNumber { get; set; }
    public string? Purpose { get; set; }
    public string? PersonToMeet { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutTime { get; set; }
    public string? Badge { get; set; }
    public string? Notes { get; set; }
    public int? StudentId { get; set; }
}
