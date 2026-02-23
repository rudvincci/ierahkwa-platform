using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Domain.Repositories;

internal interface IDocumentRepository
{
    Task<Document?> GetAsync(DocumentId id, CancellationToken cancellationToken = default);
    Task AddAsync(Document document, CancellationToken cancellationToken = default);
    Task UpdateAsync(Document document, CancellationToken cancellationToken = default);
    Task DeleteAsync(DocumentId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<IReadOnlyList<Document>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByStorageKeyAsync(string storageKey, CancellationToken cancellationToken = default);
}
