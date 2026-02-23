using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio.Exceptions;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Mamey.Persistence.Minio.Infrastructure.Resilience;

/// <summary>
/// Executes operations with retry and circuit breaker policies.
/// </summary>
public class RetryPolicyExecutor : IRetryPolicyExecutor
{
    private readonly RetryPolicy _retryPolicy;
    private readonly CircuitBreakerPolicy? _circuitBreakerPolicy;
    private readonly ILogger<RetryPolicyExecutor> _logger;
    private readonly IAsyncPolicy _retryPolicyInstance;
    private readonly IAsyncPolicy? _circuitBreakerPolicyInstance;

    public RetryPolicyExecutor(IOptions<MinioOptions> options, ILogger<RetryPolicyExecutor> logger)
    {
        _retryPolicy = options.Value.RetryPolicy;
        _circuitBreakerPolicy = options.Value.CircuitBreaker;
        _logger = logger;

        _retryPolicyInstance = CreateRetryPolicy();
        _circuitBreakerPolicyInstance = _circuitBreakerPolicy?.Enabled == true ? CreateCircuitBreakerPolicy() : null;
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        var policy = CombinePolicies();
        return await policy.ExecuteAsync(operation, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        var policy = CombinePolicies();
        await policy.ExecuteAsync(operation, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncPolicy GetRetryPolicy() => _retryPolicyInstance;

    /// <inheritdoc />
    public IAsyncPolicy? GetCircuitBreakerPolicy() => _circuitBreakerPolicyInstance;

    private IAsyncPolicy CombinePolicies()
    {
        // If circuit breaker is disabled, return only the retry policy
        if (_circuitBreakerPolicyInstance == null)
        {
            return _retryPolicyInstance;
        }

        // Validate that retry policy is valid
        if (_retryPolicyInstance == null)
        {
            throw new InvalidOperationException("Retry policy is not configured.");
        }

        // When MaxRetries is 0, avoid wrapping - retry adds no value and can cause Polly "at least two policies" edge cases
        if (_retryPolicy.MaxRetries <= 0)
        {
            return _retryPolicyInstance;
        }

        // If both policies exist, wrap them (retry inner, circuit breaker outer)
        return Policy.WrapAsync(_retryPolicyInstance, _circuitBreakerPolicyInstance);
    }

    private IAsyncPolicy CreateRetryPolicy()
    {
        return Policy
            .Handle<Exception>(IsRetryableException)
            .WaitAndRetryAsync(
                retryCount: _retryPolicy.MaxRetries,
                sleepDurationProvider: retryAttempt => CalculateDelay(retryAttempt))
            .WithPolicyKey("MinioRetryPolicy");
    }

    private IAsyncPolicy CreateCircuitBreakerPolicy()
    {
        if (_circuitBreakerPolicy == null)
            throw new InvalidOperationException("Circuit breaker policy is not configured.");

        return Policy
            .Handle<Exception>(IsRetryableException)
            .AdvancedCircuitBreakerAsync(
                failureThreshold: _circuitBreakerPolicy.FailureRatioThreshold,
                samplingDuration: _circuitBreakerPolicy.SamplingDuration,
                minimumThroughput: _circuitBreakerPolicy.MinimumThroughput,
                durationOfBreak: _circuitBreakerPolicy.DurationOfBreak,
                onBreak: (exception, duration) =>
                {
                    _logger.LogError(
                        "Circuit breaker opened for {Duration}ms due to exception: {Exception}",
                        duration.TotalMilliseconds,
                        exception.Message);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset and closed.");
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker half-opened, allowing test call.");
                })
            .WithPolicyKey("MinioCircuitBreakerPolicy");
    }

    private TimeSpan CalculateDelay(int retryAttempt)
    {
        var delay = TimeSpan.FromMilliseconds(
            _retryPolicy.InitialDelay.TotalMilliseconds * Math.Pow(_retryPolicy.BackoffMultiplier, retryAttempt - 1));

        // Apply jitter if enabled
        if (_retryPolicy.UseJitter)
        {
            var jitter = Random.Shared.NextDouble() * _retryPolicy.JitterFactor;
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * (1 + jitter));
        }

        // Cap at maximum delay
        return delay > _retryPolicy.MaxDelay ? _retryPolicy.MaxDelay : delay;
    }

    private static bool IsRetryableException(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => true,
            TaskCanceledException => true,
            TimeoutException => true,
            MinioException minioException => IsMinioRetryableException(minioException),
            _ => false
        };
    }

    private static bool IsMinioRetryableException(MinioException exception)
    {
        // Retry on transient errors
        return exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("network", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("temporary", StringComparison.OrdinalIgnoreCase);
    }
}