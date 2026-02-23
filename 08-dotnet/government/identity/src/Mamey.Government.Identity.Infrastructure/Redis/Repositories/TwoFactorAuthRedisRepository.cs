using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class TwoFactorAuthRedisRepository : ITwoFactorAuthRepository
{
    private readonly ICache _cache;
    private const string TwoFactorAuthPrefix = "two-factor-auth:";
    private const string UserPrefix = "two-factor-auth:user:";

    public TwoFactorAuthRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        // Cache with TTL based on typical session duration (e.g., 24 hours)
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{TwoFactorAuthPrefix}{twoFactorAuth.Id.Value}", twoFactorAuth, ttl);
        await _cache.SetAsync($"{UserPrefix}{twoFactorAuth.UserId.Value}", twoFactorAuth.Id.Value, ttl);
    }

    public async Task UpdateAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        await AddAsync(twoFactorAuth, cancellationToken);
    }

    public async Task DeleteAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var auth = await GetAsync(id, cancellationToken);
        if (auth == null) return;

        await _cache.DeleteAsync<TwoFactorAuth>($"{TwoFactorAuthPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{UserPrefix}{auth.UserId.Value}");
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<TwoFactorAuth>();
    }

    public async Task<TwoFactorAuth> GetAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<TwoFactorAuth>($"{TwoFactorAuthPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{TwoFactorAuthPrefix}{id.Value}");

    public async Task<TwoFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var authId = await _cache.GetAsync<Guid>($"{UserPrefix}{userId.Value}");
        return authId != Guid.Empty ? await GetAsync(new TwoFactorAuthId(authId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<TwoFactorAuth>();
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetActiveTwoFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<TwoFactorAuth>();
    }

    public async Task<bool> HasActiveTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var auth = await GetByUserIdAsync(userId, cancellationToken);
        return auth != null && auth.Status == TwoFactorAuthStatus.Active;
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var auth = await GetByUserIdAsync(userId, cancellationToken);
        if (auth != null)
        {
            await DeleteAsync(auth.Id, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }
}

