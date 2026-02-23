namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Configuration options for resilient blockchain operations.
/// </summary>
public class ResilientBlockchainOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "resilientBlockchain";

    /// <summary>
    /// Maximum number of retry attempts for failed operations.
    /// Default: 5 attempts.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 5;

    /// <summary>
    /// Initial delay for exponential backoff in milliseconds.
    /// Default: 1000ms (1 second).
    /// </summary>
    public int InitialRetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Maximum delay between retries in milliseconds.
    /// Default: 30000ms (30 seconds).
    /// </summary>
    public int MaxRetryDelayMs { get; set; } = 30000;

    /// <summary>
    /// Exponential backoff multiplier.
    /// Default: 2.0 (doubles delay each retry).
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Whether to add jitter to retry delays.
    /// Default: true (reduces thundering herd).
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Circuit breaker failure threshold.
    /// Number of consecutive failures before circuit opens.
    /// Default: 5 failures.
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker recovery time in seconds.
    /// Time the circuit stays open before testing again.
    /// Default: 60 seconds.
    /// </summary>
    public int CircuitBreakerRecoverySeconds { get; set; } = 60;

    /// <summary>
    /// Background sync interval in seconds.
    /// How often to retry failed blockchain account creations.
    /// Default: 300 seconds (5 minutes).
    /// </summary>
    public int BackgroundSyncIntervalSeconds { get; set; } = 300;

    /// <summary>
    /// Maximum number of failed creations to process per sync cycle.
    /// Default: 50.
    /// </summary>
    public int MaxFailedCreationsPerSync { get; set; } = 50;

    /// <summary>
    /// Maximum age in hours for failed creations to retry.
    /// Older failures are considered permanent and won't be retried.
    /// Default: 24 hours.
    /// </summary>
    public int MaxFailedCreationAgeHours { get; set; } = 24;

    /// <summary>
    /// Whether to enable background sync service.
    /// Default: true.
    /// </summary>
    public bool EnableBackgroundSync { get; set; } = true;

    /// <summary>
    /// Whether to enable Prometheus metrics.
    /// Default: true.
    /// </summary>
    public bool EnableMetrics { get; set; } = true;
}
