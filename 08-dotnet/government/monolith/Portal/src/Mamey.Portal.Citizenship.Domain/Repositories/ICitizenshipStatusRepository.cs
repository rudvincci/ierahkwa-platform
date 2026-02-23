using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface ICitizenshipStatusRepository
{
    Task<CitizenshipStatus?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CitizenshipStatus?> GetByEmailAsync(string tenantId, string email, CancellationToken ct = default);
    Task SaveAsync(CitizenshipStatus status, CancellationToken ct = default);
}
