using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mamey.Identity.Core;
using Mamey.Identity.Redis.Configuration;
using System.Text.Json;

namespace Mamey.Identity.Redis.Services;

public class RedisTokenCache : IRedisTokenCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisTokenCache> _logger;
    private readonly RedisTokenCacheOptions _options;

    public RedisTokenCache(
        IDistributedCache cache,
        ILogger<RedisTokenCache> logger,
        Microsoft.Extensions.Options.IOptions<RedisTokenCacheOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string?> GetTokenAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var token = await _cache.GetStringAsync(cacheKey, cancellationToken);
            
            if (token != null)
            {
                _logger.LogDebug("Token retrieved from cache for key: {Key}", key);
            }
            
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving token from cache for key: {Key}", key);
            return null;
        }
    }

    public async Task SetTokenAsync(string key, string token, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration
            };

            await _cache.SetStringAsync(cacheKey, token, options, cancellationToken);
            _logger.LogDebug("Token cached for key: {Key} with expiration: {Expiration}", key, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching token for key: {Key}", key);
        }
    }

    public async Task RemoveTokenAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetCacheKey(key);
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogDebug("Token removed from cache for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing token from cache for key: {Key}", key);
        }
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var blacklistKey = GetBlacklistKey(token);
            var blacklisted = await _cache.GetStringAsync(blacklistKey, cancellationToken);
            return blacklisted != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if token is blacklisted: {Token}", token);
            return false;
        }
    }

    public async Task BlacklistTokenAsync(string token, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var blacklistKey = GetBlacklistKey(token);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultBlacklistExpiration
            };

            await _cache.SetStringAsync(blacklistKey, "blacklisted", options, cancellationToken);
            _logger.LogDebug("Token blacklisted: {Token} with expiration: {Expiration}", token, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blacklisting token: {Token}", token);
        }
    }

    public async Task<AuthenticatedUser?> GetUserAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetUserCacheKey(key);
            var userJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
            
            if (userJson == null)
            {
                return null;
            }

            var user = JsonSerializer.Deserialize<AuthenticatedUser>(userJson);
            _logger.LogDebug("User retrieved from cache for key: {Key}", key);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user from cache for key: {Key}", key);
            return null;
        }
    }

    public async Task SetUserAsync(string key, AuthenticatedUser user, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetUserCacheKey(key);
            var userJson = JsonSerializer.Serialize(user);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration
            };

            await _cache.SetStringAsync(cacheKey, userJson, options, cancellationToken);
            _logger.LogDebug("User cached for key: {Key} with expiration: {Expiration}", key, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching user for key: {Key}", key);
        }
    }

    public async Task RemoveUserAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetUserCacheKey(key);
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogDebug("User removed from cache for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from cache for key: {Key}", key);
        }
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real scenario, you might want to use Redis SCAN to find and delete all keys with the prefix
            _logger.LogWarning("ClearAllAsync is not fully implemented. Consider using Redis SCAN for production use.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all cache entries");
        }
    }

    public async Task<string> GetCachedTokenAsync(string key)
    {
        var result = await GetTokenAsync(key);
        return result ?? string.Empty;
    }

    public async Task SetCachedTokenAsync(string key, string token)
    {
        await SetTokenAsync(key, token);
    }

    private string GetCacheKey(string key) => $"{_options.KeyPrefix}:token:{key}";
    private string GetBlacklistKey(string token) => $"{_options.KeyPrefix}:blacklist:{token}";
    private string GetUserCacheKey(string key) => $"{_options.KeyPrefix}:user:{key}";
}
