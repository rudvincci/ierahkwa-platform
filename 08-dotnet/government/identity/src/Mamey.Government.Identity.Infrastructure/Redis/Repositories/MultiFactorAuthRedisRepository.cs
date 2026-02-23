using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class MultiFactorAuthRedisRepository : IMultiFactorAuthRepository
{
    private readonly ICache _cache;
    private const string MultiFactorAuthPrefix = "multi-factor-auth:";
    private const string UserPrefix = "multi-factor-auth:user:";

    public MultiFactorAuthRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        // Cache with TTL based on typical session duration (e.g., 24 hours)
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{MultiFactorAuthPrefix}{multiFactorAuth.Id.Value}", multiFactorAuth, ttl);
        await _cache.SetAsync($"{UserPrefix}{multiFactorAuth.UserId.Value}", multiFactorAuth.Id.Value, ttl);
    }

    public async Task UpdateAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        await AddAsync(multiFactorAuth, cancellationToken);
    }

    public async Task DeleteAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var mfa = await GetAsync(id, cancellationToken);
        if (mfa == null) return;

        await _cache.DeleteAsync<MultiFactorAuth>($"{MultiFactorAuthPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{UserPrefix}{mfa.UserId.Value}");
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MultiFactorAuth>();
    }

    public async Task<MultiFactorAuth> GetAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<MultiFactorAuth>($"{MultiFactorAuthPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{MultiFactorAuthPrefix}{id.Value}");

    public async Task<MultiFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfaId = await _cache.GetAsync<Guid>($"{UserPrefix}{userId.Value}");
        return mfaId != Guid.Empty ? await GetAsync(new MultiFactorAuthId(mfaId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MultiFactorAuth>();
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MultiFactorAuth>();
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetActiveMultiFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<MultiFactorAuth>();
    }

    public async Task<bool> HasActiveMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfa = await GetByUserIdAsync(userId, cancellationToken);
        return mfa != null && mfa.Status == MultiFactorAuthStatus.Active;
    }

    public async Task<bool> HasMethodEnabledAsync(UserId userId, MfaMethod method, CancellationToken cancellationToken = default)
    {
        var mfa = await GetByUserIdAsync(userId, cancellationToken);
        return mfa != null && mfa.EnabledMethods.Contains(method);
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfa = await GetByUserIdAsync(userId, cancellationToken);
        if (mfa != null)
        {
            await DeleteAsync(mfa.Id, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }
}

