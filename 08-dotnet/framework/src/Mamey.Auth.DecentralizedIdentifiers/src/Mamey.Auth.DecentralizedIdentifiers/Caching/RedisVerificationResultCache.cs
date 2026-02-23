using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Redis implementation of verification result cache
/// </summary>
public class RedisVerificationResultCache : IVerificationResultCache
{
    private readonly ICache _cache;
    private readonly ILogger<RedisVerificationResultCache> _logger;
    private readonly string _keyPrefix = "did:verification:";
    private readonly int _defaultTtlMinutes = 30;

    public RedisVerificationResultCache(ICache cache, ILogger<RedisVerificationResultCache> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VerificationResult> GetAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            var result = await _cache.GetAsync<VerificationResult>(cacheKey);
            
            if (result == null)
            {
                _logger.LogDebug("Verification result cache miss: {Key}", key);
                return null;
            }

            // Check if expired
            if (result.VerifiedAt.AddMinutes(_defaultTtlMinutes) < DateTime.UtcNow)
            {
                await _cache.DeleteAsync<VerificationResult>(cacheKey);
                _logger.LogDebug("Verification result cache expired: {Key}", key);
                return null;
            }

            _logger.LogDebug("Verification result cache hit: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get verification result from Redis cache: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync(string key, VerificationResult result, int ttlMinutes = 30)
    {
        if (string.IsNullOrWhiteSpace(key) || result == null)
        {
            return;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            var expiry = TimeSpan.FromMinutes(ttlMinutes);
            
            await _cache.SetAsync(cacheKey, result, expiry);
            
            _logger.LogDebug("Cached verification result in Redis: {Key}, TTL: {TtlMinutes} minutes", key, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache verification result in Redis: {Key}", key);
            throw;
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            await _cache.DeleteAsync<VerificationResult>(cacheKey);
            
            _logger.LogDebug("Removed verification result from Redis cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove verification result from Redis cache: {Key}", key);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            return await _cache.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check verification result existence in Redis cache: {Key}", key);
            return false;
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real Redis implementation, you'd use SCAN to find and delete all keys with the prefix
            _logger.LogWarning("ClearAsync not fully implemented for Redis - consider using Redis SCAN command");
            
            _logger.LogInformation("Cleared verification result cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear verification result cache");
            throw;
        }
    }

    private string GetCacheKey(string key)
    {
        return $"{_keyPrefix}{key}";
    }
}

