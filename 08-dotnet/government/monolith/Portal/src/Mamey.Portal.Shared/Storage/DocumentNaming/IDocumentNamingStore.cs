namespace Mamey.Portal.Shared.Storage.DocumentNaming;

public interface IDocumentNamingStore
{
    Task<DocumentNamingPattern> GetAsync(string tenantId, CancellationToken ct = default);
    Task SetAsync(string tenantId, DocumentNamingPattern pattern, CancellationToken ct = default);
}




