using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents the result of a DID resolution operation as defined by W3C DID 1.1 specification.
/// </summary>
public class DidResolutionResult
{
    /// <summary>
    /// The DID that was resolved
    /// </summary>
    [JsonPropertyName("did")]
    public string Did { get; set; } = string.Empty;
    
    /// <summary>
    /// The DID Document if resolution was successful
    /// </summary>
    [JsonPropertyName("didDocument")]
    public DidDocument? DidDocument { get; set; }
    
    /// <summary>
    /// Metadata about the resolution process
    /// </summary>
    [JsonPropertyName("didResolutionMetadata")]
    public DidResolutionMetadata ResolutionMetadata { get; set; } = new();
    
    /// <summary>
    /// Metadata about the DID Document
    /// </summary>
    [JsonPropertyName("didDocumentMetadata")]
    public DidDocumentMetadata DocumentMetadata { get; set; } = new();
    
    /// <summary>
    /// Indicates whether the resolution was successful
    /// </summary>
    public bool IsSuccessful => ResolutionMetadata.Error == null;
    
    /// <summary>
    /// Creates a successful resolution result
    /// </summary>
    /// <param name="did">The DID that was resolved</param>
    /// <param name="didDocument">The resolved DID Document</param>
    /// <param name="resolutionMetadata">Optional resolution metadata</param>
    /// <param name="documentMetadata">Optional document metadata</param>
    /// <returns>A successful DidResolutionResult</returns>
    public static DidResolutionResult Success(string did, DidDocument didDocument, 
        DidResolutionMetadata? resolutionMetadata = null, 
        DidDocumentMetadata? documentMetadata = null)
    {
        return new DidResolutionResult
        {
            Did = did,
            DidDocument = didDocument,
            ResolutionMetadata = resolutionMetadata ?? new DidResolutionMetadata(),
            DocumentMetadata = documentMetadata ?? new DidDocumentMetadata()
        };
    }
    
    /// <summary>
    /// Creates a failed resolution result
    /// </summary>
    /// <param name="did">The DID that failed to resolve</param>
    /// <param name="error">The error that occurred</param>
    /// <param name="errorMessage">Optional error message</param>
    /// <returns>A failed DidResolutionResult</returns>
    public static DidResolutionResult Failed(string did, string error, string? errorMessage = null)
    {
        return new DidResolutionResult
        {
            Did = did,
            ResolutionMetadata = new DidResolutionMetadata
            {
                Error = error,
                ErrorMessage = errorMessage
            }
        };
    }
    
    /// <summary>
    /// Creates a result for an invalid DID
    /// </summary>
    /// <param name="did">The invalid DID</param>
    /// <returns>A DidResolutionResult indicating invalid DID</returns>
    public static DidResolutionResult InvalidDid(string did)
    {
        return Failed(did, "invalidDid", "The DID format is invalid");
    }
    
    /// <summary>
    /// Creates a result for an unsupported DID method
    /// </summary>
    /// <param name="did">The DID with unsupported method</param>
    /// <returns>A DidResolutionResult indicating method not supported</returns>
    public static DidResolutionResult MethodNotSupported(string did)
    {
        return Failed(did, "methodNotSupported", "The DID method is not supported");
    }
    
    /// <summary>
    /// Creates a result for a DID that was not found
    /// </summary>
    /// <param name="did">The DID that was not found</param>
    /// <returns>A DidResolutionResult indicating not found</returns>
    public static DidResolutionResult NotFound(string did)
    {
        return Failed(did, "notFound", "The DID was not found");
    }
    
    /// <summary>
    /// Creates a result for a DID that was deactivated
    /// </summary>
    /// <param name="did">The deactivated DID</param>
    /// <returns>A DidResolutionResult indicating deactivated</returns>
    public static DidResolutionResult Deactivated(string did)
    {
        return Failed(did, "deactivated", "The DID has been deactivated");
    }
}

/// <summary>
/// Metadata about the DID resolution process
/// </summary>
public class DidResolutionMetadata
{
    /// <summary>
    /// The content type of the resolved DID Document
    /// </summary>
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    
    /// <summary>
    /// The error that occurred during resolution, if any
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }
    
    /// <summary>
    /// Additional error message
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// The resolution duration in milliseconds
    /// </summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
    
    /// <summary>
    /// The method used to resolve the DID
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
/// Metadata about the DID Document
/// </summary>
public class DidDocumentMetadata
{
    /// <summary>
    /// The creation timestamp of the DID Document
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }
    
    /// <summary>
    /// The last updated timestamp of the DID Document
    /// </summary>
    [JsonPropertyName("updated")]
    public DateTime? Updated { get; set; }
    
    /// <summary>
    /// The deactivation timestamp of the DID Document
    /// </summary>
    [JsonPropertyName("deactivated")]
    public DateTime? Deactivated { get; set; }
    
    /// <summary>
    /// The version of the DID Document
    /// </summary>
    [JsonPropertyName("versionId")]
    public string? VersionId { get; set; }
    
    /// <summary>
    /// The next update timestamp of the DID Document
    /// </summary>
    [JsonPropertyName("nextUpdate")]
    public DateTime? NextUpdate { get; set; }
    
    /// <summary>
    /// The next deactivation timestamp of the DID Document
    /// </summary>
    [JsonPropertyName("nextDeactivation")]
    public DateTime? NextDeactivation { get; set; }
    
    /// <summary>
    /// The equivalent ID of the DID Document
    /// </summary>
    [JsonPropertyName("equivalentId")]
    public string? EquivalentId { get; set; }
    
    /// <summary>
    /// The canonical ID of the DID Document
    /// </summary>
    [JsonPropertyName("canonicalId")]
    public string? CanonicalId { get; set; }
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}
