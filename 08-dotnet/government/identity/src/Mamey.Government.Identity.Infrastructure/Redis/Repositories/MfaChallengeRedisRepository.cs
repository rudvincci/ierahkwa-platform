using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class MfaChallengeRedisRepository : IMfaChallengeRepository
{
    private readonly ICache _cache;
    private const string MfaChallengePrefix = "mfa-challenge:";
    private const string MultiFactorAuthPrefix = "mfa-challenge:mfa:";
    private const string ActivePrefix = "mfa-challenge:active:";

    public MfaChallengeRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        var ttl = mfaChallenge.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) return;

        await _cache.SetAsync($"{MfaChallengePrefix}{mfaChallenge.Id.Value}", mfaChallenge, ttl);
        
        // Index by MultiFactorAuthId
        await _cache.SetAsync($"{MultiFactorAuthPrefix}{mfaChallenge.MultiFactorAuthId.Value}", mfaChallenge.Id.Value, ttl);
        
        // Index active challenges
        if (mfaChallenge.Status == MfaChallengeStatus.Pending && !mfaChallenge.IsExpired())
        {
            await _cache.SetAsync($"{ActivePrefix}{mfaChallenge.MultiFactorAuthId.Value}", mfaChallenge.Id.Value, ttl);
        }
    }

    public async Task UpdateAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        await AddAsync(mfaChallenge, cancellationToken);
    }

    public async Task DeleteAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        var challenge = await GetAsync(id, cancellationToken);
        if (challenge == null) return;

        await _cache.DeleteAsync<MfaChallenge>($"{MfaChallengePrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{MultiFactorAuthPrefix}{challenge.MultiFactorAuthId.Value}");
        await _cache.DeleteAsync<Guid>($"{ActivePrefix}{challenge.MultiFactorAuthId.Value}");
    }

    public async Task<IReadOnlyList<MfaChallenge>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }

    public async Task<MfaChallenge> GetAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<MfaChallenge>($"{MfaChallengePrefix}{id.Value}");

    public async Task<bool> ExistsAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{MfaChallengePrefix}{id.Value}");

    public async Task<MfaChallenge> GetByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challengeId = await _cache.GetAsync<Guid>($"{MultiFactorAuthPrefix}{multiFactorAuthId.Value}");
        return challengeId != Guid.Empty ? await GetAsync(new MfaChallengeId(challengeId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetActiveChallengesAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }

    public async Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challengeId = await _cache.GetAsync<Guid>($"{ActivePrefix}{multiFactorAuthId.Value}");
        if (challengeId == Guid.Empty) return false;
        
        var challenge = await GetAsync(new MfaChallengeId(challengeId), cancellationToken);
        return challenge != null && challenge.Status == MfaChallengeStatus.Pending && !challenge.IsExpired();
    }

    public async Task DeleteExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        // Expired items are automatically removed by Redis TTL
    }

    public async Task DeleteChallengesByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challenge = await GetByMultiFactorAuthIdAsync(multiFactorAuthId, cancellationToken);
        if (challenge != null)
        {
            await DeleteAsync(challenge.Id, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<MfaChallenge> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // This requires joining with MultiFactorAuth, not efficient in Redis
        // Should be handled by query handler that combines repositories
        return null;
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => await GetExpiredChallengesAsync(cancellationToken);

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MfaChallenge>();
    }
}

