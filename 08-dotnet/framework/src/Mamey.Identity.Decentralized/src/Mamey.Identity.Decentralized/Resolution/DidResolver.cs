using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Resolution;

/// <summary>
/// Aggregates all DID methods and orchestrates resolution in compliance with the W3C DID Resolution spec.
/// </summary>
public class DidResolver : IDidResolver
{
    private readonly IDidMethodRegistry _methodRegistry;

    /// <summary>
    /// Constructs a DID Resolver using a method registry.
    /// </summary>
    public DidResolver(IDidMethodRegistry methodRegistry)
    {
        _methodRegistry = methodRegistry ?? throw new ArgumentNullException(nameof(methodRegistry));
    }

    /// <inheritdoc />
    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(did))
            throw new ArgumentNullException(nameof(did));

        var didObj = new Did(did);
        var method = _methodRegistry.Get(didObj.Method);

        if (method == null)
            throw new NotSupportedException($"No DID method registered for '{didObj.Method}'.");

        var doc = await method.ResolveAsync(did, cancellationToken);

        // Generate document and resolution metadata (could enrich as needed)
        var result = new DidResolutionResult
        {
            DidDocument = doc,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object>
            {
                { "resolver", GetType().Name },
                { "method", didObj.Method }
                // implement more
            }
        };
        return result;
    }

    /// <inheritdoc />
    public bool SupportsMethod(string didMethod) => _methodRegistry.Get(didMethod) is { } method;
}
