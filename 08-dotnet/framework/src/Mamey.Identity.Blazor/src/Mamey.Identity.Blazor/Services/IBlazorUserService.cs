using Mamey.Identity.Core;

namespace Mamey.Identity.Blazor.Services;

/// <summary>
/// Service for managing user information in Blazor WebAssembly.
/// </summary>
public interface IBlazorUserService
{
    /// <summary>
    /// Gets the current user information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current user if authenticated.</returns>
    Task<AuthenticatedUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores user information in localStorage.
    /// </summary>
    /// <param name="user">The user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user information was stored successfully.</returns>
    Task<bool> StoreUserAsync(AuthenticatedUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user information from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user information if found.</returns>
    Task<AuthenticatedUser?> GetStoredUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes user information from localStorage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user information was removed successfully.</returns>
    Task<bool> RemoveUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the current user's information.
    /// </summary>
    /// <param name="user">The updated user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user information was updated successfully.</returns>
    Task<bool> UpdateUserAsync(AuthenticatedUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes user information from the server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed user information.</returns>
    Task<AuthenticatedUser?> RefreshUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user information is stored locally.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if user information is stored locally.</returns>
    Task<bool> HasStoredUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored user information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all user information was cleared successfully.</returns>
    Task<bool> ClearUserDataAsync(CancellationToken cancellationToken = default);
}


































