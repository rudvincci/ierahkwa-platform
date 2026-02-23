using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Mamey.Authentik.Caching;

/// <summary>
/// Distributed cache implementation using IDistributedCache (e.g., Redis).
/// </summary>
public class DistributedAuthentikCache : IAuthentikCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedAuthentikCache> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedAuthentikCache"/> class.
    /// </summary>
    public DistributedAuthentikCache(
        IDistributedCache distributedCache,
        ILogger<DistributedAuthentikCache> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            if (cachedBytes == null || cachedBytes.Length == 0)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            var json = System.Text.Encoding.UTF8.GetString(cachedBytes);
            var value = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            
            if (value != null)
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading from distributed cache for key: {Key}", key);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await _distributedCache.SetAsync(key, bytes, options, cancellationToken);
            _logger.LogDebug("Cached value for key: {Key} with TTL: {Ttl}", key, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error writing to distributed cache for key: {Key}", key);
            // Don't throw - caching failures shouldn't break the application
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Removed cache entry for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing from distributed cache for key: {Key}", key);
        }
    }

    /// <inheritdoc />
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: IDistributedCache doesn't support pattern matching natively.
        // This would require Redis-specific implementation or scanning all keys.
        // For now, we'll log a warning and do nothing.
        _logger.LogWarning(
            "Pattern-based cache removal not supported by IDistributedCache. Pattern: {Pattern}",
            pattern);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Note: IDistributedCache doesn't support clearing all keys natively.
        // This would require Redis-specific implementation.
        _logger.LogWarning("Cache clearing not supported by IDistributedCache");
        return Task.CompletedTask;
    }
}
