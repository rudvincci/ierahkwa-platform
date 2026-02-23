using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Methods.Peer;

/// <summary>
/// Represents a peer key pair (Ed25519 or other).
/// </summary>
public class PeerKeyPair
{
    [JsonPropertyName("publicKey")]
    public byte[] PublicKey { get; set; }
    [JsonPropertyName("privateKey")]
    public byte[] PrivateKey { get; set; }
    
    
}