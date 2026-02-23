using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Caching;

/// <summary>
/// Interface for DID Document caching
/// </summary>
public interface IDidDocumentCache
{
    /// <summary>
    /// Gets a DID Document from cache
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The DID Document, or null if not found</returns>
    Task<DidDocument?> GetAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a DID Document in cache
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="document">The DID Document</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task SetAsync(string did, DidDocument document, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a DID Document from cache
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the document was removed, false if it wasn't found</returns>
    Task<bool> RemoveAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a DID Document exists in cache
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the document exists, false otherwise</returns>
    Task<bool> ExistsAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached DID Documents
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    /// <returns>Cache statistics</returns>
    CacheStatistics GetStatistics();
}

/// <summary>
/// Cache statistics
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// The number of items in the cache
    /// </summary>
    public long ItemCount { get; set; }

    /// <summary>
    /// The number of cache hits
    /// </summary>
    public long HitCount { get; set; }

    /// <summary>
    /// The number of cache misses
    /// </summary>
    public long MissCount { get; set; }

    /// <summary>
    /// The cache hit ratio (hits / (hits + misses))
    /// </summary>
    public double HitRatio => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) : 0;

    /// <summary>
    /// The number of cache evictions
    /// </summary>
    public long EvictionCount { get; set; }

    /// <summary>
    /// The total memory usage in bytes
    /// </summary>
    public long MemoryUsage { get; set; }

    /// <summary>
    /// The cache type
    /// </summary>
    public string CacheType { get; set; } = string.Empty;
}
