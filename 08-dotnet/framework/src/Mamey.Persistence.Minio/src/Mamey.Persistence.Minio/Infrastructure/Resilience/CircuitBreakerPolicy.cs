namespace Mamey.Persistence.Minio.Infrastructure.Resilience;

/// <summary>
/// Configuration for circuit breaker policies in Minio operations.
/// </summary>
public class CircuitBreakerPolicy
{
    /// <summary>
    /// Gets or sets the number of consecutive failures before opening the circuit.
    /// </summary>
    public int FailureThreshold { get; set; } = 5;

    /// <summary>
    /// Gets or sets the duration the circuit stays open before attempting to close.
    /// </summary>
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the duration for sampling failures.
    /// </summary>
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the minimum throughput before the circuit breaker can open.
    /// </summary>
    public int MinimumThroughput { get; set; } = 2;

    /// <summary>
    /// Gets or sets the failure ratio threshold (0.0 to 1.0).
    /// </summary>
    public double FailureRatioThreshold { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets whether the circuit breaker is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
