using Mamey.FWID.Identities.Application.AML.Models;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Interface for AML risk assessment service.
/// </summary>
public interface IRiskAssessmentService
{
    /// <summary>
    /// Calculates risk profile for an identity.
    /// </summary>
    Task<RiskProfile> CalculateRiskProfileAsync(
        RiskAssessmentRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets risk profile for an identity.
    /// </summary>
    Task<RiskProfile?> GetRiskProfileAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates risk profile with new factor.
    /// </summary>
    Task<RiskProfile> UpdateRiskFactorAsync(
        Guid identityId,
        RiskFactor factor,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recalculates risk score.
    /// </summary>
    Task<RiskProfile> RecalculateRiskScoreAsync(
        Guid identityId,
        string? reason = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets identities requiring review.
    /// </summary>
    Task<IReadOnlyList<RiskProfile>> GetIdentitiesRequiringReviewAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets high-risk identities.
    /// </summary>
    Task<IReadOnlyList<RiskProfile>> GetHighRiskIdentitiesAsync(
        int limit = 100,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds review note to profile.
    /// </summary>
    Task AddReviewNoteAsync(
        Guid identityId,
        Guid reviewerId,
        string reviewerName,
        string note,
        string? action = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for risk assessment.
/// </summary>
public class RiskAssessmentRequest
{
    public Guid IdentityId { get; set; }
    public string Zone { get; set; } = null!;
    public string? Clan { get; set; }
    public ScreeningResult? SanctionsScreening { get; set; }
    public ScreeningResult? PEPScreening { get; set; }
    public int? TransactionCount30Days { get; set; }
    public decimal? TransactionVolume30Days { get; set; }
    public bool? HasInternationalTransactions { get; set; }
    public bool? HasHighRiskCountryConnections { get; set; }
    public double? BehavioralAnomalyScore { get; set; }
    public Dictionary<string, object>? AdditionalFactors { get; set; }
}
