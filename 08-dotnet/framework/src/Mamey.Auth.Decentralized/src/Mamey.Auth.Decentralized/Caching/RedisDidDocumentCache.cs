using System.Text.Json;
using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Caching;

/// <summary>
/// Redis implementation of DID Document cache
/// </summary>
public class RedisDidDocumentCache : IDidDocumentCache
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisDidDocumentCache> _logger;
    private readonly string _keyPrefix;
    private long _hitCount;
    private long _missCount;

    public RedisDidDocumentCache(IDatabase database, ILogger<RedisDidDocumentCache> logger, string keyPrefix = "did:document:")
    {
        _database = database;
        _logger = logger;
        _keyPrefix = keyPrefix;
    }

    public async Task<DidDocument?> GetAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Document from Redis cache: {Did}", did);

            var key = GetCacheKey(did);
            var value = await _database.StringGetAsync(key);

            if (value.HasValue)
            {
                Interlocked.Increment(ref _hitCount);
                _logger.LogDebug("Cache hit for DID: {Did}", did);
                
                var document = JsonSerializer.Deserialize<DidDocument>(value!);
                return document;
            }

            Interlocked.Increment(ref _missCount);
            _logger.LogDebug("Cache miss for DID: {Did}", did);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Document from Redis cache: {Did}", did);
            throw;
        }
    }

    public async Task SetAsync(string did, DidDocument document, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Setting DID Document in Redis cache: {Did}", did);

            var key = GetCacheKey(did);
            var value = JsonSerializer.Serialize(document);

            if (expiration.HasValue)
            {
                await _database.StringSetAsync(key, value, expiration.Value);
            }
            else
            {
                await _database.StringSetAsync(key, value);
            }

            _logger.LogDebug("DID Document cached successfully in Redis: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting DID Document in Redis cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> RemoveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing DID Document from Redis cache: {Did}", did);

            var key = GetCacheKey(did);
            var removed = await _database.KeyDeleteAsync(key);

            _logger.LogDebug("DID Document removal result: {Removed} for DID: {Did}", removed, did);
            return removed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing DID Document from Redis cache: {Did}", did);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if DID Document exists in Redis cache: {Did}", did);

            var key = GetCacheKey(did);
            var exists = await _database.KeyExistsAsync(key);

            _logger.LogDebug("DID Document exists in Redis cache: {Exists} for DID: {Did}", exists, did);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if DID Document exists in Redis cache: {Did}", did);
            throw;
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Clearing all DID Documents from Redis cache");

            var pattern = $"{_keyPrefix}*";
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            var count = 0;
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
                count++;
            }

            _logger.LogDebug("Cleared {Count} DID Documents from Redis cache", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing Redis cache");
            throw;
        }
    }

    public CacheStatistics GetStatistics()
    {
        var totalHits = Interlocked.Read(ref _hitCount);
        var totalMisses = Interlocked.Read(ref _missCount);

        return new CacheStatistics
        {
            ItemCount = GetItemCount(),
            HitCount = totalHits,
            MissCount = totalMisses,
            EvictionCount = 0, // Redis handles evictions automatically
            MemoryUsage = 0, // Redis memory usage is not easily accessible
            CacheType = "Redis"
        };
    }

    private string GetCacheKey(string did)
    {
        return $"{_keyPrefix}{did}";
    }

    private long GetItemCount()
    {
        try
        {
            var pattern = $"{_keyPrefix}*";
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            return keys.Count();
        }
        catch
        {
            return 0;
        }
    }
}
