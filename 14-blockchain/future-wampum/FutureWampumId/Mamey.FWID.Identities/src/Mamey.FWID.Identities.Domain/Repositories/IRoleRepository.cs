using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IRoleRepository
{
    Task AddAsync(Role entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default);
    Task<Role?> GetAsync(RoleId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default);
    
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Role>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> FindAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);
}

