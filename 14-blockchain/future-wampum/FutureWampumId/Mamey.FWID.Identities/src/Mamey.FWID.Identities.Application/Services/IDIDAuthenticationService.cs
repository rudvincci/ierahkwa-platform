using Mamey.FWID.Identities.Contracts.Commands;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for DID-based authentication.
/// Handles authentication of users via their Decentralized Identifiers.
/// 
/// TDD Reference: Lines 1594-1703 (Identity Service)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
public interface IDIDAuthenticationService
{
    /// <summary>
    /// Creates a DID authentication challenge.
    /// </summary>
    /// <param name="did">The DID requesting authentication.</param>
    /// <param name="domain">The domain/origin requesting authentication.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The challenge to be signed.</returns>
    Task<DIDAuthChallenge> CreateChallengeAsync(
        string did,
        string domain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user using their DID and signed challenge.
    /// </summary>
    /// <param name="command">The authentication command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authentication result.</returns>
    Task<DIDAuthenticationResult> AuthenticateAsync(
        AuthenticateWithDID command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves an identity by DID.
    /// </summary>
    /// <param name="did">The DID to resolve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identity ID if found.</returns>
    Task<Guid?> ResolveIdentityByDIDAsync(
        string did,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Links a DID to an existing identity.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="did">The DID to link.</param>
    /// <param name="verificationProof">Proof of DID ownership.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether linking was successful.</returns>
    Task<bool> LinkDIDToIdentityAsync(
        Guid identityId,
        string did,
        string verificationProof,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlinks a DID from an identity.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="did">The DID to unlink.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether unlinking was successful.</returns>
    Task<bool> UnlinkDIDFromIdentityAsync(
        Guid identityId,
        string did,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all DIDs linked to an identity.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of linked DIDs.</returns>
    Task<List<LinkedDIDInfo>> GetLinkedDIDsAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DID authentication challenge.
/// </summary>
public class DIDAuthChallenge
{
    /// <summary>
    /// Unique challenge ID.
    /// </summary>
    public string ChallengeId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The DID this challenge is for.
    /// </summary>
    public string DID { get; set; } = string.Empty;

    /// <summary>
    /// The nonce to sign.
    /// </summary>
    public string Nonce { get; set; } = string.Empty;

    /// <summary>
    /// The domain/origin.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// When the challenge was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the challenge expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Message to sign (formatted).
    /// </summary>
    public string MessageToSign => $"{DID}:{Nonce}:{Domain}:{CreatedAt:O}";
}

/// <summary>
/// Information about a linked DID.
/// </summary>
public class LinkedDIDInfo
{
    /// <summary>
    /// The DID.
    /// </summary>
    public string DID { get; set; } = string.Empty;

    /// <summary>
    /// The DID method (e.g., "futurewampum", "web", "key").
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// When the DID was linked.
    /// </summary>
    public DateTime LinkedAt { get; set; }

    /// <summary>
    /// Whether this is the primary DID.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Whether the DID is verified on blockchain.
    /// </summary>
    public bool IsBlockchainVerified { get; set; }

    /// <summary>
    /// Last authentication time using this DID.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
}
