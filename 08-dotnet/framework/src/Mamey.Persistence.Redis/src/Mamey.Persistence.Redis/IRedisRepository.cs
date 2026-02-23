namespace Mamey.Redis;

/// <summary>
/// Generic Redis repository interface for caching and persistence operations.
/// </summary>
/// <typeparam name="T">The type of entity to store in Redis</typeparam>
public interface IRedisRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by key
    /// </summary>
    Task<T?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an entity with the specified key
    /// </summary>
    Task SetAsync(string key, T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an entity with the specified key and expiration
    /// </summary>
    Task SetAsync(string key, T entity, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity by key
    /// </summary>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple entities by keys
    /// </summary>
    Task<IDictionary<string, T?>> GetManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets multiple entities with their keys
    /// </summary>
    Task SetManyAsync(IDictionary<string, T> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all keys matching the specified pattern
    /// </summary>
    Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments a counter by key
    /// </summary>
    Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets expiration for a key
    /// </summary>
    Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
}