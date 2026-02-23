namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Provides metadata for DID Documents and Resolution Results as required by the spec.
/// </summary>
public interface IMetadataProvider
{
    /// <summary>
    /// Gets metadata associated with a DID Document (e.g., created, updated, deactivated, versionId).
    /// </summary>
    IReadOnlyDictionary<string, object> GetDocumentMetadata();

    /// <summary>
    /// Gets metadata associated with a resolution or dereferencing operation (e.g., contentType, resolverVersion).
    /// </summary>
    IReadOnlyDictionary<string, object> GetResolutionMetadata();
}