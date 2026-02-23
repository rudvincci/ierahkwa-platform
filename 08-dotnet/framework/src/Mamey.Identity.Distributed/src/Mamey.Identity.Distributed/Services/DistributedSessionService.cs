using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Identity.Core;
using Mamey.Identity.Distributed.Configuration;
using Mamey.Identity.Redis.Services;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for managing distributed sessions across microservices.
/// </summary>
public class DistributedSessionService : IDistributedSessionService
{
    private readonly ILogger<DistributedSessionService> _logger;
    private readonly DistributedIdentityOptions _options;
    private readonly IRedisTokenCache _tokenCache;

    public DistributedSessionService(
        ILogger<DistributedSessionService> logger,
        IOptions<DistributedIdentityOptions> options,
        IRedisTokenCache tokenCache)
    {
        _logger = logger;
        _options = options.Value;
        _tokenCache = tokenCache;
    }

    public async Task<string> CreateSessionAsync(AuthenticatedUser user, string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessionId = Guid.NewGuid().ToString();
            var sessionData = new SessionData
            {
                SessionId = sessionId,
                UserId = user.UserId,
                ServiceId = serviceId,
                User = user,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_options.SessionTimeoutMinutes),
                LastAccessedAt = DateTime.UtcNow
            };

            var sessionJson = JsonSerializer.Serialize(sessionData);
            var cacheKey = GetSessionCacheKey(sessionId);
            
            await _tokenCache.SetCachedTokenAsync(cacheKey, sessionJson);
            
            // Also store user sessions mapping
            var userSessionsKey = GetUserSessionsCacheKey(user.UserId);
            var existingSessions = await GetUserSessionsAsync(user.UserId, cancellationToken);
            var updatedSessions = existingSessions.Append(sessionId).ToList();
            await _tokenCache.SetCachedTokenAsync(userSessionsKey, JsonSerializer.Serialize(updatedSessions));

            _logger.LogDebug("Created distributed session {SessionId} for user {UserId} on service {ServiceId}", 
                sessionId, user.UserId, serviceId);
            
            return sessionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating distributed session for user {UserId} on service {ServiceId}", 
                user.UserId, serviceId);
            throw;
        }
    }

    public async Task<AuthenticatedUser?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetSessionCacheKey(sessionId);
            var sessionJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            
            if (string.IsNullOrEmpty(sessionJson))
            {
                _logger.LogDebug("Session {SessionId} not found in cache", sessionId);
                return null;
            }

            var sessionData = JsonSerializer.Deserialize<SessionData>(sessionJson);
            if (sessionData == null)
            {
                _logger.LogWarning("Failed to deserialize session data for session {SessionId}", sessionId);
                return null;
            }

            // Check if session is expired
            if (sessionData.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogDebug("Session {SessionId} has expired", sessionId);
                await RevokeSessionAsync(sessionId, cancellationToken);
                return null;
            }

            // Update last accessed time
            sessionData.LastAccessedAt = DateTime.UtcNow;
            var updatedSessionJson = JsonSerializer.Serialize(sessionData);
            await _tokenCache.SetCachedTokenAsync(cacheKey, updatedSessionJson);

            return sessionData.User;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session {SessionId}", sessionId);
            return null;
        }
    }

    public async Task<bool> UpdateSessionAsync(string sessionId, AuthenticatedUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetSessionCacheKey(sessionId);
            var sessionJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            
            if (string.IsNullOrEmpty(sessionJson))
            {
                _logger.LogWarning("Session {SessionId} not found for update", sessionId);
                return false;
            }

            var sessionData = JsonSerializer.Deserialize<SessionData>(sessionJson);
            if (sessionData == null)
            {
                _logger.LogWarning("Failed to deserialize session data for update of session {SessionId}", sessionId);
                return false;
            }

            sessionData.User = user;
            sessionData.LastAccessedAt = DateTime.UtcNow;
            
            var updatedSessionJson = JsonSerializer.Serialize(sessionData);
            await _tokenCache.SetCachedTokenAsync(cacheKey, updatedSessionJson);

            _logger.LogDebug("Updated session {SessionId}", sessionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session {SessionId}", sessionId);
            return false;
        }
    }

    public async Task<bool> ExtendSessionAsync(string sessionId, int extensionMinutes, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetSessionCacheKey(sessionId);
            var sessionJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            
            if (string.IsNullOrEmpty(sessionJson))
            {
                _logger.LogWarning("Session {SessionId} not found for extension", sessionId);
                return false;
            }

            var sessionData = JsonSerializer.Deserialize<SessionData>(sessionJson);
            if (sessionData == null)
            {
                _logger.LogWarning("Failed to deserialize session data for extension of session {SessionId}", sessionId);
                return false;
            }

            sessionData.ExpiresAt = sessionData.ExpiresAt.AddMinutes(extensionMinutes);
            sessionData.LastAccessedAt = DateTime.UtcNow;
            
            var updatedSessionJson = JsonSerializer.Serialize(sessionData);
            await _tokenCache.SetCachedTokenAsync(cacheKey, updatedSessionJson);

            _logger.LogDebug("Extended session {SessionId} by {ExtensionMinutes} minutes", sessionId, extensionMinutes);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending session {SessionId}", sessionId);
            return false;
        }
    }

    public async Task<bool> RevokeSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetSessionCacheKey(sessionId);
            await _tokenCache.SetCachedTokenAsync(cacheKey, string.Empty); // Set to empty to revoke

            _logger.LogDebug("Revoked session {SessionId}", sessionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking session {SessionId}", sessionId);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userSessionsKey = GetUserSessionsCacheKey(userId);
            var sessionsJson = await _tokenCache.GetCachedTokenAsync(userSessionsKey);
            
            if (string.IsNullOrEmpty(sessionsJson))
            {
                return Enumerable.Empty<string>();
            }

            var sessions = JsonSerializer.Deserialize<List<string>>(sessionsJson);
            return sessions ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user sessions for user {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await GetUserSessionsAsync(userId, cancellationToken);
            
            foreach (var sessionId in sessions)
            {
                await RevokeSessionAsync(sessionId, cancellationToken);
            }

            // Clear user sessions mapping
            var userSessionsKey = GetUserSessionsCacheKey(userId);
            await _tokenCache.SetCachedTokenAsync(userSessionsKey, string.Empty);

            _logger.LogDebug("Revoked all sessions for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all sessions for user {UserId}", userId);
            return false;
        }
    }

    private string GetSessionCacheKey(string sessionId) => $"{_options.CacheKeyPrefix}session:{sessionId}";
    private string GetUserSessionsCacheKey(Guid userId) => $"{_options.CacheKeyPrefix}user_sessions:{userId}";

    private class SessionData
    {
        public string SessionId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string ServiceId { get; set; } = string.Empty;
        public AuthenticatedUser User { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
    }
}


































