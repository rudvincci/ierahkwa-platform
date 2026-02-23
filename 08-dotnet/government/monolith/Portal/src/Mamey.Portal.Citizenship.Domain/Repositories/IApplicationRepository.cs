using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.ValueObjects;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IApplicationRepository
{
    Task<CitizenshipApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken ct = default);
    Task SaveAsync(CitizenshipApplication application, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
