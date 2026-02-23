namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Describes a verification method as defined in the W3C DID Core specification.
/// </summary>
public interface IDidVerificationMethod
{
    /// <summary>
    /// The identifier for this verification method (usually a DID URL fragment).
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The type of cryptographic key or method (e.g., "Ed25519VerificationKey2018").
    /// </summary>
    string Type { get; }

    /// <summary>
    /// The controller (DID or URI) responsible for this verification method.
    /// </summary>
    string Controller { get; }

    /// <summary>
    /// The public key material, which may be JWK, Multibase, or base58 encoded, depending on method.
    /// </summary>
    IReadOnlyDictionary<string, object> PublicKeyJwk { get; }

    /// <summary>
    /// Raw public key material, base58 or multibase encoded.
    /// </summary>
    string PublicKeyBase58 { get; }

    /// <summary>
    /// Raw public key material, multibase encoded.
    /// </summary>
    string PublicKeyMultibase { get; }

    /// <summary>
    /// Additional method-specific properties.
    /// </summary>
    IReadOnlyDictionary<string, object> AdditionalProperties { get; }

    /// <summary>
    /// Gets the public key bytes for verification.
    /// </summary>
    byte[] PublicKey { get; }

    /// <summary>
    /// Extracts the public key bytes from the preferred encoding (multibase, base58, jwk, etc.)
    /// </summary>
    /// <returns>Byte array of the public key.</returns>
    byte[] GetPublicKeyBytes();
}