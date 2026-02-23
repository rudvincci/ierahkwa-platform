# Mamey.Identity.Jwt

**Library**: `Mamey.Identity.Jwt`  
**Location**: `Mamey/src/Mamey.Identity.Jwt/`  
**Type**: Identity Library - JWT Authentication  
**Version**: 2.0.*  
**Files**: 15 C# files  
**Namespace**: `Mamey.Identity.Jwt`

## Overview

Mamey.Identity.Jwt provides comprehensive JWT (JSON Web Token) authentication support for the Mamey framework. It includes JWT token generation, validation, and integration with ASP.NET Core authentication middleware.

### Key Features

- **JWT Token Generation**: Create signed JWT tokens with custom claims
- **Token Validation**: Validate JWT tokens with configurable parameters
- **Bearer Authentication**: ASP.NET Core JWT Bearer authentication integration
- **Certificate Support**: X.509 certificate-based token signing
- **Symmetric Key Support**: HMAC-based token signing
- **Token Storage**: Memory and Redis token storage options
- **Token Refresh**: Refresh token support

## Installation

```bash
dotnet add package Mamey.Identity.Jwt
```

## Quick Start

```csharp
using Mamey.Identity.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddJwt(options =>
    {
        options.IssuerSigningKey = "your-secret-key";
        options.ValidIssuer = "mamey-app";
        options.ValidAudience = "mamey-api";
        options.ValidateIssuer = true;
        options.ValidateAudience = true;
        options.ValidateLifetime = true;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

## Core Components

- **IJwtHandler**: JWT token operations interface
- **JwtHandler**: JWT handler implementation
- **JwtOptions**: JWT configuration options
- **AccessTokenValidatorMiddleware**: Token validation middleware
- **InMemoryAccessTokenService**: In-memory token storage
- **RedisAccessTokenService**: Redis token storage

## Usage Examples

### Generate JWT Token

```csharp
@inject IJwtHandler JwtHandler

var token = JwtHandler.CreateToken(
    userId: "user123",
    role: "Admin",
    audience: "mamey-api",
    claims: new Dictionary<string, string>
    {
        { "email", "user@example.com" },
        { "name", "John Doe" }
    });
```

### Validate Token

```csharp
var payload = JwtHandler.GetTokenPayload(token.AccessToken);
var userId = payload.Sub;
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Identity.Redis**: Redis token storage

## Tags

#identity #jwt #authentication #token #bearer #mamey

