using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling SMS confirmation operations.
/// </summary>
public interface ISmsConfirmationService
{
    /// <summary>
    /// Creates an SMS confirmation code.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="phoneNumber">The phone number to confirm.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The SMS confirmation identifier and code.</returns>
    Task<SmsConfirmationResult> CreateSmsConfirmationAsync(
        IdentityId identityId,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a phone number using a code.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="code">The confirmation code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ConfirmSmsAsync(
        IdentityId identityId,
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resends an SMS confirmation.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="phoneNumber">The phone number to resend confirmation to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ResendSmsConfirmationAsync(
        IdentityId identityId,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a phone number is confirmed for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the phone number is confirmed.</returns>
    Task<bool> IsPhoneConfirmedAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired SMS confirmations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of confirmations cleaned up.</returns>
    Task<int> CleanupExpiredSmsConfirmationsAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of an SMS confirmation creation.
/// </summary>
public class SmsConfirmationResult
{
    /// <summary>
    /// The SMS confirmation identifier.
    /// </summary>
    public SmsConfirmationId SmsConfirmationId { get; set; } = null!;

    /// <summary>
    /// The confirmation code.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// The expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

