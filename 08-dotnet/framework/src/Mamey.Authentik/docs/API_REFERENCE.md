# Mamey.Authentik API Reference

## Namespaces

- `Mamey.Authentik` - Main library namespace
- `Mamey.Authentik.Services` - Service interfaces and implementations
- `Mamey.Authentik.Models` - Data models and DTOs
- `Mamey.Authentik.Exceptions` - Exception types
- `Mamey.Authentik.Caching` - Caching interfaces and implementations
- `Mamey.Authentik.Handlers` - HTTP message handlers
- `Mamey.Authentik.Policies` - Retry and circuit breaker policies

## Core Types

### IAuthentikClient

Main client interface providing access to all Authentik services.

```csharp
public interface IAuthentikClient
{
    IAuthentikAdminService Admin { get; }
    IAuthentikCoreService Core { get; }
    IAuthentikOAuth2Service OAuth2 { get; }
    IAuthentikFlowsService Flows { get; }
    IAuthentikPoliciesService Policies { get; }
    IAuthentikProvidersService Providers { get; }
    IAuthentikStagesService Stages { get; }
    IAuthentikSourcesService Sources { get; }
    IAuthentikEventsService Events { get; }
}
```

### AuthentikOptions

Configuration options for the Authentik client.

**Properties:**
- `string BaseUrl` - Base URL of Authentik instance (required)
- `string? ApiToken` - API token for authentication
- `string? ClientId` - OAuth2 client ID
- `string? ClientSecret` - OAuth2 client secret
- `TimeSpan Timeout` - HTTP request timeout (default: 30 seconds)
- `bool ValidateSsl` - Validate SSL certificates (default: true)
- `RetryPolicyOptions RetryPolicy` - Retry policy configuration
- `CacheOptions CacheOptions` - Cache configuration
- `LogLevel LogLevel` - Logging level (default: Information)

### AuthentikOptionsBuilder

Fluent builder for configuring Authentik options.

**Methods:**
- `WithBaseUrl(string)` - Set base URL
- `WithApiToken(string)` - Set API token
- `WithOAuth2Credentials(string, string)` - Set OAuth2 credentials
- `WithTimeout(TimeSpan)` - Set timeout
- `WithRetryPolicy(Action<RetryPolicyOptions>)` - Configure retry policy
- `WithCache(Action<CacheOptions>)` - Configure caching
- `WithoutCache()` - Disable caching
- `WithLogLevel(LogLevel)` - Set logging level
- `Build()` - Build and validate options

## Services

### IAuthentikCoreService

Core service for Authentik operations.

**Methods:**
- `Task<object?> GetUserAsync(string userId, CancellationToken cancellationToken = default)`
- `Task<PaginatedResult<object>> ListUsersAsync(int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)`

*Note: After code generation, these methods will be expanded with actual Authentik API methods.*

## Exceptions

### AuthentikException

Base exception for all Authentik-related errors.

### AuthentikApiException

General API exception with status code and response details.

**Properties:**
- `int StatusCode` - HTTP status code
- `string? ResponseBody` - Response body
- `string? RequestUri` - Request URI

### AuthentikAuthenticationException

Thrown on 401 Unauthorized errors.

### AuthentikAuthorizationException

Thrown on 403 Forbidden errors.

### AuthentikNotFoundException

Thrown on 404 Not Found errors.

**Properties:**
- `string? ResourceId` - Resource identifier that was not found

### AuthentikValidationException

Thrown on 400 Bad Request errors.

**Properties:**
- `Dictionary<string, string[]>? ValidationErrors` - Validation error details

### AuthentikRateLimitException

Thrown on 429 Too Many Requests errors.

**Properties:**
- `int? RetryAfterSeconds` - Retry after value in seconds

## Caching

### IAuthentikCache

Interface for caching Authentik API responses.

**Methods:**
- `Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)`
- `Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)`
- `Task RemoveAsync(string key, CancellationToken cancellationToken = default)`
- `Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)`
- `Task ClearAsync(CancellationToken cancellationToken = default)`

### InMemoryAuthentikCache

In-memory cache implementation using `IMemoryCache`.

### DistributedAuthentikCache

Distributed cache implementation using `IDistributedCache` (e.g., Redis).

## Extension Methods

### AddAuthentik

Registers Authentik services in the dependency injection container.

**Overloads:**
- `AddAuthentik(IServiceCollection, Action<AuthentikOptions>)`
- `AddAuthentik(IServiceCollection, IConfiguration)`

### AddAuthentikDistributedCache

Adds distributed cache support for Authentik.

**Usage:**
```csharp
services.AddStackExchangeRedisCache(options => { /* ... */ });
services.AddAuthentik(options => { /* ... */ });
services.AddAuthentikDistributedCache();
```

## Models

### PaginatedResult<T>

Represents a paginated result from Authentik API.

**Properties:**
- `List<T> Results` - List of results
- `Pagination Pagination` - Pagination information

### Pagination

Pagination information.

**Properties:**
- `int Count` - Total count of items
- `string? Next` - URL for next page
- `string? Previous` - URL for previous page
- `bool HasNext` - Whether there is a next page
- `bool HasPrevious` - Whether there is a previous page

### AuthentikResponse<T>

Base response wrapper for Authentik API responses.

**Properties:**
- `T? Data` - Response data
- `bool Success` - Whether request was successful
- `string? ErrorMessage` - Error message, if any
