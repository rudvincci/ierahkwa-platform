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

internal class CompositeMfaChallengeRepository : IMfaChallengeRepository
{
    private readonly MfaChallengeMongoRepository _mongoRepo;
    private readonly MfaChallengeRedisRepository _redisRepo;
    private readonly MfaChallengePostgresRepository _postgresRepo;
    private readonly ILogger<CompositeMfaChallengeRepository> _logger;

    public CompositeMfaChallengeRepository(
        MfaChallengeMongoRepository mongoRepo,
        MfaChallengeRedisRepository redisRepo,
        MfaChallengePostgresRepository postgresRepo,
        ILogger<CompositeMfaChallengeRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(mfaChallenge, cancellationToken);
        Try(() => _redisRepo.AddAsync(mfaChallenge, cancellationToken));
        Try(() => _mongoRepo.AddAsync(mfaChallenge, cancellationToken));
    }

    public async Task UpdateAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(mfaChallenge, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(mfaChallenge, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(mfaChallenge, cancellationToken));
    }

    public async Task DeleteAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<MfaChallenge>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<MfaChallenge> GetAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
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

    public async Task<bool> ExistsAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<MfaChallenge> GetByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);
    }

    public Task<IReadOnlyList<MfaChallenge>> GetByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<MfaChallenge>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByMethodAsync(method, cancellationToken);

    public Task<IReadOnlyList<MfaChallenge>> GetActiveChallengesAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveChallengesAsync(cancellationToken);

    public Task<IReadOnlyList<MfaChallenge>> GetExpiredChallengesAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredChallengesAsync(cancellationToken);

    public async Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasActiveChallengeAsync(multiFactorAuthId, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasActiveChallengeAsync(multiFactorAuthId, cancellationToken);
    }

    public Task DeleteExpiredChallengesAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteExpiredChallengesAsync(cancellationToken);

    public Task DeleteChallengesByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteChallengesByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    public Task<MfaChallenge> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveByUserIdAsync(userId, cancellationToken);

    public Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredAsync(cancellationToken);

    public Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredAsync(before, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}


