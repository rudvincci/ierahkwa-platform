using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Infrastructure.Tracing;

/// <summary>
/// Extension methods for configuring distributed tracing.
/// </summary>
public static class TracingExtensions
{
    /// <summary>
    /// Adds tracing services.
    /// </summary>
    public static IServiceCollection AddFWIDTracing(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<ISpanContextPropagator, SpanContextPropagator>();
        
        return services;
    }
    
    /// <summary>
    /// Uses correlation ID middleware.
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}

/// <summary>
/// Interface for propagating span context.
/// </summary>
public interface ISpanContextPropagator
{
    /// <summary>
    /// Injects span context into headers.
    /// </summary>
    void Inject(ICorrelationContext context, IDictionary<string, string> headers);
    
    /// <summary>
    /// Extracts span context from headers.
    /// </summary>
    ICorrelationContext? Extract(IDictionary<string, string> headers);
}

/// <summary>
/// Span context propagator using W3C Trace Context format.
/// </summary>
public class SpanContextPropagator : ISpanContextPropagator
{
    public const string TraceParentHeader = "traceparent";
    public const string TraceStateHeader = "tracestate";
    
    /// <inheritdoc />
    public void Inject(ICorrelationContext context, IDictionary<string, string> headers)
    {
        // W3C traceparent format: {version}-{trace-id}-{parent-id}-{trace-flags}
        var traceParent = $"00-{context.TraceId}-{context.SpanId}-01";
        headers[TraceParentHeader] = traceParent;
        
        // Add correlation ID
        headers[CorrelationIdMiddleware.CorrelationIdHeader] = context.CorrelationId;
        
        // Add baggage items
        foreach (var item in context.Baggage)
        {
            headers[$"baggage-{item.Key}"] = item.Value.ToString() ?? "";
        }
    }
    
    /// <inheritdoc />
    public ICorrelationContext? Extract(IDictionary<string, string> headers)
    {
        if (!headers.TryGetValue(TraceParentHeader, out var traceParent))
        {
            return null;
        }
        
        var parts = traceParent.Split('-');
        if (parts.Length < 4)
        {
            return null;
        }
        
        var context = new CorrelationContext
        {
            TraceId = parts[1],
            ParentSpanId = parts[2],
            SpanId = GenerateSpanId(),
            StartTime = DateTime.UtcNow
        };
        
        // Extract correlation ID
        if (headers.TryGetValue(CorrelationIdMiddleware.CorrelationIdHeader, out var correlationId))
        {
            context.CorrelationId = correlationId;
        }
        else
        {
            context.CorrelationId = Guid.NewGuid().ToString();
        }
        
        // Extract baggage
        foreach (var header in headers.Where(h => h.Key.StartsWith("baggage-")))
        {
            var key = header.Key[8..]; // Remove "baggage-" prefix
            context.Baggage[key] = header.Value;
        }
        
        return context;
    }
    
    private static string GenerateSpanId()
    {
        return Guid.NewGuid().ToString("N")[..16];
    }
}

/// <summary>
/// Extension methods for HTTP client tracing.
/// </summary>
public static class HttpClientTracingExtensions
{
    /// <summary>
    /// Adds tracing headers to an HTTP request.
    /// </summary>
    public static HttpRequestMessage WithTracingHeaders(
        this HttpRequestMessage request,
        ICorrelationContext context,
        ISpanContextPropagator propagator)
    {
        var headers = new Dictionary<string, string>();
        propagator.Inject(context, headers);
        
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        
        return request;
    }
}
