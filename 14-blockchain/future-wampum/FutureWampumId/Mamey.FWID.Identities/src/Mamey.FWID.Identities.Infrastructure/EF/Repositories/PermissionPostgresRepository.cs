using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for Permission entities.
/// </summary>
internal class PermissionPostgresRepository : IPermissionRepository
{
    private readonly IdentityDbContext _dbContext;

    public PermissionPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permissions.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Permissions.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Permissions
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.Permissions.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Permission?> GetAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .Where(p => p.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .Where(p => p.Name == name)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<PagedResult<Permission>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page;
        var resultsPerPage = query.ResultsPerPage;
        var skip = (page - 1) * resultsPerPage;

        var totalCount = await _dbContext.Permissions.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / resultsPerPage);
        var items = await _dbContext.Permissions
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(resultsPerPage)
            .ToListAsync(cancellationToken);

        return PagedResult<Permission>.Create(items, page, resultsPerPage, totalPages, totalCount);
    }

    public async Task<IReadOnlyList<Permission>> FindAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }
}

