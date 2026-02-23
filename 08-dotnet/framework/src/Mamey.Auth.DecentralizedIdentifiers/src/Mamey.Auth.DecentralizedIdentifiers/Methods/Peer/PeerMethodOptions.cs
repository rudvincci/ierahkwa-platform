using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;

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

    /// <summary>
    /// The numalgo (numbering algorithm) to use for peer DID generation.
    /// 0: Base58-encoded DID document
    /// 1: Single key, self-certified (default)
    /// 2: Multiple keys, self-certified
    /// </summary>
    [JsonPropertyName("numalgo")]
    public int Numalgo { get; set; } = 1;

    /// <summary>
    /// Additional verification methods to include in the DID document.
    /// </summary>
    [JsonPropertyName("additionalVerificationMethods")]
    public List<PeerVerificationMethod> AdditionalVerificationMethods { get; set; } = new();

    /// <summary>
    /// Service endpoints to include in the DID document.
    /// </summary>
    [JsonPropertyName("services")]
    public List<PeerService> Services { get; set; } = new();

    /// <summary>
    /// Whether to include authentication methods in the DID document.
    /// </summary>
    [JsonPropertyName("includeAuthentication")]
    public bool IncludeAuthentication { get; set; } = true;

    /// <summary>
    /// Whether to include assertion methods in the DID document.
    /// </summary>
    [JsonPropertyName("includeAssertion")]
    public bool IncludeAssertion { get; set; } = false;

    /// <summary>
    /// Whether to include key agreement methods in the DID document.
    /// </summary>
    [JsonPropertyName("includeKeyAgreement")]
    public bool IncludeKeyAgreement { get; set; } = false;

    /// <summary>
    /// Whether to include capability invocation methods in the DID document.
    /// </summary>
    [JsonPropertyName("includeCapabilityInvocation")]
    public bool IncludeCapabilityInvocation { get; set; } = false;

    /// <summary>
    /// Whether to include capability delegation methods in the DID document.
    /// </summary>
    [JsonPropertyName("includeCapabilityDelegation")]
    public bool IncludeCapabilityDelegation { get; set; } = false;
}

/// <summary>
/// Represents a verification method for peer DIDs.
/// </summary>
public class PeerVerificationMethod
{
    /// <summary>
    /// The type of verification method (e.g., "Ed25519VerificationKey2018", "RsaVerificationKey2018").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Ed25519VerificationKey2018";

    /// <summary>
    /// The key pair for this verification method.
    /// </summary>
    [JsonPropertyName("keyPair")]
    public PeerKeyPair KeyPair { get; set; }

    /// <summary>
    /// The purpose of this verification method.
    /// </summary>
    [JsonPropertyName("purpose")]
    public List<string> Purpose { get; set; } = new() { "authentication" };
}

/// <summary>
/// Represents a service endpoint for peer DIDs.
/// </summary>
public class PeerService
{
    /// <summary>
    /// The service ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The service type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The service endpoint URL.
    /// </summary>
    [JsonPropertyName("serviceEndpoint")]
    public string ServiceEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Additional properties for the service.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}