namespace Mamey.FWID.Identities.Application.AML.Models;

/// <summary>
/// Suspicious Activity Report (SAR).
/// </summary>
public class SuspiciousActivityReport
{
    /// <summary>
    /// SAR ID.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// SAR reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = null!;
    
    /// <summary>
    /// Subject identity ID.
    /// </summary>
    public Guid SubjectIdentityId { get; set; }
    
    /// <summary>
    /// Subject name.
    /// </summary>
    public string SubjectName { get; set; } = null!;
    
    /// <summary>
    /// Subject DID.
    /// </summary>
    public string? SubjectDID { get; set; }
    
    /// <summary>
    /// Trigger that caused the SAR.
    /// </summary>
    public SARTrigger Trigger { get; set; }
    
    /// <summary>
    /// SAR type/category.
    /// </summary>
    public SARType Type { get; set; }
    
    /// <summary>
    /// Priority level.
    /// </summary>
    public SARPriority Priority { get; set; }
    
    /// <summary>
    /// Current status.
    /// </summary>
    public SARStatus Status { get; set; } = SARStatus.Draft;
    
    /// <summary>
    /// Narrative description of suspicious activity.
    /// </summary>
    public string Narrative { get; set; } = null!;
    
    /// <summary>
    /// Supporting evidence.
    /// </summary>
    public List<SAREvidence> Evidence { get; set; } = new();
    
    /// <summary>
    /// Related transactions (if applicable).
    /// </summary>
    public List<SARRelatedTransaction> RelatedTransactions { get; set; } = new();
    
    /// <summary>
    /// When activity was detected.
    /// </summary>
    public DateTime ActivityDetectedAt { get; set; }
    
    /// <summary>
    /// When SAR was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Who created the SAR.
    /// </summary>
    public Guid CreatedBy { get; set; }
    
    /// <summary>
    /// SAR workflow history.
    /// </summary>
    public List<SARWorkflowStep> WorkflowHistory { get; set; } = new();
    
    /// <summary>
    /// Current assignee.
    /// </summary>
    public Guid? AssignedTo { get; set; }
    
    /// <summary>
    /// When filed with regulatory body.
    /// </summary>
    public DateTime? FiledAt { get; set; }
    
    /// <summary>
    /// Filing confirmation number.
    /// </summary>
    public string? FilingConfirmation { get; set; }
    
    /// <summary>
    /// Regulatory body filed with.
    /// </summary>
    public string? FiledWith { get; set; }
    
    /// <summary>
    /// Due date for filing.
    /// </summary>
    public DateTime DueDate { get; set; }
    
    /// <summary>
    /// Whether filing deadline was met.
    /// </summary>
    public bool? DeadlineMet { get; set; }
}

/// <summary>
/// SAR trigger types.
/// </summary>
public enum SARTrigger
{
    RiskScoreThreshold = 1,
    SanctionsMatch = 2,
    TransactionPatternAnomaly = 3,
    IdentityVelocityAnomaly = 4,
    ManualFlag = 5,
    ThirdPartyAlert = 6,
    BehavioralAnomaly = 7,
    CrossZoneAnomaly = 8
}

/// <summary>
/// SAR type/category.
/// </summary>
public enum SARType
{
    StructuringActivity = 1,
    UnusualTransaction = 2,
    IdentityFraud = 3,
    MoneyLaundering = 4,
    TerrorismFinancing = 5,
    SanctionsViolation = 6,
    BriberyCorruption = 7,
    Other = 99
}

/// <summary>
/// SAR priority.
/// </summary>
public enum SARPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// SAR status.
/// </summary>
public enum SARStatus
{
    Draft = 1,
    PendingReview = 2,
    UnderReview = 3,
    ApprovalRequired = 4,
    Approved = 5,
    Filed = 6,
    Confirmed = 7,
    Closed = 8,
    Rejected = 9
}

/// <summary>
/// Evidence attached to SAR.
/// </summary>
public class SAREvidence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileReference { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public Guid AddedBy { get; set; }
}

/// <summary>
/// Related transaction in SAR.
/// </summary>
public class SARRelatedTransaction
{
    public Guid TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string? Description { get; set; }
    public string? Counterparty { get; set; }
}

/// <summary>
/// SAR workflow step.
/// </summary>
public class SARWorkflowStep
{
    public SARStatus FromStatus { get; set; }
    public SARStatus ToStatus { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid PerformedBy { get; set; }
    public string? Comments { get; set; }
}
