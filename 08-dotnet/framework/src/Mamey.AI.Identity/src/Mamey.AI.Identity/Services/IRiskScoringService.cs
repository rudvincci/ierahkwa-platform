using Mamey.AI.Identity.Models;

namespace Mamey.AI.Identity.Services;

/// <summary>
/// Service for calculating risk scores for identity operations with explainability.
/// </summary>
public interface IRiskScoringService
{
    /// <summary>
    /// Calculates a comprehensive risk score for an identity operation.
    /// </summary>
    Task<RiskAssessment> CalculateRiskScoreAsync(
        object operationData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides explainability for a risk score, detailing contributing factors.
    /// </summary>
    Task<RiskExplanation> ExplainRiskScoreAsync(
        RiskAssessment riskAssessment,
        CancellationToken cancellationToken = default);
}
