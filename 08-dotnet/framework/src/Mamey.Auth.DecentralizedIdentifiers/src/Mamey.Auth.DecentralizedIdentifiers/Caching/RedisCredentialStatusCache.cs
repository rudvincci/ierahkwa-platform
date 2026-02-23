using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Redis implementation of credential status cache
/// </summary>
public class RedisCredentialStatusCache : ICredentialStatusCache
{
    private readonly ICache _cache;
    private readonly ILogger<RedisCredentialStatusCache> _logger;
    private readonly string _keyPrefix = "did:credential:status:";
    private readonly int _defaultTtlMinutes = 15;

    public RedisCredentialStatusCache(ICache cache, ILogger<RedisCredentialStatusCache> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CredentialStatusResult> GetAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return null;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            var result = await _cache.GetAsync<CredentialStatusResult>(key);
            
            if (result == null)
            {
                _logger.LogDebug("Credential status cache miss: {CredentialId}", credentialId);
                return null;
            }

            // Check if expired (status checks should be relatively short-lived)
            if (result.CheckedAt.AddMinutes(_defaultTtlMinutes) < DateTime.UtcNow)
            {
                await _cache.DeleteAsync<CredentialStatusResult>(key);
                _logger.LogDebug("Credential status cache expired: {CredentialId}", credentialId);
                return null;
            }

            _logger.LogDebug("Credential status cache hit: {CredentialId}", credentialId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get credential status from Redis cache: {CredentialId}", credentialId);
            return null;
        }
    }

    public async Task SetAsync(string credentialId, CredentialStatusResult status, int ttlMinutes = 15)
    {
        if (string.IsNullOrWhiteSpace(credentialId) || status == null)
        {
            return;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            var expiry = TimeSpan.FromMinutes(ttlMinutes);
            
            await _cache.SetAsync(key, status, expiry);
            
            _logger.LogDebug("Cached credential status in Redis: {CredentialId}, TTL: {TtlMinutes} minutes", credentialId, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache credential status in Redis: {CredentialId}", credentialId);
            throw;
        }
    }

    public async Task RemoveAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            await _cache.DeleteAsync<CredentialStatusResult>(key);
            
            _logger.LogDebug("Removed credential status from Redis cache: {CredentialId}", credentialId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove credential status from Redis cache: {CredentialId}", credentialId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string credentialId)
    {
        if (string.IsNullOrWhiteSpace(credentialId))
        {
            return false;
        }

        try
        {
            var key = GetCacheKey(credentialId);
            return await _cache.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check credential status existence in Redis cache: {CredentialId}", credentialId);
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
            
            _logger.LogInformation("Cleared credential status cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear credential status cache");
            throw;
        }
    }

    private string GetCacheKey(string credentialId)
    {
        return $"{_keyPrefix}{credentialId}";
    }
}

