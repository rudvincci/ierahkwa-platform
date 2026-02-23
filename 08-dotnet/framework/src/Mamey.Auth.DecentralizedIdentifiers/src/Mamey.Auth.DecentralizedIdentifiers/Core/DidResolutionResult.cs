using System.Text.Json.Serialization;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Core
{
    /// <summary>
    /// Contains the result of a DID resolution operation, as defined by the W3C DID Resolution specification.
    /// </summary>
    public class DidResolutionResult
    {
        /// <summary>
        /// The resolved DID Document.
        /// </summary>
        [JsonPropertyName("didDocument")]
        public IDidDocument DidDocument { get; set; }

        /// <summary>
        /// DID Document metadata (created, updated, deactivated, etc.).
        /// </summary>
        [JsonPropertyName("didDocumentMetadata")]
        public IDictionary<string, object> DocumentMetadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Resolution operation metadata (method used, duration, contentType, etc.).
        /// </summary>
        [JsonPropertyName("didResolutionMetadata")]
        public IDictionary<string, object> ResolutionMetadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Constructs a new DidResolutionResult.
        /// </summary>
        public DidResolutionResult() { }

        /// <summary>
        /// Constructs a new DidResolutionResult with all properties.
        /// </summary>
        /// <param name="didDocument">The resolved DID Document.</param>
        /// <param name="documentMetadata">Metadata for the DID Document.</param>
        /// <param name="resolutionMetadata">Metadata for the resolution process.</param>
        public DidResolutionResult(
            IDidDocument didDocument,
            IDictionary<string, object> documentMetadata = null,
            IDictionary<string, object> resolutionMetadata = null)
        {
            DidDocument = didDocument;
            DocumentMetadata = documentMetadata ?? new Dictionary<string, object>();
            ResolutionMetadata = resolutionMetadata ?? new Dictionary<string, object>();
        }
    }
}
