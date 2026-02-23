using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling email confirmation operations.
/// </summary>
public interface IEmailConfirmationService
{
    /// <summary>
    /// Creates an email confirmation token.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="email">The email address to confirm.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email confirmation identifier and token.</returns>
    Task<EmailConfirmationResult> CreateEmailConfirmationAsync(
        IdentityId identityId,
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms an email address using a token.
    /// </summary>
    /// <param name="token">The confirmation token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ConfirmEmailAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resends an email confirmation.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="email">The email address to resend confirmation to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ResendEmailConfirmationAsync(
        IdentityId identityId,
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is confirmed for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the email is confirmed.</returns>
    Task<bool> IsEmailConfirmedAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired email confirmations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of confirmations cleaned up.</returns>
    Task<int> CleanupExpiredEmailConfirmationsAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of an email confirmation creation.
/// </summary>
public class EmailConfirmationResult
{
    /// <summary>
    /// The email confirmation identifier.
    /// </summary>
    public EmailConfirmationId EmailConfirmationId { get; set; } = null!;

    /// <summary>
    /// The confirmation token.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

