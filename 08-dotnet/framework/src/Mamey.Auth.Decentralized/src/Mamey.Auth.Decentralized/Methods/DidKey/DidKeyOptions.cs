using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Methods.DidKey;

/// <summary>
/// Configuration options for DID Key method
/// </summary>
public class DidKeyOptions
{
    /// <summary>
    /// The default cryptographic algorithm to use for key generation
    /// </summary>
    [JsonPropertyName("defaultAlgorithm")]
    public string DefaultAlgorithm { get; set; } = "Ed25519";
    
    /// <summary>
    /// The default curve for elliptic curve algorithms
    /// </summary>
    [JsonPropertyName("defaultCurve")]
    public string? DefaultCurve { get; set; }
    
    /// <summary>
    /// Whether to include verification relationships in the generated DID Document
    /// </summary>
    [JsonPropertyName("includeVerificationRelationships")]
    public bool IncludeVerificationRelationships { get; set; } = true;
    
    /// <summary>
    /// Whether to include service endpoints in the generated DID Document
    /// </summary>
    [JsonPropertyName("includeServiceEndpoints")]
    public bool IncludeServiceEndpoints { get; set; } = false;
    
    /// <summary>
    /// The default service endpoints to include
    /// </summary>
    [JsonPropertyName("defaultServiceEndpoints")]
    public List<ServiceEndpointConfig> DefaultServiceEndpoints { get; set; } = new();
    
    /// <summary>
    /// Additional properties for custom configuration
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// Configuration for service endpoints
/// </summary>
public class ServiceEndpointConfig
{
    /// <summary>
    /// The service ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The service type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// The service endpoint URL
    /// </summary>
    [JsonPropertyName("serviceEndpoint")]
    public string ServiceEndpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional properties for the service endpoint
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}
