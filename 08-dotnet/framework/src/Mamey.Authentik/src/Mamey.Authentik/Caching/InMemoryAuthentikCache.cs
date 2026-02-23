using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Authentik.Caching;

/// <summary>
/// In-memory cache implementation using IMemoryCache.
/// </summary>
public class InMemoryAuthentikCache : IAuthentikCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryAuthentikCache> _logger;
    private readonly ConcurrentDictionary<string, object> _keys = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryAuthentikCache"/> class.
    /// </summary>
    public InMemoryAuthentikCache(
        IMemoryCache memoryCache,
        ILogger<InMemoryAuthentikCache> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return Task.FromResult<T?>(typedValue);
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        return Task.FromResult<T?>(null);
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
            SlidingExpiration = null
        };

        _memoryCache.Set(key, value, options);
        _keys.TryAdd(key, value!);
        _logger.LogDebug("Cached value for key: {Key} with TTL: {Ttl}", key, ttl);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _keys.TryRemove(key, out _);
        _logger.LogDebug("Removed cache entry for key: {Key}", key);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _keys.Keys
            .Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        foreach (var key in _keys.Keys)
        {
            _memoryCache.Remove(key);
        }

        _keys.Clear();
        _logger.LogDebug("Cleared all cache entries");

        return Task.CompletedTask;
    }
}
