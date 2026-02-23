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

internal class CompositeRoleRepository : IRoleRepository
{
    private readonly RoleMongoRepository _mongoRepo;
    private readonly RoleRedisRepository _redisRepo;
    private readonly RolePostgresRepository _postgresRepo;
    private readonly ILogger<CompositeRoleRepository> _logger;

    public CompositeRoleRepository(
        RoleMongoRepository mongoRepo,
        RoleRedisRepository redisRepo,
        RolePostgresRepository postgresRepo,
        ILogger<CompositeRoleRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(role, cancellationToken);
        Try(() => _redisRepo.AddAsync(role, cancellationToken));
        Try(() => _mongoRepo.AddAsync(role, cancellationToken));
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(role, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(role, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(role, cancellationToken));
    }

    public async Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public async Task<Role> GetAsync(RoleId id, CancellationToken cancellationToken = default)
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

    public Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
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

    public Task<IReadOnlyList<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<Role>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByPermissionIdAsync(permissionId, cancellationToken);

    public Task<IReadOnlyList<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(RoleStatus.Active, cancellationToken);

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.NameExistsAsync(name, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.NameExistsAsync(name, cancellationToken);
    }

    public Task<bool> NameExistsAsync(string name, RoleId excludeId, CancellationToken cancellationToken = default)
        => _postgresRepo.NameExistsAsync(name, excludeId, cancellationToken);

    public Task<bool> HasPermissionAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Method not implemented in PostgreSQL repository");

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(RoleStatus.Active, cancellationToken);

    // Additional methods required by IRoleRepository interface
    public Task<IReadOnlyList<Role>> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetBySubjectIdAsync(subjectId, cancellationToken);

    public Task<IReadOnlyList<Role>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => _postgresRepo.SearchAsync(searchTerm, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}
