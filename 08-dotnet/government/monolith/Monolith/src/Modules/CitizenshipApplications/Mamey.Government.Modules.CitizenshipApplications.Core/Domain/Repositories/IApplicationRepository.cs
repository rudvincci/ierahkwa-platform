using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;

internal interface IApplicationRepository
{
    Task<CitizenshipApplication?> GetAsync(AppId id, CancellationToken cancellationToken = default);
    Task AddAsync(CitizenshipApplication application, CancellationToken cancellationToken = default);
    Task UpdateAsync(CitizenshipApplication application, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AppId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CitizenshipApplication>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<IList<CitizenshipApplication>> GetAllByApplicationEmail(string email, CancellationToken cancellationToken = default);
    Task<CitizenshipApplication?> GetByApplicationEmail(string email, CancellationToken cancellationToken = default);
    Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default);
    Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default);
}
