using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class CompositeSessionRepository : ISessionRepository
{
    private readonly SessionRedisRepository _redisRepo;
    private readonly SessionPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeSessionRepository> _logger;

    public CompositeSessionRepository(
        SessionRedisRepository redisRepo,
        SessionPostgresRepository postgresRepo,
        ILogger<CompositeSessionRepository> logger)
    {
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Session? session, CancellationToken cancellationToken = default)
    {
        // Write to Redis first (primary), then PostgreSQL (backup)
        try
        {
            await _redisRepo.AddAsync(session, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write session to Redis, continuing with PostgreSQL only");
        }
        await _postgresRepo.AddAsync(session, cancellationToken);
    }

    public async Task UpdateAsync(Session? session, CancellationToken cancellationToken = default)
    {
        // Update both stores
        try
        {
            await _redisRepo.UpdateAsync(session, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update session in Redis, continuing with PostgreSQL only");
        }
        await _postgresRepo.UpdateAsync(session, cancellationToken);
    }

    public async Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        // Delete from both stores
        try
        {
            await _redisRepo.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete session from Redis, continuing with PostgreSQL only");
        }
        await _postgresRepo.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for browsing (Redis is not efficient for this)
        return await _postgresRepo.BrowseAsync(cancellationToken);
    }

    public async Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            var session = await _redisRepo.GetAsync(id, cancellationToken);
            if (session != null) return session;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            if (await _redisRepo.ExistsAsync(id, cancellationToken))
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check existence in Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<Session?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            var session = await _redisRepo.GetByAccessTokenAsync(accessToken, cancellationToken);
            if (session != null) return session;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.GetByAccessTokenAsync(accessToken, cancellationToken);
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            var session = await _redisRepo.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (session != null) return session;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.GetByRefreshTokenAsync(refreshToken, cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            var sessions = await _redisRepo.GetByUserIdAsync(userId, cancellationToken);
            if (sessions.Any()) return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetByStatusAsync(status, cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetActiveSessionsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetExpiredSessionsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetSessionsByIpAddressAsync(ipAddress, cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetSessionsByUserAgentAsync(userAgent, cancellationToken);
    }

    public async Task<bool> AccessTokenExistsAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            if (await _redisRepo.AccessTokenExistsAsync(accessToken, cancellationToken))
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check access token in Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.AccessTokenExistsAsync(accessToken, cancellationToken);
    }

    public async Task<bool> RefreshTokenExistsAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            if (await _redisRepo.RefreshTokenExistsAsync(refreshToken, cancellationToken))
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check refresh token in Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.RefreshTokenExistsAsync(refreshToken, cancellationToken);
    }

    public async Task<int> GetActiveSessionCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Try Redis first, fallback to PostgreSQL
        try
        {
            return await _redisRepo.GetActiveSessionCountAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get session count from Redis, falling back to PostgreSQL");
        }
        return await _postgresRepo.GetActiveSessionCountAsync(userId, cancellationToken);
    }

    public async Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for cleanup operations
        await _postgresRepo.DeleteExpiredSessionsAsync(cancellationToken);
        
        // Redis sessions expire automatically via TTL
    }

    public async Task DeleteSessionsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Delete from both stores
        try
        {
            await _redisRepo.DeleteSessionsByUserIdAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete sessions from Redis, continuing with PostgreSQL only");
        }
        await _postgresRepo.DeleteSessionsByUserIdAsync(userId, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for counting
        return await _postgresRepo.CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for counting
        return await _postgresRepo.CountActiveAsync(cancellationToken);
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for counting
        return await _postgresRepo.CountExpiredAsync(cancellationToken);
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for counting
        return await _postgresRepo.CountExpiredAsync(before, cancellationToken);
    }

    public async Task<int> CountRevokedAsync(CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for counting
        return await _postgresRepo.CountRevokedAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Use PostgreSQL for complex queries
        return await _postgresRepo.GetExpiredSessionsAsync(before, cancellationToken);
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        // Check if PostgreSQL is available (primary concern for initialization)
        return await _postgresRepo.CanConnectAsync(cancellationToken);
    }
}
