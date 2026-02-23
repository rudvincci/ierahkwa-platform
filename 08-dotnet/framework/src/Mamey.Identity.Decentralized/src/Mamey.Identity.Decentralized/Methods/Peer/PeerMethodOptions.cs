using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Methods.Peer;

/// <summary>
/// Options for creating a peer DID (deterministic, self-certifying).
/// </summary>
public class PeerMethodOptions
{
    /// <summary>
    /// Optional: Provide an existing key pair (if omitted, will generate).
    /// </summary>
    [JsonPropertyName("keyPair")]
    public PeerKeyPair KeyPair { get; set; }
}