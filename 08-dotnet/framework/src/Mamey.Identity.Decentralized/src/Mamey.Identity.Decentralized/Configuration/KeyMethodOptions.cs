namespace Mamey.Identity.Decentralized.Configuration;

/// <summary>
/// Mapping and settings for DID key methods and custom method logic.
/// </summary>
public class KeyMethodOptions
{
    public Dictionary<string, string> MethodToKeyType { get; set; } = new()
    {
        { "ion", "secp256k1" },
        { "web", "Ed25519" },
        { "pkh", "secp256k1" }
    };
    public List<string> SupportedKeyMethods { get; set; } = new() { "ion", "web", "pkh", "key" };
}