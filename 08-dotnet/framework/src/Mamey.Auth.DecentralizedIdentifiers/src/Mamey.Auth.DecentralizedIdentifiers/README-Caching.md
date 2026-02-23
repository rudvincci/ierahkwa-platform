# DID Authentication Caching System

## Overview

The DID Authentication Caching System provides comprehensive caching capabilities for Decentralized Identifiers (DIDs), Verifiable Credentials (VCs), and authentication results. It supports multiple storage backends including in-memory caching, Redis distributed caching, and hybrid approaches for optimal performance and scalability.

## Architecture

### Cache Types

1. **DID Document Cache** - Caches resolved DID documents with configurable TTL
2. **Verification Result Cache** - Caches cryptographic verification results
3. **Credential Status Cache** - Caches credential revocation/status information

### Storage Backends

1. **Memory Cache** - Fast in-process caching using `IMemoryCache`
2. **Redis Cache** - Distributed caching using `Mamey.Persistence.Redis`
3. **Hybrid Cache** - Combines memory and Redis for optimal performance
4. **No-Op Cache** - Disabled caching for testing or when caching is not needed

## Features

### Core Features

- **Multi-Level Caching** - Memory + Redis for optimal performance
- **Configurable TTL** - Different expiration times for different data types
- **Cache Statistics** - Hit/miss ratios and performance metrics
- **Automatic Expiration** - TTL-based cache invalidation
- **Batch Operations** - Efficient bulk get/set operations
- **Error Handling** - Graceful fallback when cache operations fail

### Advanced Features

- **Cache Warming** - Pre-populate frequently accessed data
- **Cache Invalidation** - Manual cache invalidation for specific DIDs
- **Statistics Tracking** - Detailed cache performance metrics
- **Memory Optimization** - Efficient memory usage patterns
- **Redis Integration** - Full integration with Mamey.Persistence.Redis

## Configuration

### Basic Configuration

```json
{
  "didAuth": {
    "cacheOptions": {
      "enabled": true,
      "ttlMinutes": 60,
      "storageType": "Hybrid",
      "redisConnectionString": "localhost:6379"
    }
  }
}
```

### Advanced Configuration

```json
{
  "didAuth": {
    "cacheOptions": {
      "enabled": true,
      "ttlMinutes": 60,
      "storageType": "Hybrid",
      "redisConnectionString": "localhost:6379"
    }
  },
  "redis": {
    "connectionString": "localhost:6379",
    "instance": "did-cache",
    "database": 0
  }
}
```

### Storage Types

- **Memory** - In-process caching only
- **Redis** - Distributed caching only
- **Hybrid** - Memory + Redis (recommended)
- **None** - Caching disabled

## Usage Examples

### Basic Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddRedis() // Add Redis support
    .AddDecentralizedIdentifiers()
    .AddDidAuth(); // Includes caching

var app = builder.Build();
app.UseDid();
```

### Custom Cache Configuration

```csharp
builder.Services.Configure<DidAuthOptions>(options =>
{
    options.CacheOptions.Enabled = true;
    options.CacheOptions.StorageType = "Hybrid";
    options.CacheOptions.TtlMinutes = 120;
    options.CacheOptions.RedisConnectionString = "redis:6379";
});
```

### Direct Cache Usage

```csharp
public class DidService
{
    private readonly IDidDocumentCache _didCache;
    private readonly IVerificationResultCache _verificationCache;
    private readonly ICredentialStatusCache _statusCache;

    public DidService(
        IDidDocumentCache didCache,
        IVerificationResultCache verificationCache,
        ICredentialStatusCache statusCache)
    {
        _didCache = didCache;
        _verificationCache = verificationCache;
        _statusCache = statusCache;
    }

    public async Task<DidDocument> ResolveDidAsync(string did)
    {
        // Try cache first
        var cachedDoc = await _didCache.GetAsync(did);
        if (cachedDoc != null)
        {
            return cachedDoc;
        }

        // Resolve from source
        var document = await ResolveFromSourceAsync(did);
        
        // Cache the result
        await _didCache.SetAsync(did, document, 60);
        
        return document;
    }

    public async Task<bool> VerifyPresentationAsync(string presentation)
    {
        var cacheKey = $"vp:{presentation.GetHashCode()}";
        
        // Check cache first
        var cachedResult = await _verificationCache.GetAsync(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult.IsValid;
        }

        // Verify presentation
        var isValid = await VerifyFromSourceAsync(presentation);
        
        // Cache result
        await _verificationCache.SetAsync(cacheKey, new VerificationResult
        {
            IsValid = isValid,
            VerifiedAt = DateTime.UtcNow
        }, 30);
        
        return isValid;
    }
}
```

### Cache Statistics

```csharp
public class CacheMonitoringService
{
    private readonly IDidDocumentCache _didCache;

    public async Task<CacheStatistics> GetCacheStatsAsync()
    {
        return await _didCache.GetStatisticsAsync();
    }

    public async Task LogCachePerformanceAsync()
    {
        var stats = await GetCacheStatsAsync();
        
        _logger.LogInformation("Cache Performance - Hits: {Hits}, Misses: {Misses}, Hit Ratio: {HitRatio:P2}",
            stats.HitCount, stats.MissCount, stats.HitRatio);
    }
}
```

### Batch Operations

```csharp
public async Task<Dictionary<string, DidDocument>> ResolveMultipleDidsAsync(string[] dids)
{
    // Try to get all from cache first
    var cachedDocs = await _didCache.GetManyAsync(dids);
    
    // Find missing DIDs
    var missingDids = dids.Except(cachedDocs.Keys).ToArray();
    
    if (missingDids.Any())
    {
        // Resolve missing DIDs
        var resolvedDocs = await ResolveFromSourceAsync(missingDids);
        
        // Cache the new results
        await _didCache.SetManyAsync(resolvedDocs, 60);
        
        // Merge results
        foreach (var kvp in resolvedDocs)
        {
            cachedDocs[kvp.Key] = kvp.Value;
        }
    }
    
    return cachedDocs;
}
```

## Performance Considerations

### Memory Usage

- **Memory Cache**: Limited by available RAM
- **Redis Cache**: Limited by Redis memory configuration
- **Hybrid**: Combines both for optimal performance

### TTL Recommendations

- **DID Documents**: 60-120 minutes (relatively stable)
- **Verification Results**: 15-30 minutes (shorter for security)
- **Credential Status**: 5-15 minutes (frequently changing)

### Cache Warming

```csharp
public async Task WarmCacheAsync(string[] frequentlyUsedDids)
{
    var tasks = frequentlyUsedDids.Select(async did =>
    {
        var document = await ResolveFromSourceAsync(did);
        await _didCache.SetAsync(did, document, 120);
    });
    
    await Task.WhenAll(tasks);
}
```

## Monitoring and Diagnostics

### Cache Statistics

```csharp
public class CacheDiagnostics
{
    public async Task<CacheHealth> CheckCacheHealthAsync()
    {
        var didStats = await _didCache.GetStatisticsAsync();
        var verificationStats = await _verificationCache.GetStatisticsAsync();
        var statusStats = await _statusCache.GetStatisticsAsync();
        
        return new CacheHealth
        {
            DidCacheHitRatio = didStats.HitRatio,
            VerificationCacheHitRatio = verificationStats.HitRatio,
            StatusCacheHitRatio = statusStats.HitRatio,
            TotalEntries = didStats.TotalEntries + verificationStats.TotalEntries + statusStats.TotalEntries,
            LastAccessed = DateTime.UtcNow
        };
    }
}
```

### Health Checks

```csharp
public class CacheHealthCheck : IHealthCheck
{
    private readonly IDidDocumentCache _didCache;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = await _didCache.GetStatisticsAsync();
            
            if (stats.HitRatio < 0.5) // Less than 50% hit ratio
            {
                return HealthCheckResult.Degraded("Low cache hit ratio");
            }
            
            return HealthCheckResult.Healthy($"Cache healthy - Hit ratio: {stats.HitRatio:P2}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache health check failed", ex);
        }
    }
}
```

## Error Handling

### Graceful Degradation

```csharp
public async Task<DidDocument> ResolveWithFallbackAsync(string did)
{
    try
    {
        // Try cache first
        var cachedDoc = await _didCache.GetAsync(did);
        if (cachedDoc != null)
        {
            return cachedDoc;
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Cache operation failed for DID {Did}, falling back to direct resolution", did);
    }

    // Fallback to direct resolution
    return await ResolveFromSourceAsync(did);
}
```

### Cache Invalidation

```csharp
public async Task InvalidateDidCacheAsync(string did)
{
    try
    {
        await _didCache.InvalidateAsync(did);
        _logger.LogInformation("Invalidated cache for DID: {Did}", did);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to invalidate cache for DID: {Did}", did);
    }
}
```

## Testing

### Unit Tests

```csharp
public class DidDocumentCacheTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnCachedDocument()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new MemoryDidDocumentCache(memoryCache, Mock.Of<ILogger<MemoryDidDocumentCache>>());
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var document = new DidDocument { Id = did };

        // Act
        await cache.SetAsync(did, document, 60);
        var result = await cache.GetAsync(did);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(did, result.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNullForExpiredDocument()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new MemoryDidDocumentCache(memoryCache, Mock.Of<ILogger<MemoryDidDocumentCache>>());
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var document = new DidDocument { Id = did };

        // Act
        await cache.SetAsync(did, document, -1); // Expired immediately
        var result = await cache.GetAsync(did);

        // Assert
        Assert.Null(result);
    }
}
```

### Integration Tests

```csharp
public class CacheIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    [Fact]
    public async Task ShouldCacheDidDocumentAcrossRequests()
    {
        // Arrange
        var client = _factory.CreateClient();
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act - First request
        var response1 = await client.GetAsync($"/api/dids/{did}");
        response1.EnsureSuccessStatusCode();

        // Act - Second request (should be cached)
        var response2 = await client.GetAsync($"/api/dids/{did}");
        response2.EnsureSuccessStatusCode();

        // Assert - Both requests should succeed
        Assert.True(response1.IsSuccessStatusCode);
        Assert.True(response2.IsSuccessStatusCode);
    }
}
```

## Best Practices

### 1. Cache Key Design

- Use consistent naming conventions
- Include version information for schema changes
- Avoid special characters that might cause issues

### 2. TTL Configuration

- Set appropriate TTL based on data volatility
- Use shorter TTL for security-sensitive data
- Consider sliding expiration for frequently accessed data

### 3. Error Handling

- Always implement fallback mechanisms
- Log cache failures for monitoring
- Don't let cache failures break core functionality

### 4. Memory Management

- Monitor memory usage in production
- Set appropriate cache size limits
- Use eviction policies for memory pressure

### 5. Performance Monitoring

- Track cache hit/miss ratios
- Monitor cache operation latency
- Set up alerts for cache failures

## Troubleshooting

### Common Issues

1. **High Memory Usage**
   - Reduce cache TTL
   - Implement cache size limits
   - Use Redis for distributed caching

2. **Low Hit Ratio**
   - Review cache key design
   - Increase TTL for stable data
   - Implement cache warming

3. **Redis Connection Issues**
   - Check Redis server status
   - Verify connection string
   - Implement fallback to memory cache

4. **Cache Inconsistency**
   - Implement proper invalidation
   - Use atomic operations
   - Consider cache versioning

### Debug Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Mamey.Auth.DecentralizedIdentifiers.Caching": "Debug"
    }
  }
}
```

## Migration Guide

### From Basic to Advanced Caching

1. **Enable Redis Support**
   ```csharp
   builder.Services.AddMamey().AddRedis();
   ```

2. **Configure Hybrid Caching**
   ```json
   {
     "didAuth": {
       "cacheOptions": {
         "storageType": "Hybrid"
       }
     }
   }
   ```

3. **Add Cache Monitoring**
   ```csharp
   builder.Services.AddHealthChecks()
       .AddCheck<CacheHealthCheck>("did-cache");
   ```

### Performance Optimization

1. **Implement Cache Warming**
2. **Optimize TTL Settings**
3. **Add Cache Statistics**
4. **Implement Health Checks**
5. **Add Performance Monitoring**

