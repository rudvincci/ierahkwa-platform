using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Options for DID dereferencing operations
/// </summary>
public class DidDereferencingOptions
{
    /// <summary>
    /// The content type to accept for the dereferencing result
    /// </summary>
    [JsonPropertyName("accept")]
    public string? Accept { get; set; }
    
    /// <summary>
    /// The timeout for the dereferencing operation in milliseconds
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }
    
    /// <summary>
    /// Whether to use caching for the dereferencing
    /// </summary>
    [JsonPropertyName("useCache")]
    public bool UseCache { get; set; } = true;
    
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
    /// Additional properties for specific DID methods
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
    
    /// <summary>
    /// Creates default dereferencing options
    /// </summary>
    /// <returns>Default DidDereferencingOptions</returns>
    public static DidDereferencingOptions Default()
    {
        return new DidDereferencingOptions();
    }
    
    /// <summary>
    /// Creates dereferencing options with a specific timeout
    /// </summary>
    /// <param name="timeoutMs">The timeout in milliseconds</param>
    /// <returns>DidDereferencingOptions with the specified timeout</returns>
    public static DidDereferencingOptions WithTimeout(int timeoutMs)
    {
        return new DidDereferencingOptions
        {
            Timeout = timeoutMs
        };
    }
    
    /// <summary>
    /// Creates dereferencing options that disable caching
    /// </summary>
    /// <returns>DidDereferencingOptions that disable caching</returns>
    public static DidDereferencingOptions NoCache()
    {
        return new DidDereferencingOptions
        {
            UseCache = false
        };
    }
    
    /// <summary>
    /// Creates dereferencing options that disable redirects
    /// </summary>
    /// <returns>DidDereferencingOptions that disable redirects</returns>
    public static DidDereferencingOptions NoRedirects()
    {
        return new DidDereferencingOptions
        {
            FollowRedirects = false
        };
    }
}
