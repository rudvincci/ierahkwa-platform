using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class RolePostgresRepository : EFRepository<Role, RoleId>, IRoleRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public RolePostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }

    public async Task<Role> GetAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(role, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Update(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles
            .SingleAsync(r => r.Id.Value == id.Value, cancellationToken);
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Role-specific queries
    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Name == name)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles
            .Where(r => r.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }

    public async Task<IReadOnlyList<Role>> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        // This would require querying subjects and their roles JSONB column
        // For now, return empty list - implement when needed
        return ImmutableList<Role>.Empty;
    }

    public async Task<IReadOnlyList<Role>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        // Query roles where the JSONB permissions array contains the permission ID
        var roles = await _dbContext.Roles
            .Where(r => r.Permissions.Any(p => p.Value == permissionId.Value))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, RoleId excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Name == name && r.Id.Value != excludeId.Value, cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .CountAsync(r => r.Status == status, cancellationToken);
    }

    // Search methods
    public async Task<IReadOnlyList<Role>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles
            .Where(r => r.Name.Contains(searchTerm) || r.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(roles);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
