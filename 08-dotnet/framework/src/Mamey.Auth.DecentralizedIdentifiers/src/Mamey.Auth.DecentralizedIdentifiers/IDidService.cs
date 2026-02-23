using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers;

public interface IDidService
{
    /// <summary>
    /// Resolves a DID (any method) to its DID Document, or throws on error.
    /// </summary>
    Task<DidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default);
    /// <summary>
    /// Dereferences a DID URL (fragment, service, key, etc.), returning the resolved content.
    /// </summary>
    Task<object> DereferenceAsync(string didUrl, CancellationToken cancellationToken = default);
    /// <summary>
    /// Creates a new DID Document using the specified method and options.
    /// </summary>
    Task<DidDocument> CreateAsync(string methodName, object options,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing DID Document.
    /// </summary>
    Task<DidDocument> UpdateAsync(string methodName, string did, object updateRequest,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Deactivates a DID (where supported).
    /// </summary>
    Task DeactivateAsync(string methodName, string did, CancellationToken cancellationToken = default);
    /// <summary>
    /// Validates a DID Document per W3C and internal best practices.
    /// Throws for errors, returns any non-fatal warnings.
    /// </summary>
    IList<string> Validate(DidDocument doc);
    /// <summary>
    /// Serializes a DID Document to JSON-LD.
    /// </summary>
    string Serialize(DidDocument doc);
    /// <summary>
    /// Deserializes a DID Document from JSON-LD.
    /// </summary>
    DidDocument Deserialize(string json);
    /// <summary>
    /// Returns all registered DID methods.
    /// </summary>
    IReadOnlyCollection<IDidMethod> GetRegisteredMethods();
    /// <summary>
    /// Registers a new DID method.
    /// </summary>
    void RegisterMethod(IDidMethod method);
    
    /// <summary>
    /// Returns true if a DID method is supported.
    /// </summary>
    bool SupportsMethod(string methodName);
}