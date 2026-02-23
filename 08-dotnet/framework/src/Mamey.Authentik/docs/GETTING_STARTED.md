# Getting Started with Mamey.Authentik

This guide will help you get started with the Mamey.Authentik library.

## Prerequisites

- .NET 9.0 or later
- An Authentik instance (or access to one)
- An API token or OAuth2 client credentials

## Installation

Install the package via NuGet:

```bash
dotnet add package Mamey.Authentik
```

Or via Package Manager:

```
Install-Package Mamey.Authentik
```

## Basic Setup

### 1. Configure Services

In your `Program.cs` or `Startup.cs`:

```csharp
using Mamey.Authentik;

var builder = WebApplication.CreateBuilder(args);

// Add Authentik services
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token-here";
});

var app = builder.Build();
```

### 2. Use the Client

Inject `IAuthentikClient` into your services:

```csharp
public class UserService
{
    private readonly IAuthentikClient _authentik;

    public UserService(IAuthentikClient authentik)
    {
        _authentik = authentik;
    }

    public async Task DoSomethingAsync()
    {
        // Use Authentik client
        // Methods will be available after code generation
    }
}
```

## Authentication Methods

### API Token Authentication

The simplest method - use an API token:

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ApiToken = "your-api-token";
});
```

### OAuth2 Client Credentials

For OAuth2 authentication:

```csharp
builder.Services.AddAuthentik(options =>
{
    options.BaseUrl = "https://authentik.company.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    // Optional: custom token endpoint
    options.TokenEndpoint = "https://authentik.company.com/application/o/token/";
});
```

## Configuration via appsettings.json

You can also configure via `appsettings.json`:

```json
{
  "Authentik": {
    "BaseUrl": "https://authentik.company.com",
    "ApiToken": "your-api-token",
    "Timeout": "00:01:00",
    "ValidateSsl": true,
    "RetryPolicy": {
      "MaxRetries": 3,
      "InitialDelay": "00:00:01",
      "MaxDelay": "00:00:30",
      "UseExponentialBackoff": true
    },
    "CacheOptions": {
      "Enabled": true,
      "DefaultTtl": "00:05:00",
      "MetadataTtl": "01:00:00"
    },
    "LogLevel": "Information"
  }
}
```

Then register:

```csharp
builder.Services.AddAuthentik(builder.Configuration.GetSection("Authentik"));
```

## Next Steps

- See [API Reference](API_REFERENCE.md) for detailed API documentation
- Check [Examples](EXAMPLES.md) for more usage examples
- Review error handling patterns in the main README
