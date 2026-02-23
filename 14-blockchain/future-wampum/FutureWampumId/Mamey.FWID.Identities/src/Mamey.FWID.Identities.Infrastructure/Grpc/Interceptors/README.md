# gRPC Authentication Interceptors

This directory contains gRPC authentication interceptors for the FutureWampumID services.

## Overview

The gRPC authentication interceptors provide dual authentication support:
1. **JWT Authentication** - For external clients (web, mobile apps)
2. **Certificate Authentication** - For internal service-to-service communication

## Interceptors

### JwtAuthenticationInterceptor

Handles JWT token authentication for external clients.

**Features:**
- Extracts JWT token from gRPC metadata (`authorization` header or `x-jwt-token` header)
- Validates JWT token using ASP.NET Core authentication
- Sets authenticated user in HTTP context
- Allows anonymous access for health checks

**Usage:**
The interceptor is automatically applied to all gRPC services when registered via `AddGrpc()`.

### CertificateAuthenticationInterceptor

Handles certificate-based authentication for internal service-to-service communication.

**Features:**
- Extracts certificate from HTTP context or gRPC metadata
- Validates certificate (expiry, issuer, thumbprint)
- Supports ACL-based permission validation
- Falls back to JWT authentication if no certificate is present

**Usage:**
The interceptor is automatically applied to all gRPC services when registered via `AddGrpc()`.

## Registration

The interceptors are registered in `Extensions.cs`:

```csharp
// Register gRPC authentication interceptors
builder.Services.AddScoped<JwtAuthenticationInterceptor>();
builder.Services.AddScoped<CertificateAuthenticationInterceptor>();

// Register gRPC with authentication interceptors
builder.Services.AddGrpc(options =>
{
    // Add authentication interceptors globally
    options.Interceptors.Add<JwtAuthenticationInterceptor>();
    options.Interceptors.Add<CertificateAuthenticationInterceptor>();
});
```

## Configuration

### JWT Authentication

JWT authentication is configured via `appsettings.json` using the MultiAuth configuration:

```json
{
  "multiAuth": {
    "enabled": true,
    "enableJwt": true,
    "enableDid": true,
    "policy": "EitherOr",
    "jwtScheme": "Bearer",
    "didScheme": "DidBearer",
    "jwtSectionName": "jwt",
    "didSectionName": "dids"
  },
  "jwt": {
    "algorithm": "HS512",
    "issuerSigningKey": "...",
    "issuer": "auth.mamey.io",
    "validIssuer": "auth.mamey.io",
    "audience": "localhost",
    "validateAudience": false,
    "validateIssuer": true,
    "validateLifetime": true,
    "expiry": "00.00:30:00",
    "expiryMinutes": 60,
    "refreshTokenLifetime": "00.00:30:00"
  },
  "dids": {
    "enabled": true
  }
}
```

**Note**: The service uses `Mamey.Auth.Multi` for coordinated authentication. JWT and DID authentication are configured through the `multiAuth` section, which delegates to the respective authentication libraries.

### Certificate Authentication

Certificate authentication is configured via `appsettings.json`:

```json
{
  "security": {
    "certificate": {
      "enabled": true,
      "header": "Certificate",
      "skipRevocationCheck": false,
      "allowedDomains": ["mamey.io"],
      "allowSubdomains": true,
      "allowedHosts": ["localhost"],
      "acl": {
        "service-name": {
          "validIssuer": "localhost",
          "validThumbprint": "...",
          "permissions": ["read", "write"]
        }
      }
    }
  }
}
```

## Authentication Flow

1. **Certificate First**: If a certificate is present, validate it
2. **JWT Fallback**: If no certificate, validate JWT token
3. **Anonymous**: Health checks and certain internal methods are allowed without authentication

## Error Handling

- **Unauthenticated** (401): Invalid or missing token/certificate
- **Permission Denied** (403): Certificate not authorized or insufficient permissions
- **Internal Error** (500): Authentication processing error

## Dependencies

- `Mamey.WebApi.Security` - SecurityOptions and ICertificatePermissionValidator
- `Mamey.Microservice.Infrastructure` - Includes AddCertificateAuthentication()
- `Mamey.Auth.Jwt` - JWT authentication (via AddMicroserviceSharedInfrastructure())

## Notes

- The interceptors are applied globally to all gRPC services
- Certificate authentication is optional (can be disabled via configuration)
- JWT authentication is required for external clients
- Health checks are exempt from authentication requirements



