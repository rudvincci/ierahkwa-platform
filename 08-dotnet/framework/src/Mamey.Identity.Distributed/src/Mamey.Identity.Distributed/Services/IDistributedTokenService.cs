using Mamey.Identity.Core;
using System.Security.Claims;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for managing distributed tokens across microservices.
/// </summary>
public interface IDistributedTokenService
{
    /// <summary>
    /// Creates a distributed token for the specified user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="serviceId">The target service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The distributed token.</returns>
    Task<string> CreateDistributedTokenAsync(AuthenticatedUser user, string serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a distributed token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid, false otherwise.</returns>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes a distributed token.
    /// </summary>
    /// <param name="token">The token to refresh.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed token.</returns>
    Task<string> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a distributed token.
    /// </summary>
    /// <param name="token">The token to revoke.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token was revoked successfully.</returns>
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the claims from a distributed token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The claims from the token.</returns>
    Task<IEnumerable<Claim>> GetClaimsAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user from a distributed token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authenticated user.</returns>
    Task<AuthenticatedUser?> GetUserFromTokenAsync(string token, CancellationToken cancellationToken = default);
}


































