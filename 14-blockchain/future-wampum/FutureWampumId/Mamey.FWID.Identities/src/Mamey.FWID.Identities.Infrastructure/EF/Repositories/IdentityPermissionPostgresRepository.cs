using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for IdentityPermission entities.
/// </summary>
internal class IdentityPermissionPostgresRepository : IIdentityPermissionRepository
{
    private readonly IdentityDbContext _dbContext;

    public IdentityPermissionPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(IdentityPermission entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.IdentityPermissions.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IdentityPermission entity, CancellationToken cancellationToken = default)
    {
        _dbContext.IdentityPermissions.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.IdentityPermissions
            .SingleOrDefaultAsync(ip => ip.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.IdentityPermissions.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IdentityPermission?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityPermissions
            .Where(ip => ip.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityPermissions
            .AnyAsync(ip => ip.Id == id, cancellationToken);
    }

    public async Task<IdentityPermission?> GetByIdentityAndPermissionAsync(IdentityId identityId, PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityPermissions
            .Where(ip => ip.IdentityId == identityId && ip.PermissionId == permissionId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IdentityPermission>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityPermissions
            .Where(ip => ip.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

    public async Task<IReadOnlyList<IdentityPermission>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityPermissions
            .Where(ip => ip.PermissionId == permissionId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

    public async Task<IReadOnlyList<IdentityPermission>> FindAsync(Expression<Func<IdentityPermission, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityPermissions
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

}

