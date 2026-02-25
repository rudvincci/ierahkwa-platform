using System.Collections.Concurrent;

namespace Ierahkwa.Gateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, TokenBucket> Buckets = new();

    // Rate limits by tier
    private static readonly Dictionary<string, (int RequestsPerMinute, int BurstSize)> TierLimits = new()
    {
        ["anonymous"] = (30, 10),
        ["member"] = (120, 30),
        ["resident"] = (300, 60),
        ["citizen"] = (600, 120),
        ["admin"] = (1200, 240)
    };

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientKey = GetClientKey(context);
        var tier = context.User?.FindFirst("tier")?.Value ?? "anonymous";
        var limits = TierLimits.GetValueOrDefault(tier, TierLimits["anonymous"]);

        var bucket = Buckets.GetOrAdd(clientKey, _ => new TokenBucket(limits.RequestsPerMinute, limits.BurstSize));

        if (!bucket.TryConsume())
        {
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = "60";
            context.Response.Headers["X-RateLimit-Limit"] = limits.RequestsPerMinute.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "rate_limit_exceeded",
                message = $"Rate limit exceeded for tier '{tier}'. Max {limits.RequestsPerMinute} requests/minute.",
                retryAfter = 60
            });
            return;
        }

        context.Response.Headers["X-RateLimit-Limit"] = limits.RequestsPerMinute.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = bucket.Remaining.ToString();

        await _next(context);
    }

    private static string GetClientKey(HttpContext context)
    {
        var userId = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ip}";
    }

    private class TokenBucket
    {
        private readonly int _maxTokens;
        private readonly double _refillRate; // tokens per second
        private double _tokens;
        private DateTime _lastRefill;
        private readonly object _lock = new();

        public int Remaining => (int)_tokens;

        public TokenBucket(int requestsPerMinute, int burstSize)
        {
            _maxTokens = burstSize;
            _refillRate = requestsPerMinute / 60.0;
            _tokens = burstSize;
            _lastRefill = DateTime.UtcNow;
        }

        public bool TryConsume()
        {
            lock (_lock)
            {
                Refill();
                if (_tokens < 1) return false;
                _tokens -= 1;
                return true;
            }
        }

        private void Refill()
        {
            var now = DateTime.UtcNow;
            var elapsed = (now - _lastRefill).TotalSeconds;
            _tokens = Math.Min(_maxTokens, _tokens + elapsed * _refillRate);
            _lastRefill = now;
        }
    }
}
