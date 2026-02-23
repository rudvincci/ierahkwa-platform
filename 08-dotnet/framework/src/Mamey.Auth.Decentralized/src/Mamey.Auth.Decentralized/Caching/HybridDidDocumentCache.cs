using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Caching;

/// <summary>
/// Hybrid implementation of DID Document cache using both in-memory and Redis
/// </summary>
public class HybridDidDocumentCache : IDidDocumentCache
{
    private readonly InMemoryDidDocumentCache _inMemoryCache;
    private readonly RedisDidDocumentCache _redisCache;
    private readonly ILogger<HybridDidDocumentCache> _logger;
    private readonly CacheOptions _options;

    public HybridDidDocumentCache(
        InMemoryDidDocumentCache inMemoryCache,
        RedisDidDocumentCache redisCache,
        ILogger<HybridDidDocumentCache> logger,
        CacheOptions? options = null)
    {
        _inMemoryCache = inMemoryCache;
        _redisCache = redisCache;
        _logger = logger;
        _options = options ?? new CacheOptions();
    }

    public async Task<DidDocument?> GetAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Document from hybrid cache: {Did}", did);

            // First try in-memory cache
            var document = await _inMemoryCache.GetAsync(did, cancellationToken);
            if (document != null)
            {
                _logger.LogDebug("DID Document found in in-memory cache: {Did}", did);
                return document;
            }

            // If not in memory, try Redis
            document = await _redisCache.GetAsync(did, cancellationToken);
            if (document != null)
            {
                _logger.LogDebug("DID Document found in Redis cache: {Did}", did);
                
                // Populate in-memory cache for faster access next time
                await _inMemoryCache.SetAsync(did, document, _options.InMemoryExpiration, cancellationToken);
                return document;
            }

            _logger.LogDebug("DID Document not found in any cache: {Did}", did);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Document from hybrid cache: {Did}", did);
            throw;
        }
    }

    public async Task SetAsync(string did, DidDocument document, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Setting DID Document in hybrid cache: {Did}", did);

            // Set in both caches
            var tasks = new List<Task>
            {
                _inMemoryCache.SetAsync(did, document, _options.InMemoryExpiration, cancellationToken),
                _redisCache.SetAsync(did, document, expiration ?? _options.RedisExpiration, cancellationToken)
            };

            await Task.WhenAll(tasks);
            _logger.LogDebug("DID Document cached successfully in hybrid cache: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting DID Document in hybrid cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> RemoveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing DID Document from hybrid cache: {Did}", did);

            // Remove from both caches
            var tasks = new List<Task<bool>>
            {
                _inMemoryCache.RemoveAsync(did, cancellationToken),
                _redisCache.RemoveAsync(did, cancellationToken)
            };

            var results = await Task.WhenAll(tasks);
            var removed = results.Any(r => r);

            _logger.LogDebug("DID Document removal result: {Removed} for DID: {Did}", removed, did);
            return removed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing DID Document from hybrid cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if DID Document exists in hybrid cache: {Did}", did);

            // Check in-memory cache first
            var exists = await _inMemoryCache.ExistsAsync(did, cancellationToken);
            if (exists)
            {
                _logger.LogDebug("DID Document exists in in-memory cache: {Did}", did);
                return true;
            }

            // Check Redis cache
            exists = await _redisCache.ExistsAsync(did, cancellationToken);
            _logger.LogDebug("DID Document exists in Redis cache: {Exists} for DID: {Did}", exists, did);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if DID Document exists in hybrid cache: {Did}", did);
            throw;
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Clearing all DID Documents from hybrid cache");

            // Clear both caches
            var tasks = new List<Task>
            {
                _inMemoryCache.ClearAsync(cancellationToken),
                _redisCache.ClearAsync(cancellationToken)
            };

            await Task.WhenAll(tasks);
            _logger.LogDebug("Cleared all DID Documents from hybrid cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing hybrid cache");
            throw;
        }
    }

    public CacheStatistics GetStatistics()
    {
        var inMemoryStats = _inMemoryCache.GetStatistics();
        var redisStats = _redisCache.GetStatistics();

        return new CacheStatistics
        {
            ItemCount = inMemoryStats.ItemCount + redisStats.ItemCount,
            HitCount = inMemoryStats.HitCount + redisStats.HitCount,
            MissCount = inMemoryStats.MissCount + redisStats.MissCount,
            EvictionCount = inMemoryStats.EvictionCount + redisStats.EvictionCount,
            MemoryUsage = inMemoryStats.MemoryUsage + redisStats.MemoryUsage,
            CacheType = "Hybrid"
        };
    }
}

/// <summary>
/// Cache configuration options
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// The expiration time for in-memory cache entries
    /// </summary>
    public TimeSpan InMemoryExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The expiration time for Redis cache entries
    /// </summary>
    public TimeSpan RedisExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// The maximum number of items in the in-memory cache
    /// </summary>
    public int MaxInMemoryItems { get; set; } = 1000;

    /// <summary>
    /// Whether to enable cache warming
    /// </summary>
    public bool EnableCacheWarming { get; set; } = true;

    /// <summary>
    /// The cache warming batch size
    /// </summary>
    public int CacheWarmingBatchSize { get; set; } = 100;
}
