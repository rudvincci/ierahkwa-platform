using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IIdentityRepository
{
    Task AddAsync(Identity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Identity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(IdentityId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Identity>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Identity?> GetAsync(IdentityId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(IdentityId id, CancellationToken cancellationToken = default);
    
    // Pagination
    Task<PagedResult<Identity>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<Identity>> BrowseAsync(IPagedQuery query, Expression<Func<Identity, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Expression-based queries
    Task<IReadOnlyList<Identity>> FindAsync(Expression<Func<Identity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Identity?> GetSingleOrDefaultAsync(Expression<Func<Identity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Identity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Identity, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Bulk operations
    Task AddRangeAsync(IEnumerable<Identity> entities, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Identity> entities, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<IdentityId> ids, CancellationToken cancellationToken = default);
    
    // Pessimistic locking (SELECT FOR UPDATE)
    Task<Identity?> GetWithLockAsync(IdentityId id, CancellationToken cancellationToken = default);
    Task<Identity?> GetSingleOrDefaultWithLockAsync(Expression<Func<Identity, bool>> predicate, CancellationToken cancellationToken = default);

    // Authentication-specific queries
    Task<Identity?> GetByDidAsync(string did, CancellationToken cancellationToken = default);
    Task<Identity?> GetByAzureUserIdAsync(string azureUserId, CancellationToken cancellationToken = default);
    Task<Identity?> GetByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default);
}
