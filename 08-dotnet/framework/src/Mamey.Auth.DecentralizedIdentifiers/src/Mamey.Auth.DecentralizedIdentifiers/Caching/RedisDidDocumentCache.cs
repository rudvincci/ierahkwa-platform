using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Redis implementation of DID document cache
/// </summary>
public class RedisDidDocumentCache : IDidDocumentCache
{
    private readonly ICache _cache;
    private readonly ILogger<RedisDidDocumentCache> _logger;
    private readonly string _keyPrefix = "did:document:";
    private readonly string _statsKey = "did:cache:stats";
    private readonly int _defaultTtlMinutes = 60;

    public RedisDidDocumentCache(ICache cache, ILogger<RedisDidDocumentCache> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DidDocument> GetAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return null;
        }

        try
        {
            var key = GetCacheKey(did);
            var cachedDoc = await _cache.GetAsync<CachedDocument>(key);
            
            if (cachedDoc == null)
            {
                await UpdateStatistics("miss");
                _logger.LogDebug("DID document cache miss: {Did}", did);
                return null;
            }

            // Check if expired
            if (cachedDoc.ExpiresAt < DateTime.UtcNow)
            {
                await _cache.DeleteAsync<CachedDocument>(key);
                await UpdateStatistics("miss");
                _logger.LogDebug("DID document cache expired: {Did}", did);
                return null;
            }

            await UpdateStatistics("hit");
            _logger.LogDebug("DID document cache hit: {Did}", did);
            return cachedDoc.Document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DID document from Redis cache: {Did}", did);
            await UpdateStatistics("error");
            return null;
        }
    }

    public async Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return new Dictionary<string, DidDocument>();
        }

        try
        {
            var keys = dids.Select(GetCacheKey).ToArray();
            var cachedDocs = await _cache.GetManyAsync<CachedDocument>(keys);
            
            var result = new Dictionary<string, DidDocument>();
            var now = DateTime.UtcNow;
            
            for (int i = 0; i < dids.Length; i++)
            {
                var did = dids[i];
                var cachedDoc = cachedDocs.ElementAtOrDefault(i);
                
                if (cachedDoc != null && cachedDoc.ExpiresAt > now)
                {
                    result[did] = cachedDoc.Document;
                    await UpdateStatistics("hit");
                }
                else
                {
                    if (cachedDoc != null)
                    {
                        // Remove expired entry
                        await _cache.DeleteAsync<CachedDocument>(GetCacheKey(did));
                    }
                    await UpdateStatistics("miss");
                }
            }

            _logger.LogDebug("Retrieved {Count} DID documents from Redis cache", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get multiple DID documents from Redis cache");
            await UpdateStatistics("error");
            return new Dictionary<string, DidDocument>();
        }
    }

    public async Task SetAsync(string did, DidDocument document, int ttlMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(did) || document == null)
        {
            return;
        }

        try
        {
            var key = GetCacheKey(did);
            var cachedDoc = new CachedDocument
            {
                Document = document,
                ExpiresAt = DateTime.UtcNow.AddMinutes(ttlMinutes),
                CachedAt = DateTime.UtcNow
            };

            var expiry = TimeSpan.FromMinutes(ttlMinutes);
            await _cache.SetAsync(key, cachedDoc, expiry);
            
            _logger.LogDebug("Cached DID document in Redis: {Did}, TTL: {TtlMinutes} minutes", did, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache DID document in Redis: {Did}", did);
            throw;
        }
    }

    public async Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60)
    {
        if (documents == null || !documents.Any())
        {
            return;
        }

        try
        {
            var tasks = documents.Select(kvp => SetAsync(kvp.Key, kvp.Value, ttlMinutes));
            await Task.WhenAll(tasks);
            
            _logger.LogDebug("Cached {Count} DID documents in Redis", documents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache multiple DID documents in Redis");
            throw;
        }
    }

    public async Task RemoveAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return;
        }

        try
        {
            var key = GetCacheKey(did);
            await _cache.DeleteAsync<CachedDocument>(key);
            
            _logger.LogDebug("Removed DID document from Redis cache: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove DID document from Redis cache: {Did}", did);
            throw;
        }
    }

    public async Task RemoveManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return;
        }

        try
        {
            var tasks = dids.Select(RemoveAsync);
            await Task.WhenAll(tasks);
            
            _logger.LogDebug("Removed {Count} DID documents from Redis cache", dids.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove multiple DID documents from Redis cache");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return false;
        }

        try
        {
            var key = GetCacheKey(did);
            return await _cache.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check DID document existence in Redis cache: {Did}", did);
            return false;
        }
    }

    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        try
        {
            var stats = await _cache.GetAsync<CacheStatistics>(_statsKey);
            return stats ?? new CacheStatistics();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache statistics from Redis");
            return new CacheStatistics();
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real Redis implementation, you'd use SCAN to find and delete all keys with the prefix
            _logger.LogWarning("ClearAsync not fully implemented for Redis - consider using Redis SCAN command");
            
            // Clear statistics
            await _cache.DeleteAsync<CacheStatistics>(_statsKey);
            
            _logger.LogInformation("Cleared DID document cache statistics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear DID document cache");
            throw;
        }
    }

    public async Task InvalidateAsync(string did)
    {
        await RemoveAsync(did);
    }

    private string GetCacheKey(string did)
    {
        return $"{_keyPrefix}{did}";
    }

    private async Task UpdateStatistics(string operation)
    {
        try
        {
            var stats = await _cache.GetAsync<CacheStatistics>(_statsKey) ?? new CacheStatistics();
            
            switch (operation.ToLowerInvariant())
            {
                case "hit":
                    stats.HitCount++;
                    break;
                case "miss":
                    stats.MissCount++;
                    break;
                case "error":
                    // Track errors separately if needed
                    break;
            }
            
            stats.LastAccessed = DateTime.UtcNow;
            
            // Update method statistics
            if (!stats.MethodStatistics.ContainsKey(operation))
            {
                stats.MethodStatistics[operation] = 0;
            }
            stats.MethodStatistics[operation]++;
            
            await _cache.SetAsync(_statsKey, stats, TimeSpan.FromHours(24));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update cache statistics");
        }
    }

    private class CachedDocument
    {
        public DidDocument Document { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CachedAt { get; set; }
    }
}

