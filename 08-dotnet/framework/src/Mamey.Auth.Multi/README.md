# Mamey.Auth.Multi

Multi-authentication coordinator library for the Mamey Framework. Coordinates JWT, DID, Azure, Identity, Distributed, and Certificate authentication methods with collision prevention and flexible policy support.

## Overview

`Mamey.Auth.Multi` provides a unified interface for managing multiple authentication methods in a single application. It coordinates authentication across different providers while preventing collisions through registry names and distinct scheme names. The library acts as a top-level coordinator that delegates to specialized coordinators (like `Mamey.Auth.Azure` for Azure workflows) when needed.

## Features

- **Multi-Authentication Support**: Coordinate JWT, DID, Azure, Identity, Distributed, and Certificate authentication
- **Collision Prevention**: Uses registry names and distinct scheme names to prevent duplicate registrations
- **Flexible Policies**: Supports various authentication policies (EitherOr, JwtOnly, PriorityOrder, AllRequired, etc.)
- **Azure Integration**: Delegates Azure authentication to `Mamey.Auth.Azure` coordinator
- **Configurable**: All authentication methods and schemes are configurable via `appsettings.json`
- **Production Ready**: Comprehensive unit and integration tests

## Installation

### NuGet Package
```bash
dotnet add package Mamey.Auth.Multi
```

### Project Reference
```xml
<ProjectReference Include="..\..\Mamey.Auth.Multi\src\Mamey.Auth.Multi\Mamey.Auth.Multi.csproj" />
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)
- Mamey.Auth.Jwt (for JWT authentication)
- Mamey.Auth.DecentralizedIdentifiers (for DID authentication)
- Mamey.Auth.Azure (for Azure authentication)
- Mamey.Auth.Identity (for Identity authentication - optional)
- Mamey.Auth.Distributed (for Distributed authentication - optional)

## Quick Start

### Basic Setup

```csharp
using Mamey.Auth.Multi;

var builder = WebApplication.CreateBuilder(args);

// Add multi-authentication
builder.Services
    .AddMamey()
    .AddMultiAuth();

var app = builder.Build();

// Use multi-authentication middleware
app.UseMamey()
    .UseMultiAuth();

app.Run();
```

### With Configuration

```csharp
using Mamey.Auth.Multi;

var builder = WebApplication.CreateBuilder(args);

// Add multi-authentication with custom section name
builder.Services
    .AddMamey()
    .AddMultiAuth("auth:multi");

var app = builder.Build();

app.UseMamey()
    .UseMultiAuth();

app.Run();
```

### With Options Object

```csharp
using Mamey.Auth.Multi;

var options = new MultiAuthOptions
{
    EnableJwt = true,
    EnableDid = true,
    EnableAzure = true,
    Policy = AuthenticationPolicy.EitherOr,
    JwtScheme = "Bearer",
    DidScheme = "DidBearer",
    AzureScheme = "AzureAD"
};

builder.Services
    .AddMamey()
    .AddMultiAuth(options);
```

## Configuration

### appsettings.json

```json
{
  "multiAuth": {
    "enabled": true,
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": true,
    "enableIdentity": false,
    "enableDistributed": false,
    "enableCertificate": false,
    "policy": "EitherOr",
    "jwtScheme": "Bearer",
    "didScheme": "DidBearer",
    "azureScheme": "AzureAD",
    "identityScheme": "Identity",
    "distributedScheme": "Distributed",
    "certificateScheme": "Certificate",
    "jwtSectionName": "jwt",
    "didSectionName": "dids",
    "azureSectionName": "azure",
    "identitySectionName": "identity",
    "distributedSectionName": "distributed",
    "certificateSectionName": "certificate"
  },
  "jwt": {
    "issuer": "https://your-issuer.com",
    "audience": "https://your-audience.com",
    "secretKey": "your-secret-key-at-least-32-characters-long"
  },
  "dids": {
    "enabled": true
  },
  "azure": {
    "enableAzure": false,
    "enableAzureB2B": false,
    "enableAzureB2C": true,
    "policy": "B2COnly"
  }
}
```

### Configuration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `enabled` | bool | `false` | Enable multi-authentication coordination |
| `enableJwt` | bool | `false` | Enable JWT authentication |
| `enableDid` | bool | `false` | Enable DID authentication |
| `enableAzure` | bool | `false` | Enable Azure authentication (delegates to Azure coordinator) |
| `enableIdentity` | bool | `false` | Enable Identity authentication |
| `enableDistributed` | bool | `false` | Enable Distributed authentication |
| `enableCertificate` | bool | `false` | Enable Certificate authentication |
| `policy` | string | `"EitherOr"` | Authentication policy (EitherOr, JwtOnly, DidOnly, AzureOnly, etc.) |
| `jwtScheme` | string | `"Bearer"` | JWT authentication scheme name |
| `didScheme` | string | `"DidBearer"` | DID authentication scheme name |
| `azureScheme` | string | `"AzureAD"` | Azure authentication scheme name |
| `identityScheme` | string | `"Identity"` | Identity authentication scheme name |
| `distributedScheme` | string | `"Distributed"` | Distributed authentication scheme name |
| `certificateScheme` | string | `"Certificate"` | Certificate authentication scheme name |
| `jwtSectionName` | string | `"jwt"` | Configuration section name for JWT options |
| `didSectionName` | string | `"dids"` | Configuration section name for DID options |
| `azureSectionName` | string | `"azure"` | Configuration section name for Azure options |
| `identitySectionName` | string | `"identity"` | Configuration section name for Identity options |
| `distributedSectionName` | string | `"distributed"` | Configuration section name for Distributed options |
| `certificateSectionName` | string | `"certificate"` | Configuration section name for Certificate options |

## Authentication Policies

### EitherOr (Default)
Tries authentication methods in order until one succeeds:
1. JWT
2. DID
3. Azure
4. Identity
5. Distributed
6. Certificate

**Use Case**: When you want to accept any valid authentication method.

```json
{
  "multiAuth": {
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": true,
    "policy": "EitherOr"
  }
}
```

### JwtOnly
Only JWT authentication is allowed.

**Use Case**: When you only want JWT token authentication.

```json
{
  "multiAuth": {
    "enableJwt": true,
    "policy": "JwtOnly"
  }
}
```

### DidOnly
Only DID authentication is allowed.

**Use Case**: When you only want decentralized identifier authentication.

```json
{
  "multiAuth": {
    "enableDid": true,
    "policy": "DidOnly"
  }
}
```

### AzureOnly
Only Azure authentication is allowed (delegates to Azure coordinator).

**Use Case**: When you only want Azure AD, B2B, or B2C authentication.

```json
{
  "multiAuth": {
    "enableAzure": true,
    "policy": "AzureOnly"
  },
  "azure": {
    "enableAzureB2C": true,
    "policy": "B2COnly"
  }
}
```

### IdentityOnly
Only Identity authentication is allowed.

**Use Case**: When you only want ASP.NET Core Identity authentication.

```json
{
  "multiAuth": {
    "enableIdentity": true,
    "policy": "IdentityOnly"
  }
}
```

### DistributedOnly
Only Distributed authentication is allowed.

**Use Case**: When you only want distributed token authentication.

```json
{
  "multiAuth": {
    "enableDistributed": true,
    "policy": "DistributedOnly"
  }
}
```

### CertificateOnly
Only Certificate authentication is allowed.

**Use Case**: When you only want certificate-based authentication.

```json
{
  "multiAuth": {
    "enableCertificate": true,
    "policy": "CertificateOnly"
  }
}
```

### PriorityOrder
Tries authentication methods in priority order (JWT first, then DID, then Azure, etc.).

**Use Case**: When you want to try methods in a specific order but accept any that succeeds.

```json
{
  "multiAuth": {
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": true,
    "policy": "PriorityOrder"
  }
}
```

### AllRequired
All enabled authentication methods must succeed (rare use case).

**Use Case**: When you need all authentication methods to validate a request (very rare).

```json
{
  "multiAuth": {
    "enableJwt": true,
    "enableDid": true,
    "policy": "AllRequired"
  }
}
```

## Collision Prevention Strategy

### Registry Names

Each authentication library uses a unique registry name to prevent duplicate registrations. The `TryRegister()` method checks if a library has already been registered and prevents duplicate registrations.

**Registry Name Hierarchy**:
- `Mamey.Auth.Jwt`: `"auth"`
- `Mamey.Auth.DecentralizedIdentifiers`: `"auth.dids"` (or `"auth.did"`)
- `Mamey.Auth.Azure`: `"auth.azure"`
- `Mamey.Auth.Azure.B2B`: `"auth.azure.b2b"`
- `Mamey.Auth.Azure.B2C`: `"auth.azure.b2c"`
- `Mamey.Auth.Multi`: `"auth.multi"`

### Scheme Names

Each authentication method uses a distinct scheme name to prevent conflicts in the authentication pipeline. Scheme names are configurable to allow customization.

**Default Scheme Names**:
- JWT: `"Bearer"` (default)
- DID: `"DidBearer"` (default)
- Azure: `"AzureAD"` (default, or `"AzureB2B"`, `"AzureB2C"` based on Azure coordinator config)
- Identity: `"Identity"` (default)
- Distributed: `"Distributed"` (default)
- Certificate: `"Certificate"` (default)

### Coordination Logic

The collision prevention works through a hierarchical coordination system:

1. **Top Level**: `Mamey.Auth.Multi` checks its registry (`"auth.multi"`) before registering
2. **Azure Level**: When Azure is enabled, `Mamey.Auth.Multi` calls `Mamey.Auth.Azure.AddAzure()`, which:
   - Checks its own registry (`"auth.azure"`)
   - Conditionally registers B2B or B2C based on `AzureMultiAuthOptions`
   - B2B and B2C libraries check their own registries (`"auth.azure.b2b"`, `"auth.azure.b2c"`) before registering
3. **Individual Libraries**: Each authentication library (JWT, DID, etc.) checks its own registry before registering

This multi-level registry system ensures that:
- No duplicate registrations occur
- Libraries can be safely called multiple times
- Coordination between libraries is explicit and controlled

### Example: Collision Prevention in Action

```csharp
// This is safe - TryRegister() prevents duplicates
builder.Services.AddMamey()
    .AddMultiAuth()  // Registers with "auth.multi"
    .AddMultiAuth(); // Returns immediately - already registered

// This is also safe - Azure coordinator checks its registry
builder.Services.AddMamey()
    .AddAzure()      // Registers with "auth.azure"
    .AddAzure();     // Returns immediately - already registered

// This is safe - Multi delegates to Azure, which checks its registry
builder.Services.AddMamey()
    .AddMultiAuth()  // Registers with "auth.multi", calls AddAzure()
    .AddAzure();     // Returns immediately - Azure already registered by Multi
```

## Integration Guide

### Integration with Mamey.Microservice.Infrastructure

The `Mamey.Microservice.Infrastructure` library uses `Mamey.Auth.Multi` by default:

```csharp
using Mamey.Microservice.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add shared microservice infrastructure (includes multi-auth)
builder.Services
    .AddMamey()
    .AddMicroserviceSharedInfrastructure();

var app = builder.Build();

// Use shared infrastructure (includes multi-auth middleware)
app.UseMamey()
    .UseSharedInfrastructure();

app.Run();
```

### Integration with Azure Authentication

When Azure authentication is enabled, `Mamey.Auth.Multi` delegates to `Mamey.Auth.Azure` coordinator:

```json
{
  "multiAuth": {
    "enableAzure": true,
    "policy": "AzureOnly",
    "azureSectionName": "azure"
  },
  "azure": {
    "enableAzure": false,
    "enableAzureB2B": false,
    "enableAzureB2C": true,
    "policy": "B2COnly",
    "azureB2CScheme": "AzureB2C"
  },
  "azure:b2c": {
    "enabled": true,
    "tenantId": "your-tenant-id",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret",
    "instance": "https://your-tenant.b2clogin.com",
    "domain": "your-tenant.onmicrosoft.com",
    "signUpSignInPolicyId": "B2C_1_signup_signin"
  }
}
```

The Azure coordinator handles:
- B2B authentication registration
- B2C authentication registration
- Regular Azure AD authentication registration
- Internal collision prevention between Azure methods

### Integration with JWT Authentication

```json
{
  "multiAuth": {
    "enableJwt": true,
    "policy": "JwtOnly",
    "jwtScheme": "Bearer",
    "jwtSectionName": "jwt"
  },
  "jwt": {
    "issuer": "https://your-issuer.com",
    "audience": "https://your-audience.com",
    "secretKey": "your-secret-key-at-least-32-characters-long",
    "expirationMinutes": 60
  }
}
```

### Integration with DID Authentication

```json
{
  "multiAuth": {
    "enableDid": true,
    "policy": "DidOnly",
    "didScheme": "DidBearer",
    "didSectionName": "dids"
  },
  "dids": {
    "enabled": true
  }
}
```

### Multiple Authentication Methods

```json
{
  "multiAuth": {
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": true,
    "policy": "EitherOr",
    "jwtScheme": "Bearer",
    "didScheme": "DidBearer",
    "azureScheme": "AzureAD"
  },
  "jwt": {
    "issuer": "https://your-issuer.com",
    "audience": "https://your-audience.com",
    "secretKey": "your-secret-key-at-least-32-characters-long"
  },
  "dids": {
    "enabled": true
  },
  "azure": {
    "enableAzureB2C": true,
    "policy": "B2COnly"
  }
}
```

## Usage Examples

### Example 1: JWT and DID Authentication

```csharp
using Mamey.Auth.Multi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMultiAuth();

var app = builder.Build();

app.UseMamey()
    .UseMultiAuth();

app.MapGet("/api/protected", () => "Protected Resource")
    .RequireAuthorization();

app.Run();
```

**appsettings.json**:
```json
{
  "multiAuth": {
    "enableJwt": true,
    "enableDid": true,
    "policy": "EitherOr"
  },
  "jwt": {
    "issuer": "https://your-issuer.com",
    "audience": "https://your-audience.com",
    "secretKey": "your-secret-key-at-least-32-characters-long"
  },
  "dids": {
    "enabled": true
  }
}
```

### Example 2: Azure B2C Only

```csharp
using Mamey.Auth.Multi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMultiAuth();

var app = builder.Build();

app.UseMamey()
    .UseMultiAuth();

app.Run();
```

**appsettings.json**:
```json
{
  "multiAuth": {
    "enableAzure": true,
    "policy": "AzureOnly"
  },
  "azure": {
    "enableAzure": false,
    "enableAzureB2B": false,
    "enableAzureB2C": true,
    "policy": "B2COnly"
  },
  "azure:b2c": {
    "enabled": true,
    "tenantId": "your-tenant-id",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret",
    "instance": "https://your-tenant.b2clogin.com",
    "domain": "your-tenant.onmicrosoft.com",
    "signUpSignInPolicyId": "B2C_1_signup_signin"
  }
}
```

### Example 3: Priority Order Authentication

```csharp
using Mamey.Auth.Multi;

var builder = WebApplication.CreateBuilder(args);

var options = new MultiAuthOptions
{
    EnableJwt = true,
    EnableDid = true,
    EnableAzure = true,
    Policy = AuthenticationPolicy.PriorityOrder,
    JwtScheme = "Bearer",
    DidScheme = "DidBearer",
    AzureScheme = "AzureAD"
};

builder.Services
    .AddMamey()
    .AddMultiAuth(options);

var app = builder.Build();

app.UseMamey()
    .UseMultiAuth();

app.Run();
```

## API Reference

### MultiAuthOptions

Configuration options for multi-authentication coordination.

```csharp
public class MultiAuthOptions
{
    public bool Enabled { get; set; } = false;
    public bool EnableJwt { get; set; } = false;
    public bool EnableDid { get; set; } = false;
    public bool EnableAzure { get; set; } = false;
    public bool EnableIdentity { get; set; } = false;
    public bool EnableDistributed { get; set; } = false;
    public bool EnableCertificate { get; set; } = false;
    public AuthenticationPolicy Policy { get; set; } = AuthenticationPolicy.EitherOr;
    public string JwtScheme { get; set; } = "Bearer";
    public string DidScheme { get; set; } = "DidBearer";
    public string AzureScheme { get; set; } = "AzureAD";
    public string IdentityScheme { get; set; } = "Identity";
    public string DistributedScheme { get; set; } = "Distributed";
    public string CertificateScheme { get; set; } = "Certificate";
    public string JwtSectionName { get; set; } = "jwt";
    public string DidSectionName { get; set; } = "dids";
    public string AzureSectionName { get; set; } = "azure";
    public string IdentitySectionName { get; set; } = "identity";
    public string DistributedSectionName { get; set; } = "distributed";
    public string CertificateSectionName { get; set; } = "certificate";
}
```

### AuthenticationPolicy Enum

```csharp
public enum AuthenticationPolicy
{
    JwtOnly,
    DidOnly,
    AzureOnly,
    IdentityOnly,
    DistributedOnly,
    CertificateOnly,
    EitherOr,
    PriorityOrder,
    AllRequired
}
```

### Extension Methods

#### AddMultiAuth

```csharp
// From configuration section
public static IMameyBuilder AddMultiAuth(this IMameyBuilder builder, string sectionName = "auth:multi")

// From options object
public static IMameyBuilder AddMultiAuth(this IMameyBuilder builder, MultiAuthOptions options)
```

#### UseMultiAuth

```csharp
public static IApplicationBuilder UseMultiAuth(this IApplicationBuilder app)
```

## Best Practices

1. **Use Registry Names**: Always use `TryRegister()` with unique registry names to prevent collisions
2. **Configure Scheme Names**: Use distinct scheme names for each authentication method
3. **Use Policies Wisely**: Choose the appropriate policy for your use case
4. **Enable Only What You Need**: Only enable authentication methods you actually use
5. **Test Collision Prevention**: Verify that duplicate registrations don't cause issues
6. **Monitor Authentication**: Log authentication attempts for debugging and security
7. **Secure Configuration**: Store sensitive configuration in secure storage (Azure Key Vault, etc.)
8. **Document Your Setup**: Document which authentication methods are enabled and why

## Troubleshooting

### Common Issues

**Authentication Not Working**: 
- Check that `enabled` is set to `true` in configuration
- Verify that at least one authentication method is enabled
- Check that the middleware is registered in the pipeline

**Collision Errors**:
- Verify that registry names are unique
- Check that scheme names are distinct
- Ensure that `TryRegister()` is being used correctly

**Azure Authentication Not Working**:
- Verify Azure coordinator configuration
- Check that Azure section name matches configuration
- Verify that Azure coordinator is properly registered

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Related Libraries

- [Mamey.Auth.Jwt](../Mamey.Auth.Jwt/README.md) - JWT authentication library
- [Mamey.Auth.DecentralizedIdentifiers](../Mamey.Auth.DecentralizedIdentifiers/README.md) - DID authentication library
- [Mamey.Auth.Azure](../Mamey.Auth.Azure/README.md) - Azure authentication coordinator
- [Mamey.Microservice.Infrastructure](../Mamey.Microservice.Infrastructure/README.md) - Microservice infrastructure (uses Multi-Auth)

## License

Proprietary - Copyright (c) 2025 Mamey.io
