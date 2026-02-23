using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class RoleRedisRepository : IRoleRepository
{
    private readonly ICache _cache;
    private const string RolePrefix = "role:";
    private const string NamePrefix = "role:name:";

    public RoleRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{RolePrefix}{role.Id.Value}", role, ttl);
        await _cache.SetAsync($"{NamePrefix}{role.Name}", role.Id.Value, ttl);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        await AddAsync(role, cancellationToken);
    }

    public async Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        var role = await GetAsync(id, cancellationToken);
        if (role == null) return;

        await _cache.DeleteAsync<Role>($"{RolePrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{NamePrefix}{role.Name}");
    }

    public async Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return new List<Role>();
    }

    public async Task<Role> GetAsync(RoleId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<Role>($"{RolePrefix}{id.Value}");

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{RolePrefix}{id.Value}");

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var roleId = await _cache.GetAsync<Guid>($"{NamePrefix}{name}");
        return roleId != Guid.Empty ? await GetAsync(new RoleId(roleId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        return new List<Role>();
    }

    public async Task<IReadOnlyList<Role>> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        return new List<Role>();
    }

    public async Task<IReadOnlyList<Role>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
    {
        return new List<Role>();
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{NamePrefix}{name}");

    public async Task<bool> NameExistsAsync(string name, RoleId excludeId, CancellationToken cancellationToken = default)
    {
        var role = await GetByNameAsync(name, cancellationToken);
        return role != null && role.Id != excludeId;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<IReadOnlyList<Role>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return new List<Role>();
    }
}

