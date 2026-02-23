using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Mamey.Authentik.Policies;

/// <summary>
/// Retry policy configuration for Authentik API calls.
/// </summary>
public static class AuthentikRetryPolicy
{
    /// <summary>
    /// Creates a retry policy based on the configured options.
    /// </summary>
    public static AsyncRetryPolicy<HttpResponseMessage> CreatePolicy(AuthentikOptions options)
    {
        var retryOptions = options.RetryPolicy;

        var policyBuilder = Policy
            .HandleResult<HttpResponseMessage>(r => ShouldRetry(r))
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .Or<TimeoutException>();

        if (retryOptions.UseExponentialBackoff)
        {
            return policyBuilder.WaitAndRetryAsync(
                retryOptions.MaxRetries,
                retryAttempt => CalculateDelay(retryAttempt, retryOptions),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    context["RetryCount"] = retryCount;
                    context["RetryDelay"] = timespan.TotalMilliseconds;
                });
        }
        else
        {
            return policyBuilder.WaitAndRetryAsync(
                retryOptions.MaxRetries,
                _ => retryOptions.InitialDelay,
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    context["RetryCount"] = retryCount;
                    context["RetryDelay"] = timespan.TotalMilliseconds;
                });
        }
    }

    private static bool ShouldRetry(HttpResponseMessage response)
    {
        // Retry on 5xx server errors and 429 (rate limit)
        return (int)response.StatusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
    }

    private static TimeSpan CalculateDelay(int retryAttempt, RetryPolicyOptions options)
    {
        // Exponential backoff: initialDelay * 2^(retryAttempt - 1)
        var delay = TimeSpan.FromMilliseconds(
            options.InitialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt - 1));

        // Add jitter (random 0-25% of delay)
        var jitter = TimeSpan.FromMilliseconds(
            Random.Shared.NextDouble() * delay.TotalMilliseconds * 0.25);

        var totalDelay = delay + jitter;

        // Cap at max delay
        return totalDelay > options.MaxDelay ? options.MaxDelay : totalDelay;
    }
}
