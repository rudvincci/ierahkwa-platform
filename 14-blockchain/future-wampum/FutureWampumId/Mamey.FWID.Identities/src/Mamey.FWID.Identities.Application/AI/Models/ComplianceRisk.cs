namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// AI-detected compliance risk.
/// </summary>
public class ComplianceRisk
{
    public Guid RiskId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public ComplianceRiskType RiskType { get; set; }
    public string Description { get; set; } = null!;
    public double Severity { get; set; }
    public string RegulationReference { get; set; } = null!;
    public ComplianceRiskStatus Status { get; set; } = ComplianceRiskStatus.Open;
    public List<string> Evidence { get; set; } = new();
    public string? RecommendedAction { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
}

public enum ComplianceRiskType
{
    KYCIncomplete = 1,
    AMLSuspicious = 2,
    SanctionsMatch = 3,
    PEPMatch = 4,
    IdentityVelocity = 5,
    DocumentAnomaly = 6,
    TransactionPattern = 7,
    HighRiskJurisdiction = 8,
    DataRetention = 9,
    PrivacyViolation = 10
}

public enum ComplianceRiskStatus
{
    Open = 1,
    UnderReview = 2,
    Mitigated = 3,
    Accepted = 4,
    Escalated = 5,
    Closed = 6
}

/// <summary>
/// Audit finding from AI analysis.
/// </summary>
public class AuditFinding
{
    public Guid FindingId { get; set; } = Guid.NewGuid();
    public string FindingType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public AuditFindingSeverity Severity { get; set; }
    public string RegulationReference { get; set; } = null!;
    public List<string> AffectedIdentities { get; set; } = new();
    public string? RootCause { get; set; }
    public string? Recommendation { get; set; }
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
}

public enum AuditFindingSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// AI-generated regulatory report.
/// </summary>
public class RegulatoryReport
{
    public Guid ReportId { get; set; } = Guid.NewGuid();
    public string ReportType { get; set; } = null!;
    public string RegulationCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime ReportingPeriodStart { get; set; }
    public DateTime ReportingPeriodEnd { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Draft;
    public ReportSummary Summary { get; set; } = new();
    public List<ReportSection> Sections { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public enum ReportStatus
{
    Draft = 1,
    PendingReview = 2,
    Approved = 3,
    Submitted = 4,
    Accepted = 5,
    Rejected = 6
}

public class ReportSummary
{
    public int TotalIdentities { get; set; }
    public int NewRegistrations { get; set; }
    public int HighRiskIdentities { get; set; }
    public int SARsFiled { get; set; }
    public int ComplianceIssues { get; set; }
    public double OverallComplianceScore { get; set; }
}

public class ReportSection
{
    public string SectionId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<string> DataPoints { get; set; } = new();
    public Dictionary<string, object> Metrics { get; set; } = new();
}
