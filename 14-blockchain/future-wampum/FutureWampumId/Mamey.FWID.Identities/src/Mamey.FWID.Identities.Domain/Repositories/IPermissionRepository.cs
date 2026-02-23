using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IPermissionRepository
{
    Task AddAsync(Permission entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task<Permission?> GetAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default);
    
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Permission>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> FindAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default);
}

