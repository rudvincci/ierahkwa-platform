using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IIdentityPermissionRepository
{
    Task AddAsync(IdentityPermission entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IdentityPermission?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IdentityPermission?> GetByIdentityAndPermissionAsync(IdentityId identityId, PermissionId permissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityPermission>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityPermission>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityPermission>> FindAsync(Expression<Func<IdentityPermission, bool>> predicate, CancellationToken cancellationToken = default);
}

