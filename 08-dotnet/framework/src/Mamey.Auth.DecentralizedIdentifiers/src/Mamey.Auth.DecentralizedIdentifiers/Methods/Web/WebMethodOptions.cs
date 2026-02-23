namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Web;

/// <summary>
/// Options for did:web method operations.
/// </summary>
public class WebMethodOptions
{
    /// <summary>
    /// The domain hosting the DID Document (e.g., example.com).
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Path segments under .well-known or custom (e.g., ["user", "alice"] yields .../user/alice/did.json).
    /// </summary>
    public string[] PathSegments { get; set; }

    /// <summary>
    /// The private key for signing, if applicable.
    /// </summary>
    public byte[] PrivateKey { get; set; }

    /// <summary>
    /// The public key for verification.
    /// </summary>
    public byte[] PublicKey { get; set; }

    /// <summary>
    /// The controller DID or URI.
    /// </summary>
    public string Controller { get; set; }

    /// <summary>
    /// Optional service endpoints to include.
    /// </summary>
    public object[] ServiceEndpoints { get; set; }

    /// <summary>
    /// Any additional metadata.
    /// </summary>
    public object Metadata { get; set; }
    
    public string KeyType { get; set; } = "Ed25519"; // or "P-256", etc.
}