using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.CMS.Core.Domain.Repositories;

internal interface IContentRepository
{
    Task<Content?> GetAsync(ContentId id, CancellationToken cancellationToken = default);
    Task AddAsync(Content content, CancellationToken cancellationToken = default);
    Task UpdateAsync(Content content, CancellationToken cancellationToken = default);
    Task DeleteAsync(ContentId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(ContentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Content>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Content>> GetByTenantAsync(GovTenantId tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Content>> GetByStatusAsync(ContentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Content>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default);
}
