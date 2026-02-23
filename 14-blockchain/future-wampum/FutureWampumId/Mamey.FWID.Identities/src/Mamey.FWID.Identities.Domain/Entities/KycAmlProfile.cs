using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a KYC/AML compliance profile aggregate root.
/// </summary>
internal class KycAmlProfile : AggregateRoot<KycAmlProfileId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private KycAmlProfile()
    {
        RiskAssessments = new List<RiskAssessment>();
        ComplianceChecks = new List<ComplianceCheck>();
        SanctionsScreeningResults = new List<SanctionsScreeningResult>();
        PepScreeningResults = new List<PepScreeningResult>();
        AdverseMediaFindings = new List<AdverseMediaFinding>();
    }

    /// <summary>
    /// Initializes a new instance of the KycAmlProfile aggregate root.
    /// </summary>
    /// <param name="id">The KYC/AML profile identifier.</param>
    /// <param name="identityId">The identity this profile belongs to.</param>
    /// <param name="jurisdiction">The regulatory jurisdiction.</param>
    public KycAmlProfile(
        KycAmlProfileId id,
        IdentityId identityId,
        string jurisdiction)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        Jurisdiction = jurisdiction ?? throw new ArgumentNullException(nameof(jurisdiction));
        Status = KycAmlStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        RiskLevel = RiskLevel.Unknown;
        RiskAssessments = new List<RiskAssessment>();
        ComplianceChecks = new List<ComplianceCheck>();
        SanctionsScreeningResults = new List<SanctionsScreeningResult>();
        PepScreeningResults = new List<PepScreeningResult>();
        AdverseMediaFindings = new List<AdverseMediaFinding>();
        Version = 1;

        AddEvent(new KycAmlProfileCreated(Id, IdentityId, Jurisdiction, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity this KYC/AML profile belongs to.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The regulatory jurisdiction for this profile.
    /// </summary>
    public string Jurisdiction { get; private set; }

    /// <summary>
    /// The current status of the KYC/AML process.
    /// </summary>
    public KycAmlStatus Status { get; private set; }

    /// <summary>
    /// The overall risk level assessment.
    /// </summary>
    public RiskLevel RiskLevel { get; private set; }

    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the profile was last updated.
    /// </summary>
    public DateTime? LastUpdatedAt { get; private set; }

    /// <summary>
    /// When the profile was approved.
    /// </summary>
    public DateTime? ApprovedAt { get; private set; }

    /// <summary>
    /// When the profile expires and needs renewal.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// The risk assessments performed.
    /// </summary>
    public List<RiskAssessment> RiskAssessments { get; private set; }

    /// <summary>
    /// The compliance checks performed.
    /// </summary>
    public List<ComplianceCheck> ComplianceChecks { get; private set; }

    /// <summary>
    /// The sanctions screening results.
    /// </summary>
    public List<SanctionsScreeningResult> SanctionsScreeningResults { get; private set; }

    /// <summary>
    /// The PEP (Politically Exposed Person) screening results.
    /// </summary>
    public List<PepScreeningResult> PepScreeningResults { get; private set; }

    /// <summary>
    /// Adverse media findings.
    /// </summary>
    public List<AdverseMediaFinding> AdverseMediaFindings { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Starts the KYC/AML verification process.
    /// </summary>
    public void StartVerification()
    {
        if (Status != KycAmlStatus.Pending)
            throw new InvalidOperationException("KYC/AML verification can only be started from pending status");

        Status = KycAmlStatus.InProgress;
        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new KycAmlVerificationStarted(Id, LastUpdatedAt.Value));
    }

    /// <summary>
    /// Adds a risk assessment to the profile.
    /// </summary>
    /// <param name="assessmentType">The type of risk assessment.</param>
    /// <param name="riskLevel">The assessed risk level.</param>
    /// <param name="score">The risk score (0-100).</param>
    /// <param name="factors">The risk factors considered.</param>
    /// <param name="recommendations">Recommendations for mitigation.</param>
    public void AddRiskAssessment(
        string assessmentType,
        RiskLevel riskLevel,
        int score,
        List<string> factors,
        List<string> recommendations)
    {
        var assessment = new RiskAssessment(
            assessmentType,
            riskLevel,
            score,
            factors,
            recommendations,
            DateTime.UtcNow);

        RiskAssessments.Add(assessment);

        // Update overall risk level if this assessment is more severe
        if ((int)riskLevel > (int)RiskLevel)
        {
            RiskLevel = riskLevel;
        }

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new RiskAssessmentAdded(Id, assessmentType, riskLevel, score));
    }

    /// <summary>
    /// Performs sanctions screening.
    /// </summary>
    /// <param name="screeningProvider">The screening provider used.</param>
    /// <param name="matches">Any matches found.</param>
    /// <param name="confidenceScore">The confidence score of the screening.</param>
    public void PerformSanctionsScreening(
        string screeningProvider,
        List<SanctionsMatch> matches,
        int confidenceScore)
    {
        var result = new SanctionsScreeningResult(
            screeningProvider,
            matches,
            confidenceScore,
            DateTime.UtcNow);

        SanctionsScreeningResults.Add(result);

        // If high-confidence matches found, escalate risk
        if (matches.Any(m => m.ConfidenceScore > 80))
        {
            RiskLevel = RiskLevel.High;
        }

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new SanctionsScreeningCompleted(Id, screeningProvider, matches.Count, confidenceScore));
    }

    /// <summary>
    /// Performs PEP screening.
    /// </summary>
    /// <param name="screeningProvider">The screening provider used.</param>
    /// <param name="matches">Any PEP matches found.</param>
    /// <param name="confidenceScore">The confidence score of the screening.</param>
    public void PerformPepScreening(
        string screeningProvider,
        List<PepMatch> matches,
        int confidenceScore)
    {
        var result = new PepScreeningResult(
            screeningProvider,
            matches,
            confidenceScore,
            DateTime.UtcNow);

        PepScreeningResults.Add(result);

        // PEP matches increase risk level
        if (matches.Any())
        {
            RiskLevel = RiskLevel.High;
        }

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new PepScreeningCompleted(Id, screeningProvider, matches.Count, confidenceScore));
    }

    /// <summary>
    /// Adds an adverse media finding.
    /// </summary>
    /// <param name="source">The source of the finding.</param>
    /// <param name="description">The description of the finding.</param>
    /// <param name="severity">The severity level.</param>
    /// <param name="publishedDate">When the media was published.</param>
    public void AddAdverseMediaFinding(
        string source,
        string description,
        AdverseMediaSeverity severity,
        DateTime publishedDate)
    {
        var finding = new AdverseMediaFinding(
            source,
            description,
            severity,
            publishedDate,
            DateTime.UtcNow);

        AdverseMediaFindings.Add(finding);

        // High severity adverse media increases risk
        if (severity == AdverseMediaSeverity.High)
        {
            RiskLevel = RiskLevel.High;
        }

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new AdverseMediaFindingAdded(Id, source, severity));
    }

    /// <summary>
    /// Performs a compliance check.
    /// </summary>
    /// <param name="checkType">The type of compliance check.</param>
    /// <param name="result">The result of the check.</param>
    /// <param name="details">Additional details.</param>
    public void PerformComplianceCheck(
        string checkType,
        ComplianceResult result,
        string? details = null)
    {
        var check = new ComplianceCheck(
            checkType,
            result,
            details,
            DateTime.UtcNow);

        ComplianceChecks.Add(check);

        // Failed compliance checks block approval
        if (result == ComplianceResult.Failed)
        {
            Status = KycAmlStatus.Rejected;
        }

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new ComplianceCheckPerformed(Id, checkType, result));
    }

    /// <summary>
    /// Approves the KYC/AML profile.
    /// </summary>
    /// <param name="approvedBy">The person/entity who approved.</param>
    /// <param name="validityPeriod">The validity period in months.</param>
    public void Approve(string approvedBy, int validityPeriod = 12)
    {
        if (Status != KycAmlStatus.InProgress)
            throw new InvalidOperationException("Profile must be in progress to approve");

        // Check if all required compliance checks passed
        var requiredChecks = new[] { "ID Verification", "Address Verification", "Sanctions Screening" };
        var passedChecks = ComplianceChecks
            .Where(c => requiredChecks.Contains(c.CheckType) && c.Result == ComplianceResult.Passed)
            .Select(c => c.CheckType)
            .ToHashSet();

        if (!requiredChecks.All(check => passedChecks.Contains(check)))
        {
            throw new InvalidOperationException("All required compliance checks must pass before approval");
        }

        Status = KycAmlStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ExpiresAt = ApprovedAt.Value.AddMonths(validityPeriod);
        LastUpdatedAt = ApprovedAt;
        IncrementVersion();

        AddEvent(new KycAmlProfileApproved(Id, approvedBy, ExpiresAt.Value));
    }

    /// <summary>
    /// Rejects the KYC/AML profile.
    /// </summary>
    /// <param name="reason">The reason for rejection.</param>
    /// <param name="rejectedBy">The person/entity who rejected.</param>
    public void Reject(string reason, string rejectedBy)
    {
        if (Status == KycAmlStatus.Rejected)
            return;

        Status = KycAmlStatus.Rejected;
        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new KycAmlProfileRejected(Id, reason, rejectedBy, LastUpdatedAt.Value));
    }

    /// <summary>
    /// Checks if the profile is currently valid.
    /// </summary>
    /// <returns>True if approved and not expired.</returns>
    public bool IsValid()
    {
        return Status == KycAmlStatus.Approved &&
               (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    /// Checks if the profile requires renewal.
    /// </summary>
    /// <returns>True if renewal is needed.</returns>
    public bool RequiresRenewal()
    {
        return Status == KycAmlStatus.Approved &&
               ExpiresAt.HasValue &&
               (ExpiresAt.Value - DateTime.UtcNow) < TimeSpan.FromDays(30);
    }

    #endregion
}

/// <summary>
/// Represents the status of a KYC/AML profile.
/// </summary>
internal enum KycAmlStatus
{
    Pending,
    InProgress,
    Approved,
    Rejected,
    Expired
}

/// <summary>
/// Represents the risk level assessment.
/// </summary>
internal enum RiskLevel
{
    Unknown,
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Represents the result of a compliance check.
/// </summary>
internal enum ComplianceResult
{
    Passed,
    Failed,
    Pending,
    NotApplicable
}

/// <summary>
/// Represents the severity of adverse media.
/// </summary>
internal enum AdverseMediaSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Represents a risk assessment.
/// </summary>
internal class RiskAssessment
{
    public string AssessmentType { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public int Score { get; set; }
    public List<string> Factors { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public DateTime AssessedAt { get; set; }

    public RiskAssessment(
        string assessmentType,
        RiskLevel riskLevel,
        int score,
        List<string> factors,
        List<string> recommendations,
        DateTime assessedAt)
    {
        AssessmentType = assessmentType;
        RiskLevel = riskLevel;
        Score = score;
        Factors = factors;
        Recommendations = recommendations;
        AssessedAt = assessedAt;
    }
}

/// <summary>
/// Represents a compliance check.
/// </summary>
internal class ComplianceCheck
{
    public string CheckType { get; set; }
    public ComplianceResult Result { get; set; }
    public string? Details { get; set; }
    public DateTime PerformedAt { get; set; }

    public ComplianceCheck(
        string checkType,
        ComplianceResult result,
        string? details,
        DateTime performedAt)
    {
        CheckType = checkType;
        Result = result;
        Details = details;
        PerformedAt = performedAt;
    }
}

/// <summary>
/// Represents a sanctions screening result.
/// </summary>
internal class SanctionsScreeningResult
{
    public string ScreeningProvider { get; set; }
    public List<SanctionsMatch> Matches { get; set; } = new();
    public int ConfidenceScore { get; set; }
    public DateTime ScreenedAt { get; set; }

    public SanctionsScreeningResult(
        string screeningProvider,
        List<SanctionsMatch> matches,
        int confidenceScore,
        DateTime screenedAt)
    {
        ScreeningProvider = screeningProvider;
        Matches = matches;
        ConfidenceScore = confidenceScore;
        ScreenedAt = screenedAt;
    }
}

/// <summary>
/// Represents a sanctions match.
/// </summary>
internal class SanctionsMatch
{
    public string EntityName { get; set; }
    public string ListName { get; set; }
    public int ConfidenceScore { get; set; }
    public Dictionary<string, object> MatchDetails { get; set; } = new();

    public SanctionsMatch(
        string entityName,
        string listName,
        int confidenceScore)
    {
        EntityName = entityName;
        ListName = listName;
        ConfidenceScore = confidenceScore;
    }
}

/// <summary>
/// Represents a PEP screening result.
/// </summary>
internal class PepScreeningResult
{
    public string ScreeningProvider { get; set; }
    public List<PepMatch> Matches { get; set; } = new();
    public int ConfidenceScore { get; set; }
    public DateTime ScreenedAt { get; set; }

    public PepScreeningResult(
        string screeningProvider,
        List<PepMatch> matches,
        int confidenceScore,
        DateTime screenedAt)
    {
        ScreeningProvider = screeningProvider;
        Matches = matches;
        ConfidenceScore = confidenceScore;
        ScreenedAt = screenedAt;
    }
}

/// <summary>
/// Represents a PEP match.
/// </summary>
internal class PepMatch
{
    public string PersonName { get; set; }
    public string Position { get; set; }
    public string Country { get; set; }
    public int ConfidenceScore { get; set; }
    public Dictionary<string, object> MatchDetails { get; set; } = new();

    public PepMatch(
        string personName,
        string position,
        string country,
        int confidenceScore)
    {
        PersonName = personName;
        Position = position;
        Country = country;
        ConfidenceScore = confidenceScore;
    }
}

/// <summary>
/// Represents an adverse media finding.
/// </summary>
internal class AdverseMediaFinding
{
    public string Source { get; set; }
    public string Description { get; set; }
    public AdverseMediaSeverity Severity { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime DiscoveredAt { get; set; }

    public AdverseMediaFinding(
        string source,
        string description,
        AdverseMediaSeverity severity,
        DateTime publishedDate,
        DateTime discoveredAt)
    {
        Source = source;
        Description = description;
        Severity = severity;
        PublishedDate = publishedDate;
        DiscoveredAt = discoveredAt;
    }
}
