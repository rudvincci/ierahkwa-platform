using Mamey.Portal.Cms.Domain.Entities;

namespace Mamey.Portal.Cms.Domain.Repositories;

public interface ICmsPageRepository
{
    Task<CmsPage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CmsPage?> GetBySlugAsync(string tenantId, string slug, CancellationToken ct = default);
    Task<IReadOnlyList<CmsPage>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(CmsPage page, CancellationToken ct = default);
}
