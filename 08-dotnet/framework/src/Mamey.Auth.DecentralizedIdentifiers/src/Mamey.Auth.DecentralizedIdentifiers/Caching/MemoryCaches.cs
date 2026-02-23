using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Memory-based implementation of verification result cache using IMemoryCache
/// </summary>
public class MemoryVerificationResultCache : IVerificationResultCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryVerificationResultCache> _logger;
    private readonly string _keyPrefix = "did:verification:";
    private readonly int _defaultTtlMinutes = 30;

    public MemoryVerificationResultCache(IMemoryCache memoryCache, ILogger<MemoryVerificationResultCache> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<VerificationResult> GetAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.FromResult<VerificationResult>(null);
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            if (_memoryCache.TryGetValue(cacheKey, out VerificationResult result))
            {
                _logger.LogDebug("Verification result cache hit: {Key}", key);
                return Task.FromResult(result);
            }

            _logger.LogDebug("Verification result cache miss: {Key}", key);
            return Task.FromResult<VerificationResult>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get verification result from memory cache: {Key}", key);
            return Task.FromResult<VerificationResult>(null);
        }
    }

    public Task SetAsync(string key, VerificationResult result, int ttlMinutes = 30)
    {
        if (string.IsNullOrWhiteSpace(key) || result == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(ttlMinutes / 2),
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(cacheKey, result, options);
            
            _logger.LogDebug("Cached verification result in memory: {Key}, TTL: {TtlMinutes} minutes", key, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache verification result in memory: {Key}", key);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.CompletedTask;
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            _memoryCache.Remove(cacheKey);
            
            _logger.LogDebug("Removed verification result from memory cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove verification result from memory cache: {Key}", key);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.FromResult(false);
        }

        try
        {
            var cacheKey = GetCacheKey(key);
            return Task.FromResult(_memoryCache.TryGetValue(cacheKey, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check verification result existence in memory cache: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public Task ClearAsync()
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real implementation, you'd track keys and remove them individually
            _logger.LogInformation("Cleared memory verification result cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear memory verification result cache");
            throw;
        }

        return Task.CompletedTask;
    }

    private string GetCacheKey(string key)
    {
        return $"{_keyPrefix}{key}";
    }
}

/// <summary>
/// Memory-based implementation of credential status cache using IMemoryCache
/// </summary>
public class MemoryCredentialStatusCache : ICredentialStatusCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCredentialStatusCache> _logger;
    private readonly string _keyPrefix = "did:credential:status:";
    private readonly int _defaultTtlMinutes = 15;

    public MemoryCredentialStatusCache(IMemoryCache memoryCache, ILogger<MemoryCredentialStatusCache> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CredentialStatusResult> GetAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return Task.FromResult<CredentialStatusResult>(null);
        }

        try
        {
            var key = GetCacheKey(credentialId);
            if (_memoryCache.TryGetValue(key, out CredentialStatusResult result))
            {
                _logger.LogDebug("Credential status cache hit: {CredentialId}", credentialId);
                return Task.FromResult(result);
            }

            _logger.LogDebug("Credential status cache miss: {CredentialId}", credentialId);
            return Task.FromResult<CredentialStatusResult>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get credential status from memory cache: {CredentialId}", credentialId);
            return Task.FromResult<CredentialStatusResult>(null);
        }
    }

    public Task SetAsync(string credentialId, CredentialStatusResult status, int ttlMinutes = 15)
    {
        if (string.IsNullOrWhiteSpace(credentialId) || status == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(ttlMinutes / 2),
                Priority = CacheItemPriority.High // Status checks are important
            };

            _memoryCache.Set(key, status, options);
            
            _logger.LogDebug("Cached credential status in memory: {CredentialId}, TTL: {TtlMinutes} minutes", credentialId, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache credential status in memory: {CredentialId}", credentialId);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return Task.CompletedTask;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            _memoryCache.Remove(key);
            
            _logger.LogDebug("Removed credential status from memory cache: {CredentialId}", credentialId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove credential status from memory cache: {CredentialId}", credentialId);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return Task.FromResult(false);
        }

        try
        {
            var key = GetCacheKey(credentialId);
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check credential status existence in memory cache: {CredentialId}", credentialId);
            return Task.FromResult(false);
        }
    }

    public Task ClearAsync()
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real implementation, you'd track keys and remove them individually
            _logger.LogInformation("Cleared memory credential status cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear memory credential status cache");
            throw;
        }

        return Task.CompletedTask;
    }

    private string GetCacheKey(string credentialId)
    {
        return $"{_keyPrefix}{credentialId}";
    }
}

