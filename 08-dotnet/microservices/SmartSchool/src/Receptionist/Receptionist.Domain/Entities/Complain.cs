using Common.Domain.Entities;

namespace Receptionist.Domain.Entities;

public class Complain : TenantEntity
{
    public string ComplainNumber { get; set; } = string.Empty;
    public string ComplainerName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ComplainType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ComplainDate { get; set; } = DateTime.UtcNow;
    public ComplainStatus Status { get; set; } = ComplainStatus.Open;
    public string? Resolution { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public int? AssignedTo { get; set; }
    public int? StudentId { get; set; }
}

public enum ComplainType
{
    Academic,
    Behavioral,
    Facility,
    Staff,
    Transport,
    Food,
    Other
}

public enum ComplainStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Rejected
}
