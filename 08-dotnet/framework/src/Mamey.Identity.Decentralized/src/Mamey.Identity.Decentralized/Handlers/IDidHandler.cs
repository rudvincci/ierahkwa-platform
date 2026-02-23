using System.Collections.Concurrent;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.VC;

namespace Mamey.Identity.Decentralized.Handlers;

/// <summary>
/// Unified contract for all DID operations (CRUD, resolution, verification).
/// </summary>
public interface IDidHandler
{
    Task<DidDocument> CreateAsync(CreateDidRequest request);
    Task<DidDocument> ResolveAsync(string did);
    Task<DidDocument> UpdateAsync(UpdateDidRequest request);
    Task DeactivateAsync(string did);
}
/// <summary>
/// Registry for mapping DID method to appropriate handler.
/// </summary>
public class DidHandlerRegistry
{
    private readonly ConcurrentDictionary<string, IDidHandler> _handlers = new();

    public void Register(string method, IDidHandler handler) =>
        _handlers[method.ToLowerInvariant()] = handler;

    public IDidHandler GetHandler(string did)
    {
        var method = did?.Split(':')[1]?.ToLowerInvariant();
        if (method == null || !_handlers.ContainsKey(method))
            throw new InvalidOperationException($"No handler registered for method: {method}");
        return _handlers[method];
    }
}