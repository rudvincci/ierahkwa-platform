using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;

internal interface ITravelIdentityRepository
{
    Task<TravelIdentity?> GetAsync(TravelIdentityId id, CancellationToken cancellationToken = default);
    Task AddAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TravelIdentityId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TravelIdentityId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TravelIdentity>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<TravelIdentity?> GetByTravelIdentityNumberAsync(TravelIdentityNumber travelIdentityNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TravelIdentity>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TravelIdentity>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default);
}
