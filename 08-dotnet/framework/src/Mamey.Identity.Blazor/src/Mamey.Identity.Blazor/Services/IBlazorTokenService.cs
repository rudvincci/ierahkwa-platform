namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for managing tokens in Blazor WebAssembly.
/// </summary>
public interface IBlazorTokenService
{
    /// <summary>
    /// Stores an access token in localStorage.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token was stored successfully.</returns>
    Task<bool> StoreAccessTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the access token from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token if found.</returns>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the access token from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token was removed successfully.</returns>
    Task<bool> RemoveAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a refresh token in localStorage.
    /// </summary>
    /// <param name="token">The refresh token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token was stored successfully.</returns>
    Task<bool> StoreRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the refresh token from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refresh token if found.</returns>
    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the refresh token from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token was removed successfully.</returns>
    Task<bool> RemoveRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all tokens were cleared successfully.</returns>
    Task<bool> ClearAllTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an access token exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if an access token exists.</returns>
    Task<bool> HasAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a refresh token exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a refresh token exists.</returns>
    Task<bool> HasRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the current access token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateAccessTokenAsync(CancellationToken cancellationToken = default);
}


































