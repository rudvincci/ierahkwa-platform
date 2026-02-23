using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IStatusProgressionRepository
{
    Task<StatusProgressionApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<StatusProgressionApplication>> GetByStatusIdAsync(Guid citizenshipStatusId, CancellationToken ct = default);
    Task SaveAsync(StatusProgressionApplication application, CancellationToken ct = default);
}
