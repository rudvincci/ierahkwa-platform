namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Represents cryptographic key material in a normalized, extensible form.
/// </summary>
public interface IKeyMaterial
{
    /// <summary>
    /// Gets the key type (e.g., "Ed25519", "secp256k1", "RSA").
    /// </summary>
    string KeyType { get; }

    /// <summary>
    /// Gets the key as a byte array.
    /// </summary>
    byte[] KeyBytes { get; }

    /// <summary>
    /// Gets the key in a standardized encoded format (e.g., base58, multibase, JWK).
    /// </summary>
    string EncodedKey { get; }

    /// <summary>
    /// Optionally gets a JWK (JSON Web Key) representation if available.
    /// </summary>
    string Jwk { get; }
}