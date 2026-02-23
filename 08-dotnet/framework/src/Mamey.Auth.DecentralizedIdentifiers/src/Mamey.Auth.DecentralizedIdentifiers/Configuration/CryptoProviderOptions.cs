namespace Mamey.Auth.DecentralizedIdentifiers.Configuration;

public class CryptoProviderOptions
{
    public List<string> AllowedKeyTypes { get; set; } = new() { "Ed25519", "secp256k1" };
    public string DefaultKeyType { get; set; } = "Ed25519";
    public bool AllowCustomProviders { get; set; } = false;
    public List<string> AllowedSignatureAlgorithms { get; set; } = new() { "EdDSA", "ES256K", "ES256", "RS256" };
    public string DefaultSignatureAlgorithm { get; set; } = "EdDSA";
}