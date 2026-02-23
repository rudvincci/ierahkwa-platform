using System.Security.Claims;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for validating tokens in distributed scenarios.
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateJwtTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a service token.
    /// </summary>
    /// <param name="token">The service token.</param>
    /// <param name="expectedServiceId">The expected service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateServiceTokenAsync(string token, string expectedServiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a distributed token.
    /// </summary>
    /// <param name="token">The distributed token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateDistributedTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the claims from a token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The claims from the token.</returns>
    Task<IEnumerable<Claim>?> GetClaimsAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates token expiration.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is not expired.</returns>
    Task<bool> IsTokenExpiredAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates token issuer.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="expectedIssuer">The expected issuer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the issuer is valid.</returns>
    Task<bool> ValidateIssuerAsync(string token, string expectedIssuer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates token audience.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="expectedAudience">The expected audience.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the audience is valid.</returns>
    Task<bool> ValidateAudienceAsync(string token, string expectedAudience, CancellationToken cancellationToken = default);
}


































