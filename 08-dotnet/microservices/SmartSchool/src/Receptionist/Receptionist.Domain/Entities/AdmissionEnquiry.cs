using Common.Domain.Entities;

namespace Receptionist.Domain.Entities;

public class AdmissionEnquiry : TenantEntity
{
    public string StudentName { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? GradeId { get; set; }
    public DateTime EnquiryDate { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public EnquiryStatus Status { get; set; } = EnquiryStatus.New;
    public DateTime? FollowUpDate { get; set; }
    public int? AssignedTo { get; set; }
}

public enum EnquiryStatus
{
    New,
    InProgress,
    Converted,
    Lost,
    Closed
}
