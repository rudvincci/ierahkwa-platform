using Mamey.Portal.Cms.Domain.Entities;

namespace Mamey.Portal.Cms.Domain.Repositories;

public interface ICmsNewsRepository
{
    Task<CmsNewsItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CmsNewsItem>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(CmsNewsItem item, CancellationToken ct = default);
}
