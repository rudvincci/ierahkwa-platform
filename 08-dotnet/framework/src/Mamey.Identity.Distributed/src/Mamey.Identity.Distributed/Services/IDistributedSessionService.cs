using Mamey.Identity.Core;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for managing distributed sessions across microservices.
/// </summary>
public interface IDistributedSessionService
{
    /// <summary>
    /// Creates a distributed session for the specified user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session ID.</returns>
    Task<string> CreateSessionAsync(AuthenticatedUser user, string serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a distributed session by ID.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authenticated user if session exists.</returns>
    Task<AuthenticatedUser?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a distributed session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="user">The updated user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the session was updated successfully.</returns>
    Task<bool> UpdateSessionAsync(string sessionId, AuthenticatedUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extends a distributed session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="extensionMinutes">Minutes to extend the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the session was extended successfully.</returns>
    Task<bool> ExtendSessionAsync(string sessionId, int extensionMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a distributed session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the session was revoked successfully.</returns>
    Task<bool> RevokeSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sessions for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active session IDs.</returns>
    Task<IEnumerable<string>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all sessions for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all sessions were revoked successfully.</returns>
    Task<bool> RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}



































