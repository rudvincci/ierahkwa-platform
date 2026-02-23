using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Passports.Core.Domain.Repositories;

internal interface IPassportRepository
{
    Task<Passport?> GetAsync(PassportId id, CancellationToken cancellationToken = default);
    Task AddAsync(Passport passport, CancellationToken cancellationToken = default);
    Task UpdateAsync(Passport passport, CancellationToken cancellationToken = default);
    Task DeleteAsync(PassportId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(PassportId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Passport>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<Passport?> GetByPassportNumberAsync(PassportNumber passportNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Passport>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Passport>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
}
