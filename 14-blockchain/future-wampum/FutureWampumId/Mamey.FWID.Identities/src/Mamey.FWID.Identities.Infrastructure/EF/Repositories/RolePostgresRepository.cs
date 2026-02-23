using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for Role entities.
/// </summary>
internal class RolePostgresRepository : IRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public RolePostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Role entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Roles
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Role?> GetAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Name == name)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }

    public async Task<PagedResult<Role>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page;
        var resultsPerPage = query.ResultsPerPage;
        var skip = (page - 1) * resultsPerPage;

        var totalCount = await _dbContext.Roles.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / resultsPerPage);
        var items = await _dbContext.Roles
            .OrderBy(r => r.Name)
            .Skip(skip)
            .Take(resultsPerPage)
            .ToListAsync(cancellationToken);

        return PagedResult<Role>.Create(items, page, resultsPerPage, totalPages, totalCount);
    }

    public async Task<IReadOnlyList<Role>> FindAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }
}

