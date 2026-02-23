using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;

/// <summary>
/// Represents a peer key pair (Ed25519, RSA, or other).
/// </summary>
public class PeerKeyPair
{
    /// <summary>
    /// The public key bytes.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public byte[] PublicKey { get; set; }

    /// <summary>
    /// The private key bytes.
    /// </summary>
    [JsonPropertyName("privateKey")]
    public byte[] PrivateKey { get; set; }

    /// <summary>
    /// The key type (e.g., "Ed25519", "RSA", "Secp256k1").
    /// </summary>
    [JsonPropertyName("keyType")]
    public string KeyType { get; set; } = "Ed25519";

    /// <summary>
    /// The key size in bits (for RSA keys).
    /// </summary>
    [JsonPropertyName("keySize")]
    public int KeySize { get; set; } = 256;

    /// <summary>
    /// Additional metadata for the key pair.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}