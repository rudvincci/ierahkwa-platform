using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IMfaConfigurationRepository
{
    Task AddAsync(MfaConfiguration entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(MfaConfiguration entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(MfaConfigurationId id, CancellationToken cancellationToken = default);
    Task<MfaConfiguration?> GetAsync(MfaConfigurationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(MfaConfigurationId id, CancellationToken cancellationToken = default);
    
    Task<MfaConfiguration?> GetByIdentityAndMethodAsync(IdentityId identityId, MfaMethod method, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaConfiguration>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaConfiguration>> FindAsync(Expression<Func<MfaConfiguration, bool>> predicate, CancellationToken cancellationToken = default);
}

