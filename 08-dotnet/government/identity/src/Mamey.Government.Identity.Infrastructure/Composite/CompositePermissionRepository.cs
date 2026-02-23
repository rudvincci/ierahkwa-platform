using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositePermissionRepository : IPermissionRepository
{
    private readonly PermissionMongoRepository _mongoRepo;
    private readonly PermissionRedisRepository _redisRepo;
    private readonly PermissionPostgresRepository _postgresRepo;
    private readonly ILogger<CompositePermissionRepository> _logger;

    public CompositePermissionRepository(
        PermissionMongoRepository mongoRepo,
        PermissionRedisRepository redisRepo,
        PermissionPostgresRepository postgresRepo,
        ILogger<CompositePermissionRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(permission, cancellationToken);
        Try(() => _redisRepo.AddAsync(permission, cancellationToken));
        Try(() => _mongoRepo.AddAsync(permission, cancellationToken));
    }

    public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(permission, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(permission, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(permission, cancellationToken));
    }

    public async Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<Permission> GetAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetAsync(id, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetAsync(id, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByNameAsync(name, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByNameAsync(name, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByNameAsync(name, cancellationToken);
    }

    public Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByResourceAsync(resource, cancellationToken);

    public Task<IReadOnlyList<Permission>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByActionAsync(action, cancellationToken);

    public Task<IReadOnlyList<Permission>> GetByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<Permission>> GetActivePermissionsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(PermissionStatus.Active, cancellationToken);

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.NameExistsAsync(name, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.NameExistsAsync(name, cancellationToken);
    }

    public Task<bool> NameExistsAsync(string name, PermissionId excludeId, CancellationToken cancellationToken = default)
        => _postgresRepo.NameExistsAsync(name, excludeId, cancellationToken);

    public async Task<bool> ResourceActionExistsAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ResourceActionExistsAsync(resource, action, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.ResourceActionExistsAsync(resource, action, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByResourceAndActionAsync(resource, action, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByResourceAndActionAsync(resource, action, cancellationToken);
            if (fromMongo != null && fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByResourceAndActionAsync(resource, action, cancellationToken);
    }

    public Task<bool> ResourceActionExistsAsync(string resource, string action, PermissionId excludeId, CancellationToken cancellationToken = default)
        => _postgresRepo.ResourceActionExistsAsync(resource, action, excludeId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(PermissionStatus.Active, cancellationToken);

    public Task<int> CountByResourceAsync(string resource, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Method not implemented in PostgreSQL repository");


    public Task<IReadOnlyList<Permission>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByRoleIdAsync(roleId, cancellationToken);

    public Task<IReadOnlyList<Permission>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => _postgresRepo.SearchAsync(searchTerm, cancellationToken);

    public Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<PermissionId> ids, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByIdsAsync(ids, cancellationToken);

    public Task<IReadOnlyList<string>> GetDistinctResourcesAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetDistinctResourcesAsync(cancellationToken);

    public Task<IReadOnlyList<string>> GetDistinctActionsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetDistinctActionsAsync(cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}

