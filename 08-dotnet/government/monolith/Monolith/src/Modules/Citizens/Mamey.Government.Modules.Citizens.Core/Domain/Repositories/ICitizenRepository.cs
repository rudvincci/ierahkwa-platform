using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Repositories;

internal interface ICitizenRepository
{
    Task<Citizen?> GetAsync(CitizenId id, CancellationToken cancellationToken = default);
    Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default);
    Task UpdateAsync(Citizen citizen, CancellationToken cancellationToken = default);
    Task DeleteAsync(CitizenId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CitizenId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Citizen>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<IReadOnlyList<Citizen>> GetByTenantAsync(GovTenantId tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Citizen>> GetByStatusAsync(CitizenshipStatus status, CancellationToken cancellationToken = default);
}
