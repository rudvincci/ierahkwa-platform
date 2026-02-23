using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class SessionRedisRepository : ISessionRepository
{
    private readonly ICache _cache;
    private readonly string _sessionPrefix;
    private readonly string _accessTokenPrefix;
    private readonly string _refreshTokenPrefix;
    private readonly string _userSessionsPrefix;
    private readonly string _expirySetKey;

    public SessionRedisRepository(ICache cache, RedisOptions options)
    {
        _cache = cache;
        var instance = options.Instance;
        _sessionPrefix = $"{instance}:session:";
        _accessTokenPrefix = $"{instance}:session:access:";
        _refreshTokenPrefix = $"{instance}:session:refresh:";
        _userSessionsPrefix = $"{instance}:session:user:";
        _expirySetKey = $"{instance}:session:expiry";
    }

    public async Task AddAsync(Session? session, CancellationToken cancellationToken = default)
    {
        var ttl = session.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) return;

        await _cache.SetAsync($"{_sessionPrefix}{session.Id.Value}", session, ttl);
        await _cache.SetAsync($"{_accessTokenPrefix}{session.AccessToken}", session.Id.Value, ttl);
        await _cache.SetAsync($"{_refreshTokenPrefix}{session.RefreshToken}", session.Id.Value, ttl);
        await _cache.AddToSetAsync($"{_userSessionsPrefix}{session.UserId.Value}", session.Id.Value);
    }

    public async Task UpdateAsync(Session? session, CancellationToken cancellationToken = default)
    {
        await AddAsync(session, cancellationToken);
    }

    public async Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        // Get session first to clean up related keys
        var session = await GetAsync(id, cancellationToken);
        if (session == null) return;

        await _cache.DeleteAsync<Session>($"{_sessionPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{_accessTokenPrefix}{session.AccessToken}");
        await _cache.DeleteAsync<Guid>($"{_refreshTokenPrefix}{session.RefreshToken}");
        await _cache.DeleteFromSetAsync<Guid>($"{_userSessionsPrefix}{session.UserId.Value}", id.Value);
    }

    public async Task<IReadOnlyList<Session?>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all keys
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<Session>($"{_sessionPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{_sessionPrefix}{id.Value}");

    public async Task<Session?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var sessionId = await _cache.GetAsync<Guid>($"{_accessTokenPrefix}{accessToken}");
        return sessionId != Guid.Empty ? await GetAsync(new SessionId(sessionId), cancellationToken) : null;
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var sessionId = await _cache.GetAsync<Guid>($"{_refreshTokenPrefix}{refreshToken}");
        return sessionId != Guid.Empty ? await GetAsync(new SessionId(sessionId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<Session?>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Get session IDs from the user's session set
        var sessionIds = await _cache.GetManyAsync<Guid>($"{_userSessionsPrefix}{userId.Value}");
        var sessions = new List<Session?>();
        
        foreach (var sessionId in sessionIds)
        {
            var session = await GetAsync(new SessionId(sessionId), cancellationToken);
            if (session != null)
            {
                sessions.Add(session);
            }
        }
        
        return sessions;
    }

    public async Task<IReadOnlyList<Session?>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<bool> AccessTokenExistsAsync(string accessToken, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{_accessTokenPrefix}{accessToken}");

    public async Task<bool> RefreshTokenExistsAsync(string refreshToken, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{_refreshTokenPrefix}{refreshToken}");

    public async Task<int> GetActiveSessionCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var sessions = await GetByUserIdAsync(userId, cancellationToken);
        return sessions.Count(s => s.Status == SessionStatus.Active);
    }

    public async Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        // Expired sessions are automatically removed by Redis TTL
    }

    public async Task DeleteSessionsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var sessions = await GetByUserIdAsync(userId, cancellationToken);
        foreach (var session in sessions)
        {
            await DeleteAsync(session.Id, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all session keys
        // In practice, this method should not be used for Redis
        return 0;
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return 0;
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return 0;
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return 0;
    }

    public async Task<int> CountRevokedAsync(CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return 0;
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // This is not efficient for Redis - would need to scan all sessions
        // In practice, this method should not be used for Redis
        return new List<Session?>();
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to ping Redis to check connectivity
            await _cache.SetAsync("ping", "pong", TimeSpan.FromSeconds(1));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
