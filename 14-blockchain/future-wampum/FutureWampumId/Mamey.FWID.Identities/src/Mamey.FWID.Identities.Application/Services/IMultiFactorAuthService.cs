using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling multi-factor authentication operations.
/// </summary>
public interface IMultiFactorAuthService
{
    /// <summary>
    /// Sets up MFA for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method to set up.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The MFA setup result with secret key (for TOTP) or other setup data.</returns>
    Task<MfaSetupResult> SetupMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables MFA for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method to enable.</param>
    /// <param name="verificationCode">The verification code to confirm setup.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task EnableMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        string verificationCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables MFA for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method to disable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DisableMfaAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an MFA challenge for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method to use.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The MFA challenge result with code or challenge data.</returns>
    Task<MfaChallengeResult> CreateMfaChallengeAsync(
        IdentityId identityId,
        MfaMethod method,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies an MFA challenge.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method used.</param>
    /// <param name="code">The verification code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the challenge is verified successfully.</returns>
    Task<bool> VerifyMfaChallengeAsync(
        IdentityId identityId,
        MfaMethod method,
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates backup codes for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="count">The number of backup codes to generate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The backup codes (plain text, should be shown to user once).</returns>
    Task<List<string>> GenerateBackupCodesAsync(
        IdentityId identityId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a backup code.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="code">The backup code to verify.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the backup code is valid.</returns>
    Task<bool> VerifyBackupCodeAsync(
        IdentityId identityId,
        string code,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of an MFA setup operation.
/// </summary>
public class MfaSetupResult
{
    /// <summary>
    /// The MFA configuration identifier.
    /// </summary>
    public MfaConfigurationId MfaConfigurationId { get; set; } = null!;

    /// <summary>
    /// The secret key for TOTP (if applicable).
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// The QR code data URL for TOTP setup (if applicable).
    /// </summary>
    public string? QrCodeDataUrl { get; set; }

    /// <summary>
    /// The verification code sent via SMS or Email (if applicable).
    /// </summary>
    public string? VerificationCode { get; set; }
}

/// <summary>
/// Represents the result of an MFA challenge creation.
/// </summary>
public class MfaChallengeResult
{
    /// <summary>
    /// The challenge identifier.
    /// </summary>
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// The verification code sent via SMS or Email (if applicable).
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The expiration date and time of the challenge.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

