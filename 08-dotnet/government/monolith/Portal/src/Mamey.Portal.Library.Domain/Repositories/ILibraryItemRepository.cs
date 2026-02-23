using Mamey.Portal.Library.Domain.Entities;

namespace Mamey.Portal.Library.Domain.Repositories;

public interface ILibraryItemRepository
{
    Task<LibraryItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LibraryItem>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(LibraryItem item, CancellationToken ct = default);
}
