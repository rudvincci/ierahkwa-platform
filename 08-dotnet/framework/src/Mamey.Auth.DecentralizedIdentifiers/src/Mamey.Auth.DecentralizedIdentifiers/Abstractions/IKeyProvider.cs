namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

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

    /// <summary>
    /// Verifies a signature using the specified key.
    /// </summary>
    /// <param name="data">The data that was signed.</param>
    /// <param name="signature">The signature to verify.</param>
    /// <param name="keyId">The key ID to use for verification.</param>
    /// <param name="cancellationToken">Cancellation token for async ops.</param>
    /// <returns>True if the signature is valid, otherwise false.</returns>
    Task<bool> VerifyAsync(byte[] data, byte[] signature, string keyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs data using the specified key.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="keyId">The key ID to use for signing.</param>
    /// <param name="cancellationToken">Cancellation token for async ops.</param>
    /// <returns>The signature bytes.</returns>
    Task<byte[]> SignAsync(byte[] data, string keyId, CancellationToken cancellationToken = default);
}