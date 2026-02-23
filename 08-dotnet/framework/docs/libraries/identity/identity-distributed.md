# Mamey.Identity.Distributed

**Library**: `Mamey.Identity.Distributed`  
**Location**: `Mamey/src/Mamey.Identity.Distributed/`  
**Type**: Identity Library - Distributed Identity  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Identity.Distributed`

## Overview

Mamey.Identity.Distributed provides distributed identity and authentication support for microservices architectures in the Mamey framework. It enables cross-service authentication, distributed token management, and microservice-to-microservice authentication.

### Key Features

- **Distributed Token Management**: Shared token storage across services
- **Microservice Authentication**: Service-to-service authentication
- **Distributed Sessions**: Shared session management
- **Token Validation**: Cross-service token validation
- **Redis Integration**: Distributed token caching
- **JWT Support**: JWT-based distributed authentication

## Installation

```bash
dotnet add package Mamey.Identity.Distributed
```

## Quick Start

```csharp
using Mamey.Identity.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddDistributedIdentity(builder.Configuration);

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "Identity:Distributed": {
    "Enabled": true,
    "EnableJwtAuthentication": true,
    "EnableDistributedSessions": true,
    "EnableMicroserviceAuth": true,
    "JwtIssuer": "mamey-api",
    "JwtAudience": "mamey-services",
    "JwtSigningKey": "your-secret-key",
    "RedisConnectionString": "localhost:6379"
  }
}
```

## Core Components

- **IDistributedTokenService**: Distributed token operations
- **IDistributedSessionService**: Distributed session management
- **IMicroserviceAuthService**: Microservice authentication
- **ITokenValidationService**: Token validation service
- **DistributedAuthenticationMiddleware**: Authentication middleware
- **MicroserviceAuthMiddleware**: Microservice auth middleware

## Usage Examples

### Create Distributed Token

```csharp
@inject IDistributedTokenService TokenService

var token = await TokenService.CreateDistributedTokenAsync(
    user: authenticatedUser,
    serviceId: "service-123");
```

### Validate Token

```csharp
var isValid = await TokenService.ValidateDistributedTokenAsync(token);
```

### Microservice Authentication

```csharp
@inject IMicroserviceAuthService MicroserviceAuth

var serviceToken = await MicroserviceAuth.AuthenticateServiceAsync(
    microserviceId: "service-123",
    microserviceSecret: "secret");
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Identity.Redis**: Redis token storage
- **Mamey.Identity.Jwt**: JWT authentication

## Tags

#identity #distributed #microservices #authentication #token #mamey

