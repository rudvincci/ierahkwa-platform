using Polly;

namespace Mamey.Persistence.Minio.Infrastructure.Resilience;

/// <summary>
/// Interface for executing operations with retry policies.
/// </summary>
public interface IRetryPolicyExecutor
{
    /// <summary>
    /// Executes an operation with retry policy.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with retry policy.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the configured retry policy.
    /// </summary>
    /// <returns>The retry policy.</returns>
    IAsyncPolicy GetRetryPolicy();

    /// <summary>
    /// Gets the configured circuit breaker policy.
    /// </summary>
    /// <returns>The circuit breaker policy.</returns>
    IAsyncPolicy? GetCircuitBreakerPolicy();
}
