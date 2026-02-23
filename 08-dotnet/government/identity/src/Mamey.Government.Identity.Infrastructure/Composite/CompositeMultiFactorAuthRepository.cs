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

internal class CompositeMultiFactorAuthRepository : IMultiFactorAuthRepository
{
    private readonly MultiFactorAuthMongoRepository _mongoRepo;
    private readonly MultiFactorAuthRedisRepository _redisRepo;
    private readonly MultiFactorAuthPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeMultiFactorAuthRepository> _logger;

    public CompositeMultiFactorAuthRepository(
        MultiFactorAuthMongoRepository mongoRepo,
        MultiFactorAuthRedisRepository redisRepo,
        MultiFactorAuthPostgresRepository postgresRepo,
        ILogger<CompositeMultiFactorAuthRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(multiFactorAuth, cancellationToken);
        Try(() => _redisRepo.AddAsync(multiFactorAuth, cancellationToken));
        Try(() => _mongoRepo.AddAsync(multiFactorAuth, cancellationToken));
    }

    public async Task UpdateAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(multiFactorAuth, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(multiFactorAuth, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(multiFactorAuth, cancellationToken));
    }

    public async Task DeleteAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<MultiFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<MultiFactorAuth> GetAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
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

    public async Task<bool> ExistsAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<MultiFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByUserIdAsync(userId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByUserIdAsync(userId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByUserIdAsync(userId, cancellationToken);
    }

    public Task<IReadOnlyList<MultiFactorAuth>> GetByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<MultiFactorAuth>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByMethodAsync(method, cancellationToken);

    public Task<IReadOnlyList<MultiFactorAuth>> GetActiveMultiFactorAuthsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveMultiFactorAuthsAsync(cancellationToken);

    public async Task<bool> HasActiveMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasActiveMultiFactorAuthAsync(userId, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasActiveMultiFactorAuthAsync(userId, cancellationToken);
    }

    public async Task<bool> HasMethodEnabledAsync(UserId userId, MfaMethod method, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasMethodEnabledAsync(userId, method, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasMethodEnabledAsync(userId, method, cancellationToken);
    }

    public Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteByUserIdAsync(userId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}


