using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents the result of a DID dereferencing operation as defined by W3C DID 1.1 specification.
/// </summary>
public class DidDereferencingResult
{
    /// <summary>
    /// The DID URL that was dereferenced
    /// </summary>
    [JsonPropertyName("dereferencingMetadata")]
    public DidDereferencingMetadata DereferencingMetadata { get; set; } = new();
    
    /// <summary>
    /// The content that was dereferenced
    /// </summary>
    [JsonPropertyName("contentStream")]
    public DidDereferencingContent? ContentStream { get; set; }
    
    /// <summary>
    /// Metadata about the content
    /// </summary>
    [JsonPropertyName("contentMetadata")]
    public DidContentMetadata ContentMetadata { get; set; } = new();
    
    /// <summary>
    /// Indicates whether the dereferencing was successful
    /// </summary>
    public bool IsSuccessful => DereferencingMetadata.Error == null;
    
    /// <summary>
    /// Creates a successful dereferencing result
    /// </summary>
    /// <param name="contentStream">The dereferenced content</param>
    /// <param name="dereferencingMetadata">Optional dereferencing metadata</param>
    /// <param name="contentMetadata">Optional content metadata</param>
    /// <returns>A successful DidDereferencingResult</returns>
    public static DidDereferencingResult Success(DidDereferencingContent contentStream,
        DidDereferencingMetadata? dereferencingMetadata = null,
        DidContentMetadata? contentMetadata = null)
    {
        return new DidDereferencingResult
        {
            ContentStream = contentStream,
            DereferencingMetadata = dereferencingMetadata ?? new DidDereferencingMetadata(),
            ContentMetadata = contentMetadata ?? new DidContentMetadata()
        };
    }
    
    /// <summary>
    /// Creates a failed dereferencing result
    /// </summary>
    /// <param name="error">The error that occurred</param>
    /// <param name="errorMessage">Optional error message</param>
    /// <returns>A failed DidDereferencingResult</returns>
    public static DidDereferencingResult Failed(string error, string? errorMessage = null)
    {
        return new DidDereferencingResult
        {
            DereferencingMetadata = new DidDereferencingMetadata
            {
                Error = error,
                ErrorMessage = errorMessage
            }
        };
    }
}

/// <summary>
/// Metadata about the DID dereferencing process
/// </summary>
public class DidDereferencingMetadata
{
    /// <summary>
    /// The content type of the dereferenced content
    /// </summary>
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    
    /// <summary>
    /// The error that occurred during dereferencing, if any
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }
    
    /// <summary>
    /// Additional error message
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// The dereferencing duration in milliseconds
    /// </summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
    
    /// <summary>
    /// The DID method used for dereferencing
    /// </summary>
    [JsonPropertyName("method")]
    public string? Method { get; set; }
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// The content that was dereferenced
/// </summary>
public class DidDereferencingContent
{
    /// <summary>
    /// The content as a DID Document
    /// </summary>
    [JsonPropertyName("didDocument")]
    public DidDocument? DidDocument { get; set; }
    
    /// <summary>
    /// The content as a verification method
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    public VerificationMethod? VerificationMethod { get; set; }
    
    /// <summary>
    /// The content as a service endpoint
    /// </summary>
    [JsonPropertyName("service")]
    public ServiceEndpoint? Service { get; set; }
    
    /// <summary>
    /// The content as raw data
    /// </summary>
    [JsonPropertyName("data")]
    public byte[]? Data { get; set; }
    
    /// <summary>
    /// The content as a string
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// Metadata about the dereferenced content
/// </summary>
public class DidContentMetadata
{
    /// <summary>
    /// The content type of the dereferenced content
    /// </summary>
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    
    /// <summary>
    /// The size of the content in bytes
    /// </summary>
    [JsonPropertyName("contentLength")]
    public long? ContentLength { get; set; }
    
    /// <summary>
    /// The last modified timestamp of the content
    /// </summary>
    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }
    
    /// <summary>
    /// The ETag of the content
    /// </summary>
    [JsonPropertyName("etag")]
    public string? Etag { get; set; }
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}
