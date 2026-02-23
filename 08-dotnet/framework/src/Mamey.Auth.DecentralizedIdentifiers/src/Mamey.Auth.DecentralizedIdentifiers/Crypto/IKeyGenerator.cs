// using System.Security.Cryptography;

namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Provides methods for generating cryptographic key pairs.
/// </summary>
public interface IKeyGenerator
{
    /// <summary>
    /// Generates a new key pair of the specified type.
    /// </summary>
    /// <param name="keyType">The cryptographic key type (e.g., "Ed25519").</param>
    /// <returns>A tuple containing the private and public keys as byte arrays.</returns>
    Task<(byte[] privateKey, byte[] publicKey)> GenerateKeyPairAsync(string keyType);
}