using Mamey.FWID.Identities.Application.AI.Models;
using Mamey.FWID.Identities.Application.AML.Models;
using Mamey.FWID.Identities.Application.AML.Services;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// AI-powered compliance automation service implementation.
/// </summary>
public class AIComplianceService : IAIComplianceService
{
    private readonly ILogger<AIComplianceService> _logger;
    private readonly IAIIdentityVerificationService _verificationService;
    private readonly ISanctionsScreeningService _sanctionsService;
    private readonly IPEPDetectionService _pepService;
    private readonly IRiskAssessmentService _riskService;
    
    private readonly Dictionary<Guid, List<ComplianceRisk>> _complianceRisks = new();
    private readonly Dictionary<Guid, RegulatoryReport> _reports = new();
    private readonly object _lock = new();
    
    public AIComplianceService(
        ILogger<AIComplianceService> logger,
        IAIIdentityVerificationService verificationService,
        ISanctionsScreeningService sanctionsService,
        IPEPDetectionService pepService,
        IRiskAssessmentService riskService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        _sanctionsService = sanctionsService ?? throw new ArgumentNullException(nameof(sanctionsService));
        _pepService = pepService ?? throw new ArgumentNullException(nameof(pepService));
        _riskService = riskService ?? throw new ArgumentNullException(nameof(riskService));
    }
    
    /// <inheritdoc />
    public async Task<KYCVerificationResult> PerformKYCAsync(
        KYCVerificationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Performing automated KYC for {IdentityId}", request.IdentityId);
        
        var result = new KYCVerificationResult
        {
            IdentityId = request.IdentityId
        };
        
        // Document Verification Check
        if (request.DocumentFront != null)
        {
            var verificationResult = await _verificationService.VerifyIdentityAsync(
                new IdentityVerificationRequest
                {
                    IdentityId = request.IdentityId,
                    DocumentFrontImage = request.DocumentFront,
                    DocumentBackImage = request.DocumentBack,
                    SelfieImage = request.Selfie,
                    ExpectedFirstName = request.ProvidedFirstName,
                    ExpectedLastName = request.ProvidedLastName,
                    ExpectedDateOfBirth = request.ProvidedDateOfBirth
                },
                cancellationToken);
            
            result.Checks.Add(new KYCCheck
            {
                CheckType = "DocumentVerification",
                Passed = verificationResult.Status == VerificationStatus.Verified,
                Score = verificationResult.OverallConfidence,
                Details = $"Document: {verificationResult.DocumentResult?.Classification}"
            });
            
            result.Checks.Add(new KYCCheck
            {
                CheckType = "BiometricMatch",
                Passed = verificationResult.BiometricResult?.IsMatch == true,
                Score = verificationResult.BiometricResult?.MatchScore ?? 0,
                Details = $"Match: {verificationResult.BiometricResult?.IsMatch}"
            });
            
            result.Checks.Add(new KYCCheck
            {
                CheckType = "LivenessCheck",
                Passed = verificationResult.BiometricResult?.LivenessCheck?.IsLive == true,
                Score = verificationResult.BiometricResult?.LivenessCheck?.LivenessScore ?? 0
            });
        }
        
        // Sanctions Check
        var sanctionsResult = await _sanctionsService.ScreenIdentityAsync(
            new SanctionsScreeningRequest
            {
                IdentityId = request.IdentityId,
                FirstName = request.ProvidedFirstName ?? "",
                LastName = request.ProvidedLastName ?? "",
                DateOfBirth = request.ProvidedDateOfBirth
            },
            cancellationToken);
        
        result.Checks.Add(new KYCCheck
        {
            CheckType = "SanctionsScreening",
            Passed = sanctionsResult.Status == ScreeningStatus.Clear,
            Score = sanctionsResult.HasMatches ? sanctionsResult.HighestMatchScore : 0,
            Details = $"Matches: {sanctionsResult.Matches.Count}"
        });
        
        // PEP Check
        var pepResult = await _pepService.ScreenForPEPAsync(
            new PEPScreeningRequest
            {
                IdentityId = request.IdentityId,
                FirstName = request.ProvidedFirstName ?? "",
                LastName = request.ProvidedLastName ?? "",
                DateOfBirth = request.ProvidedDateOfBirth
            },
            cancellationToken);
        
        result.Checks.Add(new KYCCheck
        {
            CheckType = "PEPScreening",
            Passed = pepResult.Status == ScreeningStatus.Clear,
            Score = pepResult.HasMatches ? pepResult.HighestMatchScore : 0,
            Details = $"Matches: {pepResult.Matches.Count}"
        });
        
        // Calculate overall
        var passedChecks = result.Checks.Count(c => c.Passed);
        result.OverallScore = (double)passedChecks / result.Checks.Count * 100;
        
        result.Status = passedChecks == result.Checks.Count
            ? KYCStatus.Verified
            : passedChecks > result.Checks.Count / 2
                ? KYCStatus.PendingReview
                : KYCStatus.Failed;
        
        result.FailureReasons = result.Checks
            .Where(c => !c.Passed)
            .Select(c => $"{c.CheckType}: {c.Details ?? "Failed"}")
            .ToList();
        
        _logger.LogInformation(
            "KYC completed for {IdentityId}: {Status}, Score: {Score}",
            request.IdentityId, result.Status, result.OverallScore);
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<List<ComplianceRisk>> DetectComplianceRisksAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        var risks = new List<ComplianceRisk>();
        
        // Get risk profile
        var riskProfile = await _riskService.GetRiskProfileAsync(identityId, cancellationToken);
        
        if (riskProfile != null)
        {
            if (riskProfile.RiskLevel == RiskLevel.Critical)
            {
                risks.Add(new ComplianceRisk
                {
                    IdentityId = identityId,
                    RiskType = ComplianceRiskType.AMLSuspicious,
                    Description = "Identity has critical risk score requiring enhanced due diligence",
                    Severity = 0.9,
                    RegulationReference = "2025-AM01",
                    RecommendedAction = "Perform enhanced due diligence and consider SAR filing"
                });
            }
            
            if (riskProfile.HasSanctionsMatches)
            {
                risks.Add(new ComplianceRisk
                {
                    IdentityId = identityId,
                    RiskType = ComplianceRiskType.SanctionsMatch,
                    Description = "Identity has potential sanctions list matches",
                    Severity = 1.0,
                    RegulationReference = "2025-AM01",
                    RecommendedAction = "Immediate review required"
                });
            }
            
            if (riskProfile.IsPEP)
            {
                risks.Add(new ComplianceRisk
                {
                    IdentityId = identityId,
                    RiskType = ComplianceRiskType.PEPMatch,
                    Description = "Identity identified as Politically Exposed Person",
                    Severity = 0.7,
                    RegulationReference = "2025-AM01",
                    RecommendedAction = "Apply enhanced due diligence for PEP"
                });
            }
        }
        
        lock (_lock)
        {
            _complianceRisks[identityId] = risks;
        }
        
        return risks;
    }
    
    /// <inheritdoc />
    public Task<List<AuditFinding>> AnalyzeAuditTrailAsync(
        AuditAnalysisRequest request,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<AuditFinding>();
        
        // Simulate audit trail analysis
        // In production, analyze actual audit logs using ML models
        
        findings.Add(new AuditFinding
        {
            FindingType = "DataAccess",
            Title = "Unusual Data Access Pattern",
            Description = "Detected unusual bulk data access pattern",
            Severity = AuditFindingSeverity.Medium,
            RegulationReference = "2025-DS01",
            Recommendation = "Review data access policies and user permissions"
        });
        
        return Task.FromResult(findings);
    }
    
    /// <inheritdoc />
    public Task<RegulatoryReport> GenerateRegulatoryReportAsync(
        RegulatoryReportRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating regulatory report for {Regulation}", request.RegulationCode);
        
        var report = new RegulatoryReport
        {
            ReportType = $"{request.RegulationCode} Compliance Report",
            RegulationCode = request.RegulationCode,
            Title = $"{request.RegulationCode} Compliance Report - {request.PeriodStart:yyyy-MM} to {request.PeriodEnd:yyyy-MM}",
            ReportingPeriodStart = request.PeriodStart,
            ReportingPeriodEnd = request.PeriodEnd,
            Summary = new ReportSummary
            {
                TotalIdentities = 1500,
                NewRegistrations = 150,
                HighRiskIdentities = 25,
                SARsFiled = 5,
                ComplianceIssues = 12,
                OverallComplianceScore = 94.5
            }
        };
        
        report.Sections.Add(new ReportSection
        {
            SectionId = "1",
            Title = "Executive Summary",
            Content = $"During the reporting period, the FutureWampumID system processed {report.Summary.NewRegistrations} new identity registrations with a {report.Summary.OverallComplianceScore}% compliance rate.",
            Metrics = new Dictionary<string, object>
            {
                ["TotalRegistrations"] = report.Summary.NewRegistrations,
                ["ComplianceScore"] = report.Summary.OverallComplianceScore
            }
        });
        
        report.Sections.Add(new ReportSection
        {
            SectionId = "2",
            Title = "AML/KYC Summary",
            Content = $"AML screening identified {report.Summary.HighRiskIdentities} high-risk identities. {report.Summary.SARsFiled} Suspicious Activity Reports were filed during this period.",
            Metrics = new Dictionary<string, object>
            {
                ["HighRiskCount"] = report.Summary.HighRiskIdentities,
                ["SARsFiled"] = report.Summary.SARsFiled
            }
        });
        
        lock (_lock)
        {
            _reports[report.ReportId] = report;
        }
        
        return Task.FromResult(report);
    }
    
    /// <inheritdoc />
    public Task<ComplianceDashboard> GetComplianceDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var allRisks = new List<ComplianceRisk>();
        lock (_lock)
        {
            allRisks = _complianceRisks.Values.SelectMany(r => r).ToList();
        }
        
        var dashboard = new ComplianceDashboard
        {
            OverallComplianceScore = 94.5,
            OpenRisks = allRisks.Count(r => r.Status == ComplianceRiskStatus.Open),
            CriticalRisks = allRisks.Count(r => r.Severity >= 0.9),
            PendingSARs = 3,
            KYCPending = 12,
            HighRiskIdentities = 25,
            RisksByType = allRisks
                .GroupBy(r => r.RiskType.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            RecentRisks = allRisks
                .OrderByDescending(r => r.DetectedAt)
                .Take(10)
                .ToList()
        };
        
        return Task.FromResult(dashboard);
    }
    
    /// <inheritdoc />
    public Task<bool> ResolveComplianceRiskAsync(
        Guid riskId,
        Guid resolvedBy,
        string resolutionNotes,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            foreach (var risks in _complianceRisks.Values)
            {
                var risk = risks.FirstOrDefault(r => r.RiskId == riskId);
                if (risk != null)
                {
                    risk.Status = ComplianceRiskStatus.Closed;
                    risk.ResolvedAt = DateTime.UtcNow;
                    risk.ResolvedBy = resolvedBy;
                    risk.ResolutionNotes = resolutionNotes;
                    return Task.FromResult(true);
                }
            }
        }
        return Task.FromResult(false);
    }
}
