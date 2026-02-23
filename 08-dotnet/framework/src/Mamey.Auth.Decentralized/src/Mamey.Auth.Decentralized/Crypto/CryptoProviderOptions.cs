using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Configuration options for cryptographic providers
/// </summary>
public class CryptoProviderOptions
{
    /// <summary>
    /// The default cryptographic algorithm to use
    /// </summary>
    [JsonPropertyName("defaultAlgorithm")]
    public string DefaultAlgorithm { get; set; } = "Ed25519";
    
    /// <summary>
    /// The default curve for elliptic curve algorithms
    /// </summary>
    [JsonPropertyName("defaultCurve")]
    public string? DefaultCurve { get; set; }
    
    /// <summary>
    /// RSA-specific options
    /// </summary>
    [JsonPropertyName("rsa")]
    public RsaOptions Rsa { get; set; } = new();
    
    /// <summary>
    /// Ed25519-specific options
    /// </summary>
    [JsonPropertyName("ed25519")]
    public Ed25519Options Ed25519 { get; set; } = new();
    
    /// <summary>
    /// Secp256k1-specific options
    /// </summary>
    [JsonPropertyName("secp256k1")]
    public Secp256k1Options Secp256k1 { get; set; } = new();
    
    /// <summary>
    /// Additional properties for custom providers
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// RSA-specific configuration options
/// </summary>
public class RsaOptions
{
    /// <summary>
    /// The default key size in bits
    /// </summary>
    [JsonPropertyName("defaultKeySize")]
    public int DefaultKeySize { get; set; } = 2048;
    
    /// <summary>
    /// The minimum key size in bits
    /// </summary>
    [JsonPropertyName("minKeySize")]
    public int MinKeySize { get; set; } = 2048;
    
    /// <summary>
    /// The maximum key size in bits
    /// </summary>
    [JsonPropertyName("maxKeySize")]
    public int MaxKeySize { get; set; } = 4096;
    
    /// <summary>
    /// The hash algorithm to use for signing
    /// </summary>
    [JsonPropertyName("hashAlgorithm")]
    public string HashAlgorithm { get; set; } = "SHA256";
    
    /// <summary>
    /// The padding mode to use for signing
    /// </summary>
    [JsonPropertyName("paddingMode")]
    public string PaddingMode { get; set; } = "Pkcs1";
}

/// <summary>
/// Ed25519-specific configuration options
/// </summary>
public class Ed25519Options
{
    /// <summary>
    /// Whether to use deterministic key generation
    /// </summary>
    [JsonPropertyName("useDeterministicGeneration")]
    public bool UseDeterministicGeneration { get; set; } = true;
    
    /// <summary>
    /// The context string for domain separation
    /// </summary>
    [JsonPropertyName("context")]
    public string? Context { get; set; }
}

/// <summary>
/// Secp256k1-specific configuration options
/// </summary>
public class Secp256k1Options
{
    /// <summary>
    /// Whether to use compressed public keys
    /// </summary>
    [JsonPropertyName("useCompressedPublicKeys")]
    public bool UseCompressedPublicKeys { get; set; } = true;
    
    /// <summary>
    /// Whether to use deterministic key generation
    /// </summary>
    [JsonPropertyName("useDeterministicGeneration")]
    public bool UseDeterministicGeneration { get; set; } = true;
    
    /// <summary>
    /// The context string for domain separation
    /// </summary>
    [JsonPropertyName("context")]
    public string? Context { get; set; }
}
