using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class PermissionPostgresRepository : EFRepository<Permission, PermissionId>, IPermissionRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public PermissionPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<Permission> GetAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .Where(p => p.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permissions.AddAsync(permission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _dbContext.Permissions.Update(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        var permission = await _dbContext.Permissions
            .SingleAsync(p => p.Id.Value == id.Value, cancellationToken);
        _dbContext.Permissions.Remove(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Permission-specific queries
    public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .Where(p => p.Name == name)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => p.Resource == resource)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<Permission>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => p.Action == action)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => p.Resource == resource && p.Action == action)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<Permission>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        // This would require querying roles and their permissions JSONB column
        // For now, return empty list - implement when needed
        return ImmutableList<Permission>.Empty;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Name == name && p.Id.Value != excludeId.Value, cancellationToken);
    }

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Resource == resource && p.Action == action, cancellationToken);
    }

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .AnyAsync(p => p.Resource == resource && p.Action == action && p.Id.Value != excludeId.Value, cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .CountAsync(p => p.Status == status, cancellationToken);
    }

    // Search and batch methods
    public async Task<IReadOnlyList<Permission>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || 
                       p.Resource.Contains(searchTerm) || p.Action.Contains(searchTerm))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<PermissionId> ids, CancellationToken cancellationToken = default)
    {
        var idValues = ids.Select(id => id.Value).ToList();
        var permissions = await _dbContext.Permissions
            .Where(p => idValues.Contains(p.Id.Value))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(permissions);
    }

    public async Task<IReadOnlyList<string>> GetDistinctResourcesAsync(CancellationToken cancellationToken = default)
    {
        var resources = await _dbContext.Permissions
            .Select(p => p.Resource)
            .Distinct()
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(resources);
    }

    public async Task<IReadOnlyList<string>> GetDistinctActionsAsync(CancellationToken cancellationToken = default)
    {
        var actions = await _dbContext.Permissions
            .Select(p => p.Action)
            .Distinct()
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(actions);
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
