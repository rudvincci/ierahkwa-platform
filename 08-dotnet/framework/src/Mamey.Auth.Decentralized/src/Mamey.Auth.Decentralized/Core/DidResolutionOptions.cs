using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Options for DID resolution operations
/// </summary>
public class DidResolutionOptions
{
    /// <summary>
    /// The content type to accept for the resolution result
    /// </summary>
    [JsonPropertyName("accept")]
    public string? Accept { get; set; }
    
    /// <summary>
    /// Whether to include the DID Document in the result
    /// </summary>
    [JsonPropertyName("includeDocument")]
    public bool IncludeDocument { get; set; } = true;
    
    /// <summary>
    /// Whether to include resolution metadata in the result
    /// </summary>
    [JsonPropertyName("includeMetadata")]
    public bool IncludeMetadata { get; set; } = true;
    
    /// <summary>
    /// The timeout for the resolution operation in milliseconds
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }
    
    /// <summary>
    /// Whether to validate the resolved DID Document
    /// </summary>
    [JsonPropertyName("validateDocument")]
    public bool ValidateDocument { get; set; } = true;
    
    /// <summary>
    /// Whether to use caching for the resolution
    /// </summary>
    [JsonPropertyName("useCache")]
    public bool UseCache { get; set; } = true;
    
    /// <summary>
    /// Additional properties for specific DID methods
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
    
    /// <summary>
    /// Creates default resolution options
    /// </summary>
    /// <returns>Default DidResolutionOptions</returns>
    public static DidResolutionOptions Default()
    {
        return new DidResolutionOptions();
    }
    
    /// <summary>
    /// Creates resolution options with a specific timeout
    /// </summary>
    /// <param name="timeoutMs">The timeout in milliseconds</param>
    /// <returns>DidResolutionOptions with the specified timeout</returns>
    public static DidResolutionOptions WithTimeout(int timeoutMs)
    {
        return new DidResolutionOptions
        {
            Timeout = timeoutMs
        };
    }
    
    /// <summary>
    /// Creates resolution options that skip validation
    /// </summary>
    /// <returns>DidResolutionOptions that skip validation</returns>
    public static DidResolutionOptions SkipValidation()
    {
        return new DidResolutionOptions
        {
            ValidateDocument = false
        };
    }
    
    /// <summary>
    /// Creates resolution options that disable caching
    /// </summary>
    /// <returns>DidResolutionOptions that disable caching</returns>
    public static DidResolutionOptions NoCache()
    {
        return new DidResolutionOptions
        {
            UseCache = false
        };
    }
}
