using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling identity operations with logging.
/// </summary>
internal interface IIdentityService
{
    /// <summary>
    /// Creates a new identity.
    /// </summary>
    Task<Identity> CreateIdentityAsync(
        IdentityId identityId,
        Name name,
        PersonalDetails personalDetails,
        ContactInformation contactInformation,
        BiometricData biometricData,
        string? zone = null,
        Guid? clanRegistrarId = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an identity.
    /// </summary>
    Task RevokeIdentityAsync(
        IdentityId identityId,
        string? reason,
        Guid? revokedBy,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates biometric data for an identity.
    /// </summary>
    Task UpdateBiometricDataAsync(
        IdentityId identityId,
        BiometricData biometricData,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a biometric update transaction (used by handlers after they perform the update).
    /// </summary>
    Task LogBiometricUpdateAsync(
        IdentityId identityId,
        string biometricType,
        bool isEnrollment,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the blockchain account address for an identity.
    /// </summary>
    Task<string?> GetBlockchainAccountAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries blockchain account creation for an identity that previously failed.
    /// </summary>
    Task<string?> RetryBlockchainAccountCreationAsync(
        IdentityId identityId,
        string currency = "USD",
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}

