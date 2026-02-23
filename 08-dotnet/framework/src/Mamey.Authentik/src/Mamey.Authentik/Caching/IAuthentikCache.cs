namespace Mamey.Authentik.Caching;

/// <summary>
/// Interface for caching Authentik API responses.
/// </summary>
public interface IAuthentikCache
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets a cached value with the specified key and TTL.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all cached values matching the specified pattern.
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached values.
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
