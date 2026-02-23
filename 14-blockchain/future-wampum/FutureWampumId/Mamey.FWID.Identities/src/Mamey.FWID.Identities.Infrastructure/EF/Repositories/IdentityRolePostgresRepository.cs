using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for IdentityRole entities.
/// </summary>
internal class IdentityRolePostgresRepository : IIdentityRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public IdentityRolePostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(IdentityRole entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.IdentityRoles.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IdentityRole entity, CancellationToken cancellationToken = default)
    {
        _dbContext.IdentityRoles.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.IdentityRoles
            .SingleOrDefaultAsync(ir => ir.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.IdentityRoles.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IdentityRole?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityRoles
            .Where(ir => ir.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityRoles
            .AnyAsync(ir => ir.Id == id, cancellationToken);
    }

    public async Task<IdentityRole?> GetByIdentityAndRoleAsync(IdentityId identityId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IdentityRoles
            .Where(ir => ir.IdentityId == identityId && ir.RoleId == roleId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IdentityRole>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityRoles
            .Where(ir => ir.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

    public async Task<IReadOnlyList<IdentityRole>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityRoles
            .Where(ir => ir.RoleId == roleId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

    public async Task<IReadOnlyList<IdentityRole>> FindAsync(Expression<Func<IdentityRole, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var assignments = await _dbContext.IdentityRoles
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(assignments);
    }

}

