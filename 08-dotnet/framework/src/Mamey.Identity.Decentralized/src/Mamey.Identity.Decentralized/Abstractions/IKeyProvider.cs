namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Resolves cryptographic keys (public or private) for DIDs and verification methods.
/// Used for signature verification, signing, and key management across DID/VC flows.
/// </summary>
public interface IKeyProvider
{
    /// <summary>
    /// Returns the public key bytes for a given verification method ID or DID.
    /// </summary>
    /// <param name="verificationMethodIdOrDid">The verification method ID or DID.</param>
    /// <param name="cancellationToken">Cancellation token for async ops.</param>
    /// <returns>Public key bytes, or throws if not found.</returns>
    Task<byte[]> GetPublicKeyAsync(string verificationMethodIdOrDid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the private key bytes for a given key ID or DID (if authorized).
    /// </summary>
    /// <param name="keyIdOrDid">The private key ID or DID.</param>
    /// <param name="cancellationToken">Cancellation token for async ops.</param>
    /// <returns>Private key bytes, or throws if not found.</returns>
    Task<byte[]> GetPrivateKeyAsync(string keyIdOrDid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously resolves a key (public or private) by key ID.
    /// </summary>
    /// <param name="keyIdOrDid">Key ID or DID.</param>
    /// <returns>Raw key bytes, or null if not found.</returns>
    byte[] ResolveKey(string keyIdOrDid);
}