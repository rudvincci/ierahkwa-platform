namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Verifies a digital signature using the corresponding public key.
/// </summary>
public interface IKeyVerifier
{
    /// <summary>
    /// Verifies the provided signature for the given data and public key.
    /// </summary>
    /// <param name="keyType">Type of key.</param>
    /// <param name="publicKey">Public key bytes.</param>
    /// <param name="data">Original data.</param>
    /// <param name="signature">Signature bytes.</param>
    /// <returns>True if the signature is valid; otherwise, false.</returns>
    bool Verify(string keyType, byte[] publicKey, byte[] data, byte[] signature);
}