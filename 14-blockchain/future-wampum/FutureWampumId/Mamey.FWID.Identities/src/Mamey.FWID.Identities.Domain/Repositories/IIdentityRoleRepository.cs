using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IIdentityRoleRepository
{
    Task AddAsync(IdentityRole entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IdentityRole?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IdentityRole?> GetByIdentityAndRoleAsync(IdentityId identityId, RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityRole>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityRole>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityRole>> FindAsync(Expression<Func<IdentityRole, bool>> predicate, CancellationToken cancellationToken = default);
}

