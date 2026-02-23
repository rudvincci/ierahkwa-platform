using Mamey.Identity.Core;
using System.Security.Claims;

namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for handling Blazor WebAssembly authentication.
/// </summary>
public interface IBlazorAuthenticationService
{
    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authentication result.</returns>
    Task<BlazorAuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if logout was successful.</returns>
    Task<bool> LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the authentication token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed authentication result.</returns>
    Task<BlazorAuthenticationResult?> RefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current authentication state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current authentication state.</returns>
    Task<BlazorAuthenticationState> GetAuthenticationStateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user is authenticated.</returns>
    Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user's claims.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's claims.</returns>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the user has a specific claim.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <param name="claimValue">The claim value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the claim.</returns>
    Task<bool> HasClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    /// <param name="role">The role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the role.</returns>
    Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a Blazor authentication operation.
/// </summary>
public class BlazorAuthenticationResult
{
    /// <summary>
    /// Gets or sets whether the authentication was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if authentication failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user.
    /// </summary>
    public AuthenticatedUser? User { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Current Blazor authentication state.
/// </summary>
public class BlazorAuthenticationState
{
    /// <summary>
    /// Gets or sets whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user.
    /// </summary>
    public AuthenticatedUser? User { get; set; }

    /// <summary>
    /// Gets or sets the user's claims.
    /// </summary>
    public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets whether the token is expired.
    /// </summary>
    public bool IsTokenExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
}


































