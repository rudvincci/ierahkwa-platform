# Mamey.Authentik Examples

## Table of Contents

1. [Basic Setup](#basic-setup)
2. [User Management](#user-management)
3. [OAuth2 Operations](#oauth2-operations)
4. [Error Handling](#error-handling)
5. [Caching](#caching)
6. [Advanced Configuration](#advanced-configuration)

## Basic Setup

### Minimal Configuration

```csharp
using Mamey.Authentik;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
});

var app = builder.Build();
```

### Using Configuration File

```csharp
// appsettings.json
{
  "Authentik": {
    "BaseUrl": "https://authentik.company.com",
    "ApiToken": "your-api-token",
    "Timeout": "00:01:00"
  }
}

// Program.cs
builder.Services.AddAuthentik(builder.Configuration.GetSection("Authentik"));
```

## User Management

### Get User by ID

```csharp
public class UserService
{
    private readonly IAuthentikClient _authentik;

    public UserService(IAuthentikClient authentik)
    {
        _authentik = authentik;
    }

    public async Task<object?> GetUserAsync(string userId)
    {
        try
        {
            return await _authentik.Core.GetUserAsync(userId);
        }
        catch (Exceptions.AuthentikNotFoundException)
        {
            // User not found
            return null;
        }
    }
}
```

### List Users with Pagination

```csharp
public async Task<PaginatedResult<object>> GetUsersAsync(int page = 1, int pageSize = 20)
{
    var result = await _authentik.Core.ListUsersAsync(page, pageSize);
    
    // Access pagination info
    if (result.Pagination.HasNext)
    {
        // Load next page
    }
    
    return result;
}
```

## OAuth2 Operations

### Using OAuth2 Client Credentials

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    // Token will be automatically fetched and refreshed
});
```

### Custom Token Endpoint

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.TokenEndpoint = "https://authentik.company.com/custom/token/endpoint";
});
```

## Error Handling

### Comprehensive Error Handling

```csharp
public async Task<object?> GetUserSafelyAsync(string userId)
{
    try
    {
        return await _authentik.Core.GetUserAsync(userId);
    }
    catch (Exceptions.AuthentikValidationException ex)
    {
        // Handle validation errors
        foreach (var error in ex.ValidationErrors ?? new Dictionary<string, string[]>())
        {
            _logger.LogWarning("Validation error for {Key}: {Errors}", 
                error.Key, string.Join(", ", error.Value));
        }
        return null;
    }
    catch (Exceptions.AuthentikAuthenticationException ex)
    {
        // Handle authentication errors
        _logger.LogError("Authentication failed: {Message}", ex.Message);
        throw;
    }
    catch (Exceptions.AuthentikAuthorizationException ex)
    {
        // Handle authorization errors
        _logger.LogWarning("Access forbidden: {Message}", ex.Message);
        return null;
    }
    catch (Exceptions.AuthentikNotFoundException ex)
    {
        // Handle not found
        _logger.LogInformation("User {UserId} not found", userId);
        return null;
    }
    catch (Exceptions.AuthentikRateLimitException ex)
    {
        // Handle rate limiting
        if (ex.RetryAfterSeconds.HasValue)
        {
            _logger.LogWarning("Rate limited. Retry after {Seconds} seconds", 
                ex.RetryAfterSeconds.Value);
            await Task.Delay(TimeSpan.FromSeconds(ex.RetryAfterSeconds.Value));
            // Retry the request
        }
        throw;
    }
    catch (Exceptions.AuthentikApiException ex)
    {
        // Handle other API errors
        _logger.LogError("API error {StatusCode}: {Message}", ex.StatusCode, ex.Message);
        throw;
    }
}
```

## Caching

### Enable Caching

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
    
    // Configure caching
    options.CacheOptions.Enabled = true;
    options.CacheOptions.DefaultTtl = TimeSpan.FromMinutes(10);
    options.CacheOptions.MetadataTtl = TimeSpan.FromHours(1);
});
```

### Using Distributed Cache (Redis)

```csharp
// Add Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// Add Authentik with distributed cache
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
});

builder.Services.AddAuthentikDistributedCache();
```

### Manual Cache Management

```csharp
public class CachedUserService
{
    private readonly IAuthentikClient _authentik;
    private readonly IAuthentikCache _cache;

    public CachedUserService(IAuthentikClient authentik, IAuthentikCache cache)
    {
        _authentik = authentik;
        _cache = cache;
    }

    public async Task<object?> GetUserAsync(string userId)
    {
        var cacheKey = $"user:{userId}";
        
        // Try cache first
        var cached = await _cache.GetAsync<object>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        // Fetch from API
        var user = await _authentik.Core.GetUserAsync(userId);
        
        // Cache the result
        if (user != null)
        {
            await _cache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(10));
        }

        return user;
    }

    public async Task InvalidateUserCacheAsync(string userId)
    {
        await _cache.RemoveAsync($"user:{userId}");
    }
}
```

## Advanced Configuration

### Fluent Configuration

```csharp
builder.Services.AddAuthentik(options =>
{
    new AuthentikOptionsBuilder()
        .WithBaseUrl("https://authentik.company.com")
        .WithApiToken("your-api-token")
        .WithTimeout(TimeSpan.FromSeconds(60))
        .WithSslValidation(true)
        .WithRetryPolicy(policy =>
        {
            policy.MaxRetries = 5;
            policy.InitialDelay = TimeSpan.FromSeconds(2);
            policy.MaxDelay = TimeSpan.FromSeconds(30);
            policy.UseExponentialBackoff = true;
        })
        .WithCache(cache =>
        {
            cache.Enabled = true;
            cache.DefaultTtl = TimeSpan.FromMinutes(10);
            cache.MetadataTtl = TimeSpan.FromHours(1);
        })
        .WithLogLevel(LogLevel.Debug)
        .Build();
});
```

### Custom Logging

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
    options.LogLevel = LogLevel.Debug; // Log all requests/responses
});
```

### Disable Caching

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
    options.CacheOptions.Enabled = false;
});

// Or use fluent API
builder.Services.AddAuthentik(options =>
{
    new AuthentikOptionsBuilder()
        .WithBaseUrl("https://authentik.company.com")
        .WithApiToken("your-api-token")
        .WithoutCache()
        .Build();
});
```

## Best Practices

1. **Always use dependency injection** - Register services via `AddAuthentik()`
2. **Handle exceptions appropriately** - Use specific exception types for different error scenarios
3. **Enable caching for read-heavy operations** - Improves performance and reduces API calls
4. **Configure retry policies** - Handle transient failures gracefully
5. **Use structured logging** - Leverage the built-in logging for debugging
6. **Validate configuration** - Options are validated on registration, but check in production
7. **Monitor rate limits** - Handle `AuthentikRateLimitException` and respect retry-after headers
