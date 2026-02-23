using Polly;
using Polly.CircuitBreaker;

namespace Mamey.Authentik.Policies;

/// <summary>
/// Circuit breaker policy configuration for Authentik API calls.
/// </summary>
public static class AuthentikCircuitBreakerPolicy
{
    /// <summary>
    /// Creates a circuit breaker policy with default settings.
    /// </summary>
    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CreatePolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    // Circuit opened
                },
                onReset: () =>
                {
                    // Circuit closed
                },
                onHalfOpen: () =>
                {
                    // Circuit half-open
                });
    }

    /// <summary>
    /// Creates a circuit breaker policy with custom settings.
    /// </summary>
    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CreatePolicy(
        int handledEventsAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking,
                durationOfBreak,
                onBreak: (result, duration) =>
                {
                    // Circuit opened
                },
                onReset: () =>
                {
                    // Circuit closed
                },
                onHalfOpen: () =>
                {
                    // Circuit half-open
                });
    }
}
