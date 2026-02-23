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
/// Memory-based implementation of DID document cache using IMemoryCache
/// </summary>
public class MemoryDidDocumentCache : IDidDocumentCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryDidDocumentCache> _logger;
    private readonly string _keyPrefix = "did:document:";
    private readonly ConcurrentDictionary<string, int> _hitCounts = new();
    private readonly ConcurrentDictionary<string, int> _missCounts = new();

    public MemoryDidDocumentCache(IMemoryCache memoryCache, ILogger<MemoryDidDocumentCache> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<DidDocument> GetAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return Task.FromResult<DidDocument>(null);
        }

        try
        {
            var key = GetCacheKey(did);
            if (_memoryCache.TryGetValue(key, out CachedDocument cachedDoc))
            {
                // Check if expired
                if (cachedDoc.ExpiresAt < DateTime.UtcNow)
                {
                    _memoryCache.Remove(key);
                    _missCounts.AddOrUpdate("expired", 1, (k, v) => v + 1);
                    _logger.LogDebug("DID document cache expired: {Did}", did);
                    return Task.FromResult<DidDocument>(null);
                }

                _hitCounts.AddOrUpdate("hit", 1, (k, v) => v + 1);
                _logger.LogDebug("DID document cache hit: {Did}", did);
                return Task.FromResult(cachedDoc.Document);
            }

            _missCounts.AddOrUpdate("miss", 1, (k, v) => v + 1);
            _logger.LogDebug("DID document cache miss: {Did}", did);
            return Task.FromResult<DidDocument>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DID document from memory cache: {Did}", did);
            _missCounts.AddOrUpdate("error", 1, (k, v) => v + 1);
            return Task.FromResult<DidDocument>(null);
        }
    }

    public async Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return new Dictionary<string, DidDocument>();
        }

        var result = new Dictionary<string, DidDocument>();
        
        foreach (var did in dids)
        {
            var document = await GetAsync(did);
            if (document != null)
            {
                result[did] = document;
            }
        }

        _logger.LogDebug("Retrieved {Count} DID documents from memory cache", result.Count);
        return result;
    }

    public Task SetAsync(string did, DidDocument document, int ttlMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(did) || document == null)
        {
            return Task.CompletedTask;
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

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(ttlMinutes / 2), // Refresh if accessed within half TTL
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(key, cachedDoc, options);
            
            _logger.LogDebug("Cached DID document in memory: {Did}, TTL: {TtlMinutes} minutes", did, ttlMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache DID document in memory: {Did}", did);
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60)
    {
        if (documents == null || !documents.Any())
        {
            return;
        }

        var tasks = documents.Select(kvp => SetAsync(kvp.Key, kvp.Value, ttlMinutes));
        await Task.WhenAll(tasks);
        
        _logger.LogDebug("Cached {Count} DID documents in memory", documents.Count);
    }

    public Task RemoveAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return Task.CompletedTask;
        }

        try
        {
            var key = GetCacheKey(did);
            _memoryCache.Remove(key);
            
            _logger.LogDebug("Removed DID document from memory cache: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove DID document from memory cache: {Did}", did);
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task RemoveManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return;
        }

        var tasks = dids.Select(RemoveAsync);
        await Task.WhenAll(tasks);
        
        _logger.LogDebug("Removed {Count} DID documents from memory cache", dids.Length);
    }

    public Task<bool> ExistsAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return Task.FromResult(false);
        }

        try
        {
            var key = GetCacheKey(did);
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check DID document existence in memory cache: {Did}", did);
            return Task.FromResult(false);
        }
    }

    public Task<CacheStatistics> GetStatisticsAsync()
    {
        try
        {
            var stats = new CacheStatistics
            {
                HitCount = _hitCounts.Values.Sum(),
                MissCount = _missCounts.Values.Sum(),
                LastAccessed = DateTime.UtcNow,
                MethodStatistics = new Dictionary<string, int>()
            };

            // Add hit/miss statistics
            foreach (var kvp in _hitCounts)
            {
                stats.MethodStatistics[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in _missCounts)
            {
                if (stats.MethodStatistics.ContainsKey(kvp.Key))
                {
                    stats.MethodStatistics[kvp.Key] += kvp.Value;
                }
                else
                {
                    stats.MethodStatistics[kvp.Key] = kvp.Value;
                }
            }

            return Task.FromResult(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache statistics from memory cache");
            return Task.FromResult(new CacheStatistics());
        }
    }

    public Task ClearAsync()
    {
        try
        {
            // Clear all cache entries with our prefix
            // Note: This is a simplified implementation
            // In a real implementation, you'd track keys and remove them individually
            _hitCounts.Clear();
            _missCounts.Clear();
            
            _logger.LogInformation("Cleared memory DID document cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear memory DID document cache");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string did)
    {
        return RemoveAsync(did);
    }

    private string GetCacheKey(string did)
    {
        return $"{_keyPrefix}{did}";
    }

    private class CachedDocument
    {
        public DidDocument Document { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CachedAt { get; set; }
    }
}

