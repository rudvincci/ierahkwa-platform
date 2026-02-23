using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Hybrid cache implementation that can use either memory or Redis based on configuration
/// </summary>
public class HybridDidDocumentCache : IDidDocumentCache
{
    private readonly IDidDocumentCache _memoryCache;
    private readonly IDidDocumentCache _redisCache;
    private readonly ILogger<HybridDidDocumentCache> _logger;
    private readonly bool _useRedis;
    private readonly bool _useMemory;

    public HybridDidDocumentCache(
        IMemoryCache memoryCache,
        ICache redisCache,
        ILogger<HybridDidDocumentCache> logger,
        bool useRedis = true,
        bool useMemory = true)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _useRedis = useRedis;
        _useMemory = useMemory;

        if (useMemory)
        {
            _memoryCache = new MemoryDidDocumentCache(memoryCache, logger as ILogger<MemoryDidDocumentCache>);
        }

        if (useRedis && redisCache != null)
        {
            _redisCache = new RedisDidDocumentCache(redisCache, logger as ILogger<RedisDidDocumentCache>);
        }
    }

    public async Task<DidDocument> GetAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return null;
        }

        // Try memory cache first (faster)
        if (_useMemory && _memoryCache != null)
        {
            var memoryResult = await _memoryCache.GetAsync(did);
            if (memoryResult != null)
            {
                _logger.LogDebug("DID document found in memory cache: {Did}", did);
                return memoryResult;
            }
        }

        // Try Redis cache
        if (_useRedis && _redisCache != null)
        {
            var redisResult = await _redisCache.GetAsync(did);
            if (redisResult != null)
            {
                _logger.LogDebug("DID document found in Redis cache: {Did}", did);
                
                // Populate memory cache for faster future access
                if (_useMemory && _memoryCache != null)
                {
                    await _memoryCache.SetAsync(did, redisResult, 60);
                }
                
                return redisResult;
            }
        }

        _logger.LogDebug("DID document not found in any cache: {Did}", did);
        return null;
    }

    public async Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return new Dictionary<string, DidDocument>();
        }

        var result = new Dictionary<string, DidDocument>();
        var missingDids = new List<string>();

        // Try memory cache first
        if (_useMemory && _memoryCache != null)
        {
            var memoryResults = await _memoryCache.GetManyAsync(dids);
            foreach (var kvp in memoryResults)
            {
                result[kvp.Key] = kvp.Value;
            }
            missingDids = dids.Except(result.Keys).ToList();
        }
        else
        {
            missingDids = dids.ToList();
        }

        // Try Redis cache for missing DIDs
        if (missingDids.Any() && _useRedis && _redisCache != null)
        {
            var redisResults = await _redisCache.GetManyAsync(missingDids.ToArray());
            foreach (var kvp in redisResults)
            {
                result[kvp.Key] = kvp.Value;
                
                // Populate memory cache
                if (_useMemory && _memoryCache != null)
                {
                    await _memoryCache.SetAsync(kvp.Key, kvp.Value, 60);
                }
            }
        }

        _logger.LogDebug("Retrieved {Count} DID documents from hybrid cache", result.Count);
        return result;
    }

    public async Task SetAsync(string did, DidDocument document, int ttlMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(did) || document == null)
        {
            return;
        }

        var tasks = new List<Task>();

        if (_useMemory && _memoryCache != null)
        {
            tasks.Add(_memoryCache.SetAsync(did, document, ttlMinutes));
        }

        if (_useRedis && _redisCache != null)
        {
            tasks.Add(_redisCache.SetAsync(did, document, ttlMinutes));
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
            _logger.LogDebug("Cached DID document in hybrid cache: {Did}", did);
        }
    }

    public async Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60)
    {
        if (documents == null || !documents.Any())
        {
            return;
        }

        var tasks = new List<Task>();

        if (_useMemory && _memoryCache != null)
        {
            tasks.Add(_memoryCache.SetManyAsync(documents, ttlMinutes));
        }

        if (_useRedis && _redisCache != null)
        {
            tasks.Add(_redisCache.SetManyAsync(documents, ttlMinutes));
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
            _logger.LogDebug("Cached {Count} DID documents in hybrid cache", documents.Count);
        }
    }

    public async Task RemoveAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return;
        }

        var tasks = new List<Task>();

        if (_useMemory && _memoryCache != null)
        {
            tasks.Add(_memoryCache.RemoveAsync(did));
        }

        if (_useRedis && _redisCache != null)
        {
            tasks.Add(_redisCache.RemoveAsync(did));
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
            _logger.LogDebug("Removed DID document from hybrid cache: {Did}", did);
        }
    }

    public async Task RemoveManyAsync(params string[] dids)
    {
        if (dids == null || !dids.Any())
        {
            return;
        }

        var tasks = new List<Task>();

        if (_useMemory && _memoryCache != null)
        {
            tasks.Add(_memoryCache.RemoveManyAsync(dids));
        }

        if (_useRedis && _redisCache != null)
        {
            tasks.Add(_redisCache.RemoveManyAsync(dids));
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
            _logger.LogDebug("Removed {Count} DID documents from hybrid cache", dids.Length);
        }
    }

    public async Task<bool> ExistsAsync(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
        {
            return false;
        }

        // Check memory cache first
        if (_useMemory && _memoryCache != null)
        {
            var exists = await _memoryCache.ExistsAsync(did);
            if (exists)
            {
                return true;
            }
        }

        // Check Redis cache
        if (_useRedis && _redisCache != null)
        {
            return await _redisCache.ExistsAsync(did);
        }

        return false;
    }

    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        // Combine statistics from both caches
        var stats = new CacheStatistics();

        if (_useMemory && _memoryCache != null)
        {
            var memoryStats = await _memoryCache.GetStatisticsAsync();
            stats.HitCount += memoryStats.HitCount;
            stats.MissCount += memoryStats.MissCount;
            stats.TotalEntries += memoryStats.TotalEntries;
        }

        if (_useRedis && _redisCache != null)
        {
            var redisStats = await _redisCache.GetStatisticsAsync();
            stats.HitCount += redisStats.HitCount;
            stats.MissCount += redisStats.MissCount;
            stats.TotalEntries += redisStats.TotalEntries;
        }

        stats.LastAccessed = DateTime.UtcNow;
        return stats;
    }

    public async Task ClearAsync()
    {
        var tasks = new List<Task>();

        if (_useMemory && _memoryCache != null)
        {
            tasks.Add(_memoryCache.ClearAsync());
        }

        if (_useRedis && _redisCache != null)
        {
            tasks.Add(_redisCache.ClearAsync());
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Cleared hybrid DID document cache");
        }
    }

    public async Task InvalidateAsync(string did)
    {
        await RemoveAsync(did);
    }
}

