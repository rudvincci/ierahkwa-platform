using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for Government Identity blockchain operations.
/// Registers sovereign identities on the MameyNode blockchain via Mamey.Blockchain.Government.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 86-112 (I.1-I.3 Executive Summary - Sovereign Identity)
/// </summary>
internal interface IGovernmentIdentityService
{
    /// <summary>
    /// Registers a sovereign identity on the MameyNode blockchain.
    /// Creates a permanent, immutable identity record.
    /// </summary>
    /// <param name="identity">The identity to register.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blockchain identity record result.</returns>
    Task<GovernmentIdentityResult> RegisterIdentityAsync(
        Identity identity,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing identity on the blockchain.
    /// </summary>
    /// <param name="identity">The identity with updated information.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The update result.</returns>
    Task<GovernmentIdentityResult> UpdateIdentityAsync(
        Identity identity,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies an identity against the blockchain record.
    /// </summary>
    /// <param name="identity">The identity to verify.</param>
    /// <param name="verificationType">Type of verification (e.g., "full", "basic", "biometric").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The verification result.</returns>
    Task<GovernmentIdentityVerificationResult> VerifyIdentityAsync(
        Identity identity,
        string verificationType = "full",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identity record from the blockchain.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blockchain identity record, or null if not found.</returns>
    Task<GovernmentBlockchainIdentity?> GetIdentityFromBlockchainAsync(
        string identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records clan registrar authorization on the blockchain.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="clanRegistrarId">The clan registrar who authorized the identity.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authorization record result.</returns>
    Task<GovernmentIdentityResult> RecordClanRegistrarAuthorizationAsync(
        string identityId,
        Guid clanRegistrarId,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a government identity blockchain operation.
/// </summary>
internal record GovernmentIdentityResult(
    bool Success,
    string? BlockchainIdentityId,
    string? BlockchainAccount,
    string? ErrorMessage);

/// <summary>
/// Result of verifying an identity against the blockchain.
/// </summary>
internal record GovernmentIdentityVerificationResult(
    bool Verified,
    string? VerificationResult,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Identity record from the government blockchain.
/// </summary>
internal record GovernmentBlockchainIdentity(
    string IdentityId,
    string CitizenId,
    string FirstName,
    string LastName,
    string DateOfBirth,
    string Nationality,
    string Status,
    DateTime? RegistrationDate);
