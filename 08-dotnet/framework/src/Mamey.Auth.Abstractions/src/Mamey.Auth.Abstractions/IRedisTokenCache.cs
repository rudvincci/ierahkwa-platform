namespace Mamey.Auth.Abstractions;

/// <summary>
/// Interface for distributed token caching.
/// </summary>
public interface IRedisTokenCache
{
    /// <summary>
    /// Gets a cached token by key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>The token or null.</returns>
    Task<string?> GetCachedTokenAsync(string key);

    /// <summary>
    /// Sets a token in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="token">The token to store.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetCachedTokenAsync(string key, string token);

    /// <summary>
    /// Removes a token from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveCachedTokenAsync(string key);
}