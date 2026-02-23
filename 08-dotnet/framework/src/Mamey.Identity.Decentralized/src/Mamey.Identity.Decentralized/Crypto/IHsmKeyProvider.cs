namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Abstraction for secure key operations via HSM, KMS, or other protected modules.
/// </summary>
public interface IHsmKeyProvider
{
    /// <summary>
    /// Signs a digest or payload using a private key securely stored in the HSM/KMS.
    /// </summary>
    /// <param name="keyId">Reference/identifier for the key.</param>
    /// <param name="payload">The bytes to sign (typically already hashed).</param>
    /// <param name="algorithm">Signature algorithm ("ES256K", "EdDSA", etc).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw signature bytes.</returns>
    Task<byte[]> SignAsync(string keyId, byte[] payload, string algorithm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a signature using a public key stored or managed in HSM/KMS.
    /// </summary>
    /// <param name="keyId">Key reference/identifier.</param>
    /// <param name="payload">The original bytes (typically already hashed).</param>
    /// <param name="signature">The signature bytes to verify.</param>
    /// <param name="algorithm">Signature algorithm.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if valid; false otherwise.</returns>
    Task<bool> VerifyAsync(string keyId, byte[] payload, byte[] signature, string algorithm,
        CancellationToken cancellationToken = default);
}