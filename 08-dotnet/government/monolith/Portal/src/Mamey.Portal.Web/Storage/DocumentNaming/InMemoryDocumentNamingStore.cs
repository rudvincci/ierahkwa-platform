using System.Collections.Concurrent;
using Mamey.Portal.Shared.Storage.DocumentNaming;

namespace Mamey.Portal.Web.Storage.DocumentNaming;

public sealed class InMemoryDocumentNamingStore : IDocumentNamingStore
{
    private readonly ConcurrentDictionary<string, DocumentNamingPattern> _store = new(StringComparer.Ordinal);

    public Task<DocumentNamingPattern> GetAsync(string tenantId, CancellationToken ct = default)
    {
        tenantId = string.IsNullOrWhiteSpace(tenantId) ? "default" : tenantId;
        var value = _store.GetOrAdd(tenantId, _ => DocumentNamingPattern.Default);
        return Task.FromResult(value);
    }

    public Task SetAsync(string tenantId, DocumentNamingPattern pattern, CancellationToken ct = default)
    {
        tenantId = string.IsNullOrWhiteSpace(tenantId) ? "default" : tenantId;
        _store[tenantId] = pattern;
        return Task.CompletedTask;
    }
}




