# Mamey.Authentik

A production-ready .NET client library for interacting with [Authentik](https://goauthentik.io/) Identity Provider's REST API.

## Features

- **Complete API Coverage**: Full support for all Authentik API endpoints
- **Type-Safe**: Generated from OpenAPI schema with strongly-typed models
- **Resilient**: Built-in retry policies and circuit breaker support
- **Caching**: Configurable caching for improved performance
- **Authentication**: Support for API tokens and OAuth2 client credentials
- **Logging**: Comprehensive logging with configurable levels
- **Error Handling**: Detailed exception hierarchy for better error handling

## Installation

```bash
dotnet add package Mamey.Authentik
```

## Quick Start

### Basic Configuration

```csharp
using Mamey.Authentik;

var builder = WebApplication.CreateBuilder(args);

// Add Authentik services
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
});

var app = builder.Build();

// Use Authentik client
var client = app.Services.GetRequiredService<IAuthentikClient>();
```

### Using Configuration File

```csharp
// appsettings.json
{
  "Authentik": {
    "BaseUrl": "https://authentik.company.com",
    "ApiToken": "your-api-token"
  }
}

// Program.cs
builder.Services.AddAuthentik(builder.Configuration.GetSection("Authentik"));
```

### OAuth2 Authentication

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
});
```

## Usage Examples

### User Management

```csharp
var client = serviceProvider.GetRequiredService<IAuthentikClient>();

// Get users (example - methods will be available after code generation)
// var users = await client.Core.GetUsersAsync();
```

### Fluent Configuration

```csharp
builder.Services.AddAuthentik(options =>
{
    options
        .WithBaseUrl("https://authentik.company.com")
        .WithApiToken("your-token")
        .WithTimeout(TimeSpan.FromSeconds(60))
        .WithRetryPolicy(policy =>
        {
            policy.MaxRetries = 5;
            policy.InitialDelay = TimeSpan.FromSeconds(2);
        })
        .WithCache(cache =>
        {
            cache.Enabled = true;
            cache.DefaultTtl = TimeSpan.FromMinutes(10);
        });
});
```

## Configuration Options

### AuthentikOptions

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `BaseUrl` | `string` | Base URL of Authentik instance | Required |
| `ApiToken` | `string?` | API token for authentication | Optional |
| `ClientId` | `string?` | OAuth2 client ID | Optional |
| `ClientSecret` | `string?` | OAuth2 client secret | Optional |
| `Timeout` | `TimeSpan` | HTTP request timeout | 30 seconds |
| `ValidateSsl` | `bool` | Validate SSL certificates | `true` |
| `RetryPolicy` | `RetryPolicyOptions` | Retry policy configuration | See below |
| `CacheOptions` | `CacheOptions` | Cache configuration | See below |
| `LogLevel` | `LogLevel` | Logging level | `Information` |

### Retry Policy Options

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `MaxRetries` | `int` | Maximum retry attempts | 3 |
| `InitialDelay` | `TimeSpan` | Initial delay between retries | 1 second |
| `MaxDelay` | `TimeSpan` | Maximum delay between retries | 30 seconds |
| `UseExponentialBackoff` | `bool` | Use exponential backoff | `true` |

### Cache Options

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `Enabled` | `bool` | Enable caching | `true` |
| `DefaultTtl` | `TimeSpan` | Default cache TTL | 5 minutes |
| `MetadataTtl` | `TimeSpan` | TTL for metadata endpoints | 1 hour |

## Error Handling

The library provides a comprehensive exception hierarchy:

- `AuthentikException` - Base exception
- `AuthentikApiException` - General API errors
- `AuthentikAuthenticationException` - 401 Unauthorized
- `AuthentikAuthorizationException` - 403 Forbidden
- `AuthentikNotFoundException` - 404 Not Found
- `AuthentikValidationException` - 400 Bad Request (includes validation errors)
- `AuthentikRateLimitException` - 429 Too Many Requests

Example:

```csharp
try
{
    // API call
}
catch (AuthentikValidationException ex)
{
    // Handle validation errors
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value)}");
    }
}
catch (AuthentikApiException ex)
{
    // Handle other API errors
    Console.WriteLine($"Error {ex.StatusCode}: {ex.Message}");
}
```

## Caching

The library supports both in-memory and distributed caching:

### In-Memory Cache (Default)

```csharp
// Already configured by default
builder.Services.AddAuthentik(options => { /* ... */ });
```

### Distributed Cache (Redis)

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

builder.Services.AddAuthentik(options => { /* ... */ });
builder.Services.AddAuthentikDistributedCache();
```

## Logging

Configure logging levels:

```csharp
builder.Services.AddAuthentik(options =>
{
    options.LogLevel = LogLevel.Debug; // Log all requests/responses
});
```

## Code Generation

The library uses code generation from Authentik's OpenAPI schema. To regenerate the client:

```bash
# Fetch latest schema
./scripts/update-schema.sh https://your-authentik-instance.com

# Generate client code
./scripts/generate-client.sh https://your-authentik-instance.com
```

## Contributing

Contributions are welcome! Please see our contributing guidelines.

## License

AGPL-3.0

## Support

For issues and questions, please open an issue on GitHub.
