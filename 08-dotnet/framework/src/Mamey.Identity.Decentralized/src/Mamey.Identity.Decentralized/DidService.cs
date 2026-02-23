using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Exceptions;
using Mamey.Identity.Decentralized.Serialization;
using Mamey.Identity.Decentralized.Validation;

namespace Mamey.Identity.Decentralized;

/// <summary>
/// High-level service that abstracts all core W3C DID operations, including CRUD, resolution, dereferencing, validation, and serialization.
/// </summary>
public class DidService : IDidService
{
    private readonly IDidResolver _resolver;
    private readonly IDidDereferencer _dereferencer;
    private readonly IDidMethodRegistry _registry;

    /// <summary>
    /// Constructs a DidService using the provided resolver, dereferencer, and registry.
    /// </summary>
    public DidService(IDidResolver resolver, IDidDereferencer dereferencer, IDidMethodRegistry registry)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        _dereferencer = dereferencer ?? throw new ArgumentNullException(nameof(dereferencer));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Resolves a DID (any method) to its DID Document, or throws on error.
    /// </summary>
    public async Task<DidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _resolver.ResolveAsync(did, cancellationToken);
            if (result.DidDocument is DidDocument doc)
                return doc;
            throw new DidResolutionException($"DID Document could not be resolved: {did}");
        }
        catch (Exception ex)
        {
            throw new DidResolutionException($"Failed to resolve DID '{did}'.", ex);
        }
    }

    /// <summary>
    /// Dereferences a DID URL (fragment, service, key, etc.), returning the resolved content.
    /// </summary>
    public async Task<object> DereferenceAsync(string didUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dereferencer.DereferenceAsync(didUrl, cancellationToken);
            return result.Content;
        }
        catch (Exception ex)
        {
            throw new DidDereferencingException($"Failed to dereference DID URL '{didUrl}'.", ex);
        }
    }

    /// <summary>
    /// Creates a new DID Document using the specified method and options.
    /// </summary>
    public async Task<DidDocument> CreateAsync(string methodName, object options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var method = _registry.Get(methodName);
            if (method == null)
                throw new DidMethodNotSupportedException($"DID method '{methodName}' is not registered.");

            var doc = await method.CreateAsync(options, cancellationToken) as DidDocument;
            if (doc == null)
                throw new DidResolutionException($"DID method '{methodName}' did not return a DID Document.");
            return doc;
        }
        catch (Exception ex)
        {
            throw new DidResolutionException($"Failed to create DID with method '{methodName}'.", ex);
        }
    }

    /// <summary>
    /// Updates an existing DID Document.
    /// </summary>
    public async Task<DidDocument> UpdateAsync(string methodName, string did, object updateRequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var method = _registry.Get(methodName);
            if (method == null)
                throw new DidMethodNotSupportedException($"DID method '{methodName}' is not registered.");

            var doc = await method.UpdateAsync(did, updateRequest, cancellationToken) as DidDocument;
            if (doc == null)
                throw new DidResolutionException($"DID method '{methodName}' did not return a DID Document.");
            return doc;
        }
        catch (Exception ex)
        {
            throw new DidResolutionException($"Failed to update DID '{did}' with method '{methodName}'.", ex);
        }
    }

    /// <summary>
    /// Deactivates a DID (where supported).
    /// </summary>
    public async Task DeactivateAsync(string methodName, string did, CancellationToken cancellationToken = default)
    {
        try
        {
            var method = _registry.Get(methodName);
            if (method == null)
                throw new DidMethodNotSupportedException($"DID method '{methodName}' is not registered.");

            await method.DeactivateAsync(did, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new DidResolutionException($"Failed to deactivate DID '{did}' with method '{methodName}'.", ex);
        }
    }

    /// <summary>
    /// Validates a DID Document per W3C and internal best practices.
    /// Throws for errors, returns any non-fatal warnings.
    /// </summary>
    public IList<string> Validate(DidDocument doc)
    {
        try
        {
            return DidDocumentValidator.Validate(doc);
        }
        catch (Exception ex)
        {
            throw new InvalidDidFormatException($"DID Document failed validation: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Serializes a DID Document to JSON-LD.
    /// </summary>
    public string Serialize(DidDocument doc)
    {
        try
        {
            return DidDocumentSerializer.ToJson(doc);
        }
        catch (Exception ex)
        {
            throw new DidSerializationException("Failed to serialize DID Document.", ex);
        }
    }

    /// <summary>
    /// Deserializes a DID Document from JSON-LD.
    /// </summary>
    public DidDocument Deserialize(string json)
    {
        try
        {
            return DidDocumentDeserializer.FromJson(json);
        }
        catch (Exception ex)
        {
            throw new DidSerializationException("Failed to deserialize DID Document.", ex);
        }
    }

    /// <summary>
    /// Returns all registered DID methods.
    /// </summary>
    public IReadOnlyCollection<IDidMethod> GetRegisteredMethods()
    {
        return _registry.GetAll();
    }

    /// <summary>
    /// Registers a new DID method.
    /// </summary>
    public void RegisterMethod(IDidMethod method)
    {
        _registry.Register(method);
    }

    /// <summary>
    /// Returns true if a DID method is supported.
    /// </summary>
    public bool SupportsMethod(string methodName)
    {
        return _registry.Get(methodName) != null;
    }
}