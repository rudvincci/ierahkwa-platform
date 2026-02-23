namespace Mamey.Auth.DecentralizedIdentifiers.Configuration;

/// <summary>
/// Root options class for configuring all decentralized identifier, resolver, crypto, and VC/VP settings.
/// Use this to bind from configuration (e.g., appsettings.json) and pass to AddDecentralizedIdentifiers.
/// </summary>
public class DecentralizedIdentifierOptions
{
    public const string SectionName = "dids";

    /// <summary>
    /// Enables the feature
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// Global library settings.
    /// </summary>
    public DidLibraryOptions Library { get; set; } = new();

    /// <summary>
    /// DID Resolution and dereferencing options.
    /// </summary>
    public DidResolutionOptions Resolution { get; set; } = new();

    /// <summary>
    /// Dereferencing for DID URLs and fragments.
    /// </summary>
    public DidDereferencingOptions Dereferencing { get; set; } = new();

    /// <summary>
    /// DID Resolver method options (method registry, priorities, network endpoints, etc.).
    /// </summary>
    public ResolverOptions Resolver { get; set; } = new();

    /// <summary>
    /// Key, crypto, and signature options (algorithms, allowed key types, providers, etc.).
    /// </summary>
    public CryptoProviderOptions Crypto { get; set; } = new();

    /// <summary>
    /// Key method support (mapping between DID methods and key types).
    /// </summary>
    public KeyMethodOptions KeyMethods { get; set; } = new();

    /// <summary>
    /// Default DID method to use (e.g., "ion", "web", "pkh", "ethr").
    /// </summary>
    public string DefaultDidMethod { get; set; } = "ion";

    /// <summary>
    /// Supported proof types (e.g., Ed25519Signature2020, EcdsaSecp256k1Signature2019, JsonWebSignature2020).
    /// </summary>
    public List<string> SupportedProofTypes { get; set; } = new() { "Ed25519Signature2020" };

    /// <summary>
    /// Credential status/revocation endpoint base URL (for StatusList2021).
    /// </summary>
    public string StatusListBaseUrl { get; set; }

    /// <summary>
    /// VC/VP format options (JSON-LD, JWT, JWS, etc).
    /// </summary>
    public string DefaultCredentialFormat { get; set; } = "JSON-LD";
    
    public BlockchainTrustRegistryOptions BlockchainTrustRegistry { get; set; } = new();
}