namespace Mamey.FWID.Identities.Application.ClanRegistrar.Models;

/// <summary>
/// Represents a clan registrar - an authorized person who can approve identity registrations.
/// </summary>
public class ClanRegistrar
{
    /// <summary>
    /// Unique identifier for the registrar.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The identity ID of the registrar.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// The DID of the registrar.
    /// </summary>
    public string DID { get; set; } = null!;
    
    /// <summary>
    /// The type of registrar.
    /// </summary>
    public RegistrarType Type { get; set; }
    
    /// <summary>
    /// The sovereign zone this registrar is authorized for.
    /// </summary>
    public string Zone { get; set; } = null!;
    
    /// <summary>
    /// The clan name (for clan registrars).
    /// </summary>
    public string? ClanName { get; set; }
    
    /// <summary>
    /// The institution name (for institutional registrars).
    /// </summary>
    public string? InstitutionName { get; set; }
    
    /// <summary>
    /// The registrar's title/role.
    /// </summary>
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Whether the registrar is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date the registrar was appointed.
    /// </summary>
    public DateTime AppointedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date the registrar's authority expires (if applicable).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Who appointed this registrar.
    /// </summary>
    public Guid? AppointedBy { get; set; }
    
    /// <summary>
    /// The scope of authority for this registrar.
    /// </summary>
    public RegistrarScope Scope { get; set; } = new();
    
    /// <summary>
    /// Backup registrars who can act on behalf of this registrar.
    /// </summary>
    public List<Guid> DelegateRegistrarIds { get; set; } = new();
    
    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Type of registrar.
/// </summary>
public enum RegistrarType
{
    /// <summary>
    /// Clan matriarch - primary authority for clan-based identity.
    /// </summary>
    ClanMatriarch = 1,
    
    /// <summary>
    /// Clan elder - secondary authority within a clan.
    /// </summary>
    ClanElder = 2,
    
    /// <summary>
    /// Institutional registrar - government entities.
    /// </summary>
    Institutional = 3,
    
    /// <summary>
    /// Delegate registrar - backup with limited scope.
    /// </summary>
    Delegate = 4
}

/// <summary>
/// Scope of authority for a registrar.
/// </summary>
public class RegistrarScope
{
    /// <summary>
    /// Can approve new identity registrations.
    /// </summary>
    public bool CanApproveRegistrations { get; set; } = true;
    
    /// <summary>
    /// Can issue credentials.
    /// </summary>
    public bool CanIssueCredentials { get; set; } = true;
    
    /// <summary>
    /// Can revoke credentials.
    /// </summary>
    public bool CanRevokeCredentials { get; set; } = false;
    
    /// <summary>
    /// Can approve guardian delegations.
    /// </summary>
    public bool CanApproveGuardianship { get; set; } = true;
    
    /// <summary>
    /// Can delegate authority to others.
    /// </summary>
    public bool CanDelegate { get; set; } = false;
    
    /// <summary>
    /// Maximum number of pending approvals allowed.
    /// </summary>
    public int MaxPendingApprovals { get; set; } = 100;
    
    /// <summary>
    /// Types of credentials that can be issued.
    /// </summary>
    public List<string> AllowedCredentialTypes { get; set; } = new() { "IdentityCredential" };
}

/// <summary>
/// Represents an identity registration awaiting approval.
/// </summary>
public class RegistrationApproval
{
    /// <summary>
    /// Unique identifier for the approval request.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The identity ID being registered.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// The registrar assigned to approve this registration.
    /// </summary>
    public Guid RegistrarId { get; set; }
    
    /// <summary>
    /// Current status of the approval.
    /// </summary>
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    
    /// <summary>
    /// Date the request was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date the request was reviewed.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    
    /// <summary>
    /// Registrar who reviewed the request.
    /// </summary>
    public Guid? ReviewedBy { get; set; }
    
    /// <summary>
    /// Decision made by the registrar.
    /// </summary>
    public ApprovalDecision? Decision { get; set; }
    
    /// <summary>
    /// Reason for the decision.
    /// </summary>
    public string? DecisionReason { get; set; }
    
    /// <summary>
    /// Required documents for approval.
    /// </summary>
    public List<RequiredDocument> RequiredDocuments { get; set; } = new();
    
    /// <summary>
    /// Biometric verification result.
    /// </summary>
    public BiometricVerificationStatus BiometricStatus { get; set; } = BiometricVerificationStatus.Pending;
    
    /// <summary>
    /// If escalated, the council handling it.
    /// </summary>
    public Guid? EscalatedToCouncilId { get; set; }
    
    /// <summary>
    /// Priority level for processing.
    /// </summary>
    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;
    
    /// <summary>
    /// Notes from the registrar.
    /// </summary>
    public string? RegistrarNotes { get; set; }
    
    /// <summary>
    /// Workflow history.
    /// </summary>
    public List<ApprovalHistoryEntry> History { get; set; } = new();
}

/// <summary>
/// Status of an approval request.
/// </summary>
public enum ApprovalStatus
{
    Pending = 1,
    InReview = 2,
    AdditionalInfoRequired = 3,
    Approved = 4,
    Rejected = 5,
    Escalated = 6,
    Cancelled = 7
}

/// <summary>
/// Decision made by registrar.
/// </summary>
public enum ApprovalDecision
{
    Approved = 1,
    Rejected = 2,
    NeedsMoreInfo = 3,
    Escalate = 4
}

/// <summary>
/// Biometric verification status.
/// </summary>
public enum BiometricVerificationStatus
{
    Pending = 1,
    Verified = 2,
    Failed = 3,
    NotRequired = 4
}

/// <summary>
/// Priority level for approval processing.
/// </summary>
public enum ApprovalPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4
}

/// <summary>
/// Document required for approval.
/// </summary>
public class RequiredDocument
{
    public string DocumentType { get; set; } = null!;
    public string? DocumentId { get; set; }
    public bool IsProvided { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

/// <summary>
/// History entry for approval workflow.
/// </summary>
public class ApprovalHistoryEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = null!;
    public Guid? ActorId { get; set; }
    public string? Notes { get; set; }
    public ApprovalStatus FromStatus { get; set; }
    public ApprovalStatus ToStatus { get; set; }
}
