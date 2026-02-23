using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Methods.DidWeb;

/// <summary>
/// Configuration options for DID Web method
/// </summary>
public class DidWebOptions
{
    /// <summary>
    /// The base URL for DID Web resolution
    /// </summary>
    [JsonPropertyName("baseUrl")]
    public string? BaseUrl { get; set; }
    
    /// <summary>
    /// The path to the DID Document (default: /.well-known/did.json)
    /// </summary>
    [JsonPropertyName("didDocumentPath")]
    public string DidDocumentPath { get; set; } = "/.well-known/did.json";
    
    /// <summary>
    /// Whether to use HTTPS for resolution
    /// </summary>
    [JsonPropertyName("useHttps")]
    public bool UseHttps { get; set; } = true;
    
    /// <summary>
    /// The port to use for resolution (if not default)
    /// </summary>
    [JsonPropertyName("port")]
    public int? Port { get; set; }
    
    /// <summary>
    /// The timeout for HTTP requests in milliseconds
    /// </summary>
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 30000;
    
    /// <summary>
    /// Whether to follow redirects
    /// </summary>
    [JsonPropertyName("followRedirects")]
    public bool FollowRedirects { get; set; } = true;
    
    /// <summary>
    /// The maximum number of redirects to follow
    /// </summary>
    [JsonPropertyName("maxRedirects")]
    public int MaxRedirects { get; set; } = 5;
    
    /// <summary>
    /// Additional headers to include in requests
    /// </summary>
    [JsonPropertyName("additionalHeaders")]
    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();
    
    /// <summary>
    /// Additional properties for custom configuration
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}
