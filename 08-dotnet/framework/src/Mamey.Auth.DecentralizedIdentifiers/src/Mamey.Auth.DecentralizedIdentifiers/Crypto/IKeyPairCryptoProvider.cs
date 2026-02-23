namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Provides all operations (sign, verify, export, encode) for a specific key type.
/// </summary>
public interface IKeyPairCryptoProvider : IKeyGenerator, IKeySigner, IKeyVerifier
{
    /// <summary>
    /// The canonical key type (e.g., "Ed25519", "secp256k1", "RSA").
    /// </summary>
    string KeyType { get; }

    /// <summary>
    /// The W3C verification method type (e.g., "Ed25519VerificationKey2018").
    /// </summary>
    string VerificationMethodType { get; }

    /// <summary>
    /// Exports the public key as a JWK dictionary.
    /// </summary>
    IDictionary<string, object> ExportJwk(byte[] publicKey);

    /// <summary>
    /// Exports the public key as a base58-encoded string.
    /// </summary>
    string ExportBase58(byte[] publicKey);

    /// <summary>
    /// Exports the public key as a multibase-encoded string.
    /// </summary>
    string ExportMultibase(byte[] publicKey);

    /// <summary>
    /// Parses the public key from a JWK dictionary.
    /// </summary>
    byte[] ImportJwk(IDictionary<string, object> jwk);
}