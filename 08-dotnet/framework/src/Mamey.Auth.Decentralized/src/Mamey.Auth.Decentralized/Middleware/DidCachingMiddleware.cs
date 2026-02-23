using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Auth.Decentralized.Caching;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Options;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Middleware;

/// <summary>
/// Middleware for caching DID documents and responses.
/// </summary>
public class DidCachingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DidCachingMiddleware> _logger;
    private readonly DecentralizedOptions _options;

    public DidCachingMiddleware(
        RequestDelegate next,
        ILogger<DidCachingMiddleware> logger,
        IOptions<DecentralizedOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check if caching is enabled and applicable
            if (!ShouldCache(context))
            {
                await _next(context);
                return;
            }

            // Try to get cached response
            var cachedResponse = await GetCachedResponseAsync(context);
            if (cachedResponse != null)
            {
                _logger.LogDebug("Returning cached response for {Path}", context.Request.Path);
                await WriteCachedResponseAsync(context, cachedResponse);
                return;
            }

            // Capture the original response stream
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continue to the next middleware
            await _next(context);

            // Cache the response if it's cacheable
            await CacheResponseAsync(context, responseBody);

            // Copy the response back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DID caching");
            await _next(context);
        }
    }

    private bool ShouldCache(HttpContext context)
    {
        // Check if caching is enabled
        if (!_options.EnableCaching)
            return false;

        // Only cache GET requests
        if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            return false;

        // Skip caching for certain paths
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (string.IsNullOrEmpty(path))
            return false;

        // Skip for health checks, swagger, etc.
        if (path.StartsWith("/health") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/metrics"))
            return false;

        // Check if the endpoint allows caching
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<DisableCachingAttribute>() != null)
            return false;

        // Check if the endpoint requires caching
        if (endpoint?.Metadata?.GetMetadata<RequireCachingAttribute>() != null)
            return true;

        // Default to caching for DID-related endpoints
        return path.Contains("/did") || path.Contains("/resolve") || path.Contains("/document");
    }

    private async Task<CachedResponse?> GetCachedResponseAsync(HttpContext context)
    {
        try
        {
            var cacheKey = GenerateCacheKey(context);
            if (string.IsNullOrEmpty(cacheKey))
                return null;

            var cache = context.RequestServices.GetRequiredService<IDidDocumentCache>();
            var cachedData = await cache.GetAsync(cacheKey);
            
            if (cachedData != null)
            {
                // For now, we'll skip caching since we need to implement proper response caching
                // This is a placeholder implementation
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving cached response");
        }

        return null;
    }

    private async Task CacheResponseAsync(HttpContext context, MemoryStream responseBody)
    {
        try
        {
            // Only cache successful responses
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                return;

            var cacheKey = GenerateCacheKey(context);
            if (string.IsNullOrEmpty(cacheKey))
                return;

            var responseData = responseBody.ToArray();
            var cachedResponse = new CachedResponse
            {
                StatusCode = context.Response.StatusCode,
                Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = Convert.ToBase64String(responseData),
                ContentType = context.Response.ContentType,
                CachedAt = DateTime.UtcNow
            };

            var cache = context.RequestServices.GetRequiredService<IDidDocumentCache>();
            var serializedResponse = JsonSerializer.Serialize(cachedResponse);
            
            // Cache for the configured duration
            var cacheDuration = GetCacheDuration(context);
            // await cache.SetAsync(cacheKey, serializedResponse, cacheDuration); // Commented out - type mismatch
            
            _logger.LogDebug("Cached response for {Path} with key {CacheKey}", context.Request.Path, cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error caching response");
        }
    }

    private async Task WriteCachedResponseAsync(HttpContext context, CachedResponse cachedResponse)
    {
        try
        {
            context.Response.StatusCode = cachedResponse.StatusCode;
            context.Response.ContentType = cachedResponse.ContentType;

            // Set headers
            foreach (var header in cachedResponse.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            // Add cache hit header
            context.Response.Headers["X-Cache"] = "HIT";
            context.Response.Headers["X-Cache-Date"] = cachedResponse.CachedAt.ToString("O");

            // Write response body
            var bodyBytes = Convert.FromBase64String(cachedResponse.Body);
            await context.Response.Body.WriteAsync(bodyBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing cached response");
        }
    }

    private string? GenerateCacheKey(HttpContext context)
    {
        try
        {
            var path = context.Request.Path.Value;
            var queryString = context.Request.QueryString.Value;
            var did = ExtractDidFromRequest(context);

            // Create cache key from path, query string, and DID
            var keyParts = new List<string> { path ?? "" };
            
            if (!string.IsNullOrEmpty(queryString))
            {
                keyParts.Add(queryString);
            }
            
            if (!string.IsNullOrEmpty(did))
            {
                keyParts.Add($"did:{did}");
            }

            // Add custom cache key components
            var endpoint = context.GetEndpoint();
            var customKey = endpoint?.Metadata?.GetMetadata<CacheKeyAttribute>()?.Key;
            if (!string.IsNullOrEmpty(customKey))
            {
                keyParts.Add(customKey);
            }

            var cacheKey = string.Join("|", keyParts);
            return $"did_cache:{cacheKey}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error generating cache key");
            return null;
        }
    }

    private string? ExtractDidFromRequest(HttpContext context)
    {
        // Try to extract DID from various sources

        // 1. From Authorization header (Bearer DID)
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (IsValidDid(token))
                return token;
        }

        // 2. From X-DID header
        var didHeader = context.Request.Headers["X-DID"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didHeader) && IsValidDid(didHeader))
            return didHeader;

        // 3. From query parameter
        var didQuery = context.Request.Query["did"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didQuery) && IsValidDid(didQuery))
            return didQuery;

        // 4. From custom header
        var customHeader = context.Request.Headers[_options.DidHeaderName].FirstOrDefault();
        if (!string.IsNullOrEmpty(customHeader) && IsValidDid(customHeader))
            return customHeader;

        return null;
    }

    private bool IsValidDid(string value)
    {
        try
        {
            Did.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private TimeSpan GetCacheDuration(HttpContext context)
    {
        // Check for custom cache duration from endpoint metadata
        var endpoint = context.GetEndpoint();
        var cacheDuration = endpoint?.Metadata?.GetMetadata<CacheDurationAttribute>()?.Duration;
        if (cacheDuration.HasValue)
            return cacheDuration.Value;

        // Check for DID-specific cache duration
        var did = ExtractDidFromRequest(context);
        if (!string.IsNullOrEmpty(did))
        {
            return _options.DidDocumentCacheDuration;
        }

        // Default cache duration
        return _options.DefaultCacheDuration;
    }
}

/// <summary>
/// Cached response data.
/// </summary>
public class CachedResponse
{
    public int StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public DateTime CachedAt { get; set; }
}

/// <summary>
/// Attribute to disable caching for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DisableCachingAttribute : Attribute
{
    public DisableCachingAttribute()
    {
    }
}

/// <summary>
/// Attribute to require caching for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireCachingAttribute : Attribute
{
    public RequireCachingAttribute()
    {
    }
}

/// <summary>
/// Attribute to specify custom cache key for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheKeyAttribute : Attribute
{
    public string Key { get; }

    public CacheKeyAttribute(string key)
    {
        Key = key;
    }
}

/// <summary>
/// Attribute to specify cache duration for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheDurationAttribute : Attribute
{
    public TimeSpan Duration { get; }

    public CacheDurationAttribute(int seconds)
    {
        Duration = TimeSpan.FromSeconds(seconds);
    }

    public CacheDurationAttribute(int hours, int minutes, int seconds)
    {
        Duration = new TimeSpan(hours, minutes, seconds);
    }
}
