using Mamey.AI.Identity.Models;

namespace Mamey.AI.Identity.Services;

/// <summary>
/// Service for detecting fraud in identity-related operations.
/// </summary>
public interface IFraudDetectionService
{
    /// <summary>
    /// Analyzes an identity registration application for fraud indicators.
    /// </summary>
    Task<FraudScore> AnalyzeIdentityApplicationAsync(
        object applicationData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a credential issuance request for fraud indicators.
    /// </summary>
    Task<FraudScore> AnalyzeCredentialRequestAsync(
        object credentialRequestData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a DID creation request for fraud indicators.
    /// </summary>
    Task<FraudScore> AnalyzeDidCreationAsync(
        object didCreationData,
        CancellationToken cancellationToken = default);
}
