namespace Mamey.Government.Identity.Infrastructure.Redis.Options;

/// <summary>
/// Configuration options for Redis sync services.
/// </summary>
public class RedisSyncOptions
{
    /// <summary>
    /// Gets or sets the sync interval. Default is 5 minutes.
    /// </summary>
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the initial delay before starting the first sync. Default is 30 seconds.
    /// </summary>
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the retry delay when an error occurs. Default is 1 minute.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the time to live (TTL) for cached items in Redis. Default is 24 hours.
    /// </summary>
    public TimeSpan CacheTimeToLive { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Gets or sets whether the sync service is enabled. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;
}


