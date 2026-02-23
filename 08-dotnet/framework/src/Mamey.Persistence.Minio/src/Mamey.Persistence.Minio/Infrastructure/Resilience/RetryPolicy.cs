namespace Mamey.Persistence.Minio.Infrastructure.Resilience;

/// <summary>
/// Configuration for retry policies in Minio operations.
/// </summary>
public class RetryPolicy
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries.
    /// </summary>
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the backoff multiplier for exponential backoff.
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Gets or sets the maximum delay between retries.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to use jitter for retry delays.
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Gets or sets the jitter factor (0.0 to 1.0).
    /// </summary>
    public double JitterFactor { get; set; } = 0.1;

    /// <summary>
    /// Gets or sets the timeout for individual operations.
    /// </summary>
    public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
