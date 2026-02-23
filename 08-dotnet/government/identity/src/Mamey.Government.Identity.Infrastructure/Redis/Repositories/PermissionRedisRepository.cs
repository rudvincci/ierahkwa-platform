using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class PermissionRedisRepository : IPermissionRepository
{
    private readonly ICache _cache;
    private const string PermissionPrefix = "permission:";
    private const string NamePrefix = "permission:name:";
    private const string ResourceActionPrefix = "permission:resource-action:";

    public PermissionRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{PermissionPrefix}{permission.Id.Value}", permission, ttl);
        await _cache.SetAsync($"{NamePrefix}{permission.Name}", permission.Id.Value, ttl);
        await _cache.SetAsync($"{ResourceActionPrefix}{permission.Resource}:{permission.Action}", permission.Id.Value, ttl);
    }

    public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await AddAsync(permission, cancellationToken);
    }

    public async Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        var permission = await GetAsync(id, cancellationToken);
        if (permission == null) return;

        await _cache.DeleteAsync<Permission>($"{PermissionPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{NamePrefix}{permission.Name}");
        await _cache.DeleteAsync<Guid>($"{ResourceActionPrefix}{permission.Resource}:{permission.Action}");
    }

    public async Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<Permission> GetAsync(PermissionId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<Permission>($"{PermissionPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{PermissionPrefix}{id.Value}");

    public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var permissionId = await _cache.GetAsync<Guid>($"{NamePrefix}{name}");
        return permissionId != Guid.Empty ? await GetAsync(new PermissionId(permissionId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<Permission>> GetByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<IReadOnlyList<Permission>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissionId = await _cache.GetAsync<Guid>($"{ResourceActionPrefix}{resource}:{action}");
        if (permissionId == Guid.Empty) return new List<Permission>();
        
        var permission = await GetAsync(new PermissionId(permissionId), cancellationToken);
        return permission != null ? new List<Permission> { permission } : new List<Permission>();
    }

    public async Task<IReadOnlyList<Permission>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{NamePrefix}{name}");

    public async Task<bool> NameExistsAsync(string name, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        var permission = await GetByNameAsync(name, cancellationToken);
        return permission != null && permission.Id != excludeId;
    }

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{ResourceActionPrefix}{resource}:{action}");

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, PermissionId excludeId, CancellationToken cancellationToken = default)
    {
        var permissions = await GetByResourceAndActionAsync(resource, action, cancellationToken);
        return permissions != null && permissions.Any(p => p.Id != excludeId);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<IReadOnlyList<Permission>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return new List<Permission>();
    }

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<PermissionId> ids, CancellationToken cancellationToken = default)
    {
        var permissions = new List<Permission>();
        foreach (var id in ids)
        {
            var permission = await GetAsync(id, cancellationToken);
            if (permission != null)
            {
                permissions.Add(permission);
            }
        }
        return permissions;
    }

    public async Task<IReadOnlyList<string>> GetDistinctResourcesAsync(CancellationToken cancellationToken = default)
    {
        return new List<string>();
    }

    public async Task<IReadOnlyList<string>> GetDistinctActionsAsync(CancellationToken cancellationToken = default)
    {
        return new List<string>();
    }
}

