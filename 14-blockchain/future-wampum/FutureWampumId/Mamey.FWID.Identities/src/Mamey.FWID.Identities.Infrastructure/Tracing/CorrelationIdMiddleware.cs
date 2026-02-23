using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Tracing;

/// <summary>
/// Middleware for managing correlation IDs across requests.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    
    public const string CorrelationIdHeader = "X-Correlation-ID";
    public const string TraceIdHeader = "X-Trace-ID";
    public const string SpanIdHeader = "X-Span-ID";
    public const string ParentSpanIdHeader = "X-Parent-Span-ID";
    
    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task InvokeAsync(HttpContext context, ICorrelationContext correlationContext)
    {
        // Extract or generate correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
        
        // Extract W3C Trace Context if present
        var traceId = context.Request.Headers[TraceIdHeader].FirstOrDefault()
            ?? GenerateTraceId();
        var parentSpanId = context.Request.Headers[SpanIdHeader].FirstOrDefault();
        var spanId = GenerateSpanId();
        
        // Set correlation context
        correlationContext.CorrelationId = correlationId;
        correlationContext.TraceId = traceId;
        correlationContext.SpanId = spanId;
        correlationContext.ParentSpanId = parentSpanId;
        correlationContext.ServiceName = "FWID.Identities";
        correlationContext.OperationName = $"{context.Request.Method} {context.Request.Path}";
        correlationContext.StartTime = DateTime.UtcNow;
        
        // Add to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.TryAdd(CorrelationIdHeader, correlationId);
            context.Response.Headers.TryAdd(TraceIdHeader, traceId);
            context.Response.Headers.TryAdd(SpanIdHeader, spanId);
            return Task.CompletedTask;
        });
        
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = traceId,
            ["SpanId"] = spanId
        }))
        {
            _logger.LogDebug("Request started: {Method} {Path}", context.Request.Method, context.Request.Path);
            
            try
            {
                await _next(context);
            }
            finally
            {
                correlationContext.EndTime = DateTime.UtcNow;
                _logger.LogDebug("Request completed: {Method} {Path} in {Duration}ms",
                    context.Request.Method, context.Request.Path,
                    correlationContext.Duration.TotalMilliseconds);
            }
        }
    }
    
    private static string GenerateTraceId()
    {
        // W3C Trace Context format: 32 hex characters
        return Guid.NewGuid().ToString("N");
    }
    
    private static string GenerateSpanId()
    {
        // W3C Trace Context format: 16 hex characters
        return Guid.NewGuid().ToString("N")[..16];
    }
}

/// <summary>
/// Context for correlation tracking.
/// </summary>
public interface ICorrelationContext
{
    string CorrelationId { get; set; }
    string TraceId { get; set; }
    string SpanId { get; set; }
    string? ParentSpanId { get; set; }
    string ServiceName { get; set; }
    string OperationName { get; set; }
    DateTime StartTime { get; set; }
    DateTime? EndTime { get; set; }
    TimeSpan Duration { get; }
    Dictionary<string, object> Baggage { get; }
}

/// <summary>
/// Correlation context implementation.
/// </summary>
public class CorrelationContext : ICorrelationContext
{
    public string CorrelationId { get; set; } = null!;
    public string TraceId { get; set; } = null!;
    public string SpanId { get; set; } = null!;
    public string? ParentSpanId { get; set; }
    public string ServiceName { get; set; } = null!;
    public string OperationName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan Duration => (EndTime ?? DateTime.UtcNow) - StartTime;
    public Dictionary<string, object> Baggage { get; } = new();
}
