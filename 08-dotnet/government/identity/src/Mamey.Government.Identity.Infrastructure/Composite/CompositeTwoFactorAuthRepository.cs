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

internal class CompositeTwoFactorAuthRepository : ITwoFactorAuthRepository
{
    private readonly TwoFactorAuthMongoRepository _mongoRepo;
    private readonly TwoFactorAuthRedisRepository _redisRepo;
    private readonly TwoFactorAuthPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeTwoFactorAuthRepository> _logger;

    public CompositeTwoFactorAuthRepository(
        TwoFactorAuthMongoRepository mongoRepo,
        TwoFactorAuthRedisRepository redisRepo,
        TwoFactorAuthPostgresRepository postgresRepo,
        ILogger<CompositeTwoFactorAuthRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(twoFactorAuth, cancellationToken);
        Try(() => _redisRepo.AddAsync(twoFactorAuth, cancellationToken));
        Try(() => _mongoRepo.AddAsync(twoFactorAuth, cancellationToken));
    }

    public async Task UpdateAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(twoFactorAuth, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(twoFactorAuth, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(twoFactorAuth, cancellationToken));
    }

    public async Task DeleteAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<TwoFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<TwoFactorAuth> GetAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
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

    public async Task<bool> ExistsAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<TwoFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
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

    public Task<IReadOnlyList<TwoFactorAuth>> GetByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<TwoFactorAuth>> GetActiveTwoFactorAuthsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveTwoFactorAuthsAsync(cancellationToken);

    public async Task<bool> HasActiveTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasActiveTwoFactorAuthAsync(userId, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasActiveTwoFactorAuthAsync(userId, cancellationToken);
    }

    public Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteByUserIdAsync(userId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}


