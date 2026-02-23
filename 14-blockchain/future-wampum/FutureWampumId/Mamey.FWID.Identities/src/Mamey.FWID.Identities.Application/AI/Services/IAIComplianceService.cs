using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered compliance automation service.
/// </summary>
public interface IAIComplianceService
{
    /// <summary>
    /// Performs automated KYC verification.
    /// </summary>
    Task<KYCVerificationResult> PerformKYCAsync(
        KYCVerificationRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detects compliance risks for an identity.
    /// </summary>
    Task<List<ComplianceRisk>> DetectComplianceRisksAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes audit trail for anomalies.
    /// </summary>
    Task<List<AuditFinding>> AnalyzeAuditTrailAsync(
        AuditAnalysisRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates regulatory compliance report.
    /// </summary>
    Task<RegulatoryReport> GenerateRegulatoryReportAsync(
        RegulatoryReportRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets compliance dashboard metrics.
    /// </summary>
    Task<ComplianceDashboard> GetComplianceDashboardAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resolves a compliance risk.
    /// </summary>
    Task<bool> ResolveComplianceRiskAsync(
        Guid riskId,
        Guid resolvedBy,
        string resolutionNotes,
        CancellationToken cancellationToken = default);
}

public class KYCVerificationRequest
{
    public Guid IdentityId { get; set; }
    public byte[]? DocumentFront { get; set; }
    public byte[]? DocumentBack { get; set; }
    public byte[]? Selfie { get; set; }
    public string? ProvidedFirstName { get; set; }
    public string? ProvidedLastName { get; set; }
    public DateTime? ProvidedDateOfBirth { get; set; }
    public string? ProvidedAddress { get; set; }
}

public class KYCVerificationResult
{
    public Guid VerificationId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public KYCStatus Status { get; set; }
    public double OverallScore { get; set; }
    public List<KYCCheck> Checks { get; set; } = new();
    public List<string> FailureReasons { get; set; } = new();
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}

public enum KYCStatus
{
    Verified = 1,
    PendingReview = 2,
    Failed = 3,
    Incomplete = 4
}

public class KYCCheck
{
    public string CheckType { get; set; } = null!;
    public bool Passed { get; set; }
    public double Score { get; set; }
    public string? Details { get; set; }
}

public class AuditAnalysisRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? FocusAreas { get; set; }
    public List<Guid>? SpecificIdentities { get; set; }
}

public class RegulatoryReportRequest
{
    public string RegulationCode { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public bool IncludeDetails { get; set; } = true;
}

public class ComplianceDashboard
{
    public double OverallComplianceScore { get; set; }
    public int OpenRisks { get; set; }
    public int CriticalRisks { get; set; }
    public int PendingSARs { get; set; }
    public int KYCPending { get; set; }
    public int HighRiskIdentities { get; set; }
    public Dictionary<string, int> RisksByType { get; set; } = new();
    public List<ComplianceRisk> RecentRisks { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
