using System.Collections.Concurrent;
using System.Text.Json;
using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Caching;

/// <summary>
/// In-memory implementation of DID Document cache
/// </summary>
public class InMemoryDidDocumentCache : IDidDocumentCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly ILogger<InMemoryDidDocumentCache> _logger;
    private long _hitCount;
    private long _missCount;
    private long _evictionCount;

    public InMemoryDidDocumentCache(ILogger<InMemoryDidDocumentCache> logger)
    {
        _logger = logger;
    }

    public async Task<DidDocument?> GetAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Document from in-memory cache: {Did}", did);

            if (_cache.TryGetValue(did, out var entry))
            {
                if (entry.ExpiresAt == null || entry.ExpiresAt > DateTime.UtcNow)
                {
                    Interlocked.Increment(ref _hitCount);
                    _logger.LogDebug("Cache hit for DID: {Did}", did);
                    return entry.Document;
                }
                else
                {
                    // Entry has expired, remove it
                    _cache.TryRemove(did, out _);
                    Interlocked.Increment(ref _evictionCount);
                    _logger.LogDebug("Expired entry removed from cache for DID: {Did}", did);
                }
            }

            Interlocked.Increment(ref _missCount);
            _logger.LogDebug("Cache miss for DID: {Did}", did);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Document from in-memory cache: {Did}", did);
            throw;
        }
    }

    public async Task SetAsync(string did, DidDocument document, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Setting DID Document in in-memory cache: {Did}", did);

            DateTime? expiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null;
            var entry = new CacheEntry
            {
                Document = document,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _cache.AddOrUpdate(did, entry, (key, existing) => entry);
            _logger.LogDebug("DID Document cached successfully: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting DID Document in in-memory cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> RemoveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing DID Document from in-memory cache: {Did}", did);
            var removed = _cache.TryRemove(did, out _);
            _logger.LogDebug("DID Document removal result: {Removed} for DID: {Did}", removed, did);
            return removed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing DID Document from in-memory cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if DID Document exists in in-memory cache: {Did}", did);

            if (_cache.TryGetValue(did, out var entry))
            {
                if (entry.ExpiresAt == null || entry.ExpiresAt > DateTime.UtcNow)
                {
                    _logger.LogDebug("DID Document exists in cache: {Did}", did);
                    return true;
                }
                else
                {
                    // Entry has expired, remove it
                    _cache.TryRemove(did, out _);
                    Interlocked.Increment(ref _evictionCount);
                    _logger.LogDebug("Expired entry removed from cache for DID: {Did}", did);
                }
            }

            _logger.LogDebug("DID Document does not exist in cache: {Did}", did);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if DID Document exists in in-memory cache: {Did}", did);
            throw;
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Clearing all DID Documents from in-memory cache");
            var count = _cache.Count;
            _cache.Clear();
            _logger.LogDebug("Cleared {Count} DID Documents from in-memory cache", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing in-memory cache");
            throw;
        }
    }

    public CacheStatistics GetStatistics()
    {
        var totalHits = Interlocked.Read(ref _hitCount);
        var totalMisses = Interlocked.Read(ref _missCount);
        var totalEvictions = Interlocked.Read(ref _evictionCount);

        return new CacheStatistics
        {
            ItemCount = _cache.Count,
            HitCount = totalHits,
            MissCount = totalMisses,
            EvictionCount = totalEvictions,
            MemoryUsage = CalculateMemoryUsage(),
            CacheType = "InMemory"
        };
    }

    private long CalculateMemoryUsage()
    {
        try
        {
            // Rough estimation of memory usage
            var totalSize = 0L;
            foreach (var kvp in _cache)
            {
                totalSize += kvp.Key.Length * 2; // String length * 2 bytes per char
                totalSize += EstimateObjectSize(kvp.Value.Document);
            }
            return totalSize;
        }
        catch
        {
            return 0;
        }
    }

    private long EstimateObjectSize(object obj)
    {
        try
        {
            var json = JsonSerializer.Serialize(obj);
            return json.Length * 2; // Rough estimation
        }
        catch
        {
            return 1024; // Default estimation
        }
    }

    private class CacheEntry
    {
        public DidDocument Document { get; set; } = null!;
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
