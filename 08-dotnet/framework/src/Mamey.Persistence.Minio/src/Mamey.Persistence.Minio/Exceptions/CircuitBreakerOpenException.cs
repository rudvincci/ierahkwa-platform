using Mamey.Exceptions;

namespace Mamey.Persistence.Minio.Exceptions;

/// <summary>
/// Exception thrown when the circuit breaker is open.
/// </summary>
public class CircuitBreakerOpenException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CircuitBreakerOpenException"/> class.
    /// </summary>
    /// <param name="operation">The operation that was blocked.</param>
    /// <param name="durationOfBreak">The duration the circuit breaker will remain open.</param>
    public CircuitBreakerOpenException(string operation, TimeSpan durationOfBreak)
        : base($"Circuit breaker is open for operation '{operation}'", "circuit_breaker_open", 
               $"The circuit breaker is open and will remain so for {durationOfBreak.TotalSeconds} seconds")
    {
        Operation = operation;
        DurationOfBreak = durationOfBreak;
    }

    /// <summary>
    /// Gets the operation that was blocked.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the duration the circuit breaker will remain open.
    /// </summary>
    public TimeSpan DurationOfBreak { get; }
}
