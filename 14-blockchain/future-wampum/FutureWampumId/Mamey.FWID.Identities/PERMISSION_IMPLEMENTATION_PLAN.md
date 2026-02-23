# Permission Implementation Plan - Mamey.FWID.Identities

## Overview

This document outlines the implementation plan for handling permissions ("users:read", "identities:read", etc.) in the Identities service using the `Mamey.WebApi.Security` library.

## Current State

### Default Implementation

**Location**: `Mamey/src/Mamey.WebApi.Security/src/Mamey.WebApi.Security/DefaultCertificatePermissionValidator.cs`

```csharp
internal sealed class DefaultCertificatePermissionValidator : ICertificatePermissionValidator
{
    public bool HasAccess(X509Certificate2 certificate, IEnumerable<string> permissions, HttpContext context)
        => true;  // Always returns true - no actual validation!
}
```

**Problem**: The default validator always returns `true`, meaning permissions are not actually validated.

### ACL Configuration

Permissions are defined in `appsettings.json`:

```json
{
  "security": {
    "certificate": {
      "acl": {
        "dids-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        }
      }
    }
  }
}
```

**Problem**: These permissions are defined but not checked against actual operations.

## Implementation Strategy

### 1. Custom Permission Validator

Create a custom `ICertificatePermissionValidator` that:
- Extracts required permissions from the HTTP context (route, method, etc.)
- Checks if the certificate's ACL permissions match the required permissions
- Supports permission hierarchy (e.g., `write` includes `read`)
- Stores permissions in HTTP context for later use

### 2. Permission Mapping

Map API endpoints and gRPC methods to required permissions:

| Endpoint/Method | Required Permission |
|----------------|-------------------|
| `GET /api/identities` | `identities:read` |
| `GET /api/identities/{id}` | `identities:read` |
| `POST /api/identities` | `identities:write` |
| `POST /api/identities/{id}/verify` | `identities:verify` |
| `PUT /api/identities/{id}/biometric` | `identities:write` |
| `POST /api/identities/{id}/revoke` | `identities:revoke` |
| `PUT /api/identities/{id}/zone` | `identities:write` |
| `PUT /api/identities/{id}/contact` | `identities:write` |
| `gRPC: VerifyBiometric` | `identities:verify` |

### 3. Permission Hierarchy

Implement permission hierarchy:

```
admin > write > verify > read
```

- `identities:admin` - Full access (includes all permissions)
- `identities:write` - Create/update (includes read, verify)
- `identities:verify` - Verify identities (includes read)
- `identities:read` - Read-only access
- `identities:revoke` - Revoke identities (separate permission)

## Implementation Steps

### Step 1: Create Custom Permission Validator

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Infrastructure/Security/IdentityPermissionValidator.cs`

```csharp
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Identities.Infrastructure.Security;

/// <summary>
/// Custom permission validator for Identities service.
/// Validates certificate permissions against required permissions for operations.
/// </summary>
public class IdentityPermissionValidator : ICertificatePermissionValidator
{
    private readonly ILogger<IdentityPermissionValidator> _logger;
    private static readonly Dictionary<string, string[]> PermissionHierarchy = new()
    {
        { "identities:admin", new[] { "identities:read", "identities:write", "identities:verify", "identities:revoke" } },
        { "identities:write", new[] { "identities:read", "identities:verify" } },
        { "identities:verify", new[] { "identities:read" } },
        { "identities:read", Array.Empty<string>() },
        { "identities:revoke", Array.Empty<string>() }
    };

    public IdentityPermissionValidator(ILogger<IdentityPermissionValidator> logger)
    {
        _logger = logger;
    }

    public bool HasAccess(X509Certificate2 certificate, IEnumerable<string> grantedPermissions, HttpContext context)
    {
        if (certificate == null)
        {
            _logger.LogWarning("Certificate is null");
            return false;
        }

        var granted = grantedPermissions?.ToList() ?? new List<string>();
        if (!granted.Any())
        {
            _logger.LogWarning("No permissions granted for certificate: {Subject}", certificate.Subject);
            return false;
        }

        // Get required permission from context
        var requiredPermission = GetRequiredPermission(context);
        if (string.IsNullOrEmpty(requiredPermission))
        {
            // No specific permission required - allow access
            _logger.LogDebug("No specific permission required for: {Path}", context.Request.Path);
            return true;
        }

        // Check if granted permissions include required permission or higher
        var hasAccess = HasPermission(granted, requiredPermission);
        
        if (!hasAccess)
        {
            _logger.LogWarning(
                "Permission denied: Certificate {Subject} does not have required permission {RequiredPermission}. Granted: {GrantedPermissions}",
                certificate.Subject, requiredPermission, string.Join(", ", granted));
        }
        else
        {
            _logger.LogDebug(
                "Permission granted: Certificate {Subject} has permission {RequiredPermission}",
                certificate.Subject, requiredPermission);
        }

        return hasAccess;
    }

    private string? GetRequiredPermission(HttpContext context)
    {
        // Try to get from route data (set by middleware)
        if (context.Items.TryGetValue("RequiredPermission", out var permission) && permission is string perm)
        {
            return perm;
        }

        // Map HTTP method and path to permission
        var method = context.Request.Method;
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        return (method, path) switch
        {
            // Read operations
            ("GET", "/api/identities") => "identities:read",
            ("GET", var p) when p.StartsWith("/api/identities/") && p.EndsWith("/verify") == false => "identities:read",
            
            // Write operations
            ("POST", "/api/identities") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/biometric") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/zone") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/contact") => "identities:write",
            
            // Verify operations
            ("POST", var p) when p.Contains("/verify") => "identities:verify",
            
            // Revoke operations
            ("POST", var p) when p.Contains("/revoke") => "identities:revoke",
            
            // gRPC operations (from metadata)
            _ when context.Request.Headers.ContainsKey("grpc-method") => GetGrpcPermission(context),
            
            _ => null
        };
    }

    private string? GetGrpcPermission(HttpContext context)
    {
        // Extract gRPC method from headers or route
        var method = context.Request.Headers["grpc-method"].ToString();
        
        return method switch
        {
            var m when m.Contains("VerifyBiometric") => "identities:verify",
            _ => "identities:read" // Default for gRPC
        };
    }

    private static bool HasPermission(List<string> grantedPermissions, string requiredPermission)
    {
        // Direct match
        if (grantedPermissions.Contains(requiredPermission, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check hierarchy
        if (PermissionHierarchy.TryGetValue(requiredPermission.ToLowerInvariant(), out var includedPermissions))
        {
            // Check if any granted permission includes the required permission
            foreach (var granted in grantedPermissions)
            {
                var grantedLower = granted.ToLowerInvariant();
                
                // Direct match
                if (grantedLower == requiredPermission.ToLowerInvariant())
                {
                    return true;
                }

                // Check if granted permission includes required permission in its hierarchy
                if (PermissionHierarchy.TryGetValue(grantedLower, out var grantedIncludes))
                {
                    if (grantedIncludes.Contains(requiredPermission, StringComparer.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        // Admin permission grants everything
        if (grantedPermissions.Any(p => p.Equals("identities:admin", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}
```

### Step 2: Register Custom Validator

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Infrastructure/Extensions.cs`

```csharp
// In AddInfrastructure method, update AddCertificateAuthentication call:
.AddCertificateAuthentication(sectionName: "security", permissionValidatorType: typeof(IdentityPermissionValidator))
```

### Step 3: Permission Middleware (Optional)

Create middleware to extract and set required permissions from routes:

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Infrastructure/Security/PermissionMiddleware.cs`

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Security;

/// <summary>
/// Middleware to extract required permissions from routes and store in context.
/// </summary>
public class PermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PermissionMiddleware> _logger;

    public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requiredPermission = GetRequiredPermission(context);
        if (!string.IsNullOrEmpty(requiredPermission))
        {
            context.Items["RequiredPermission"] = requiredPermission;
            _logger.LogDebug("Required permission for {Path}: {Permission}", 
                context.Request.Path, requiredPermission);
        }

        await _next(context);
    }

    private string? GetRequiredPermission(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        return (method, path) switch
        {
            ("GET", "/api/identities") => "identities:read",
            ("GET", var p) when p.StartsWith("/api/identities/") && !p.Contains("/verify") => "identities:read",
            ("POST", "/api/identities") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/biometric") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/zone") => "identities:write",
            ("PUT", var p) when p.StartsWith("/api/identities/") && p.Contains("/contact") => "identities:write",
            ("POST", var p) when p.Contains("/verify") => "identities:verify",
            ("POST", var p) when p.Contains("/revoke") => "identities:revoke",
            _ => null
        };
    }
}
```

### Step 4: Update gRPC Interceptor

Update `CertificateAuthenticationInterceptor` to check permissions based on gRPC method:

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Infrastructure/Grpc/Interceptors/CertificateAuthenticationInterceptor.cs`

```csharp
// In UnaryServerHandler, after ACL validation:
// Validate permissions if specified
if (acl.Permissions != null && acl.Permissions.Any())
{
    // Determine required permission based on gRPC method
    var requiredPermission = GetRequiredPermissionForMethod(context.Method);
    
    if (!string.IsNullOrEmpty(requiredPermission))
    {
        // Check if certificate has required permission
        var hasPermission = HasPermission(acl.Permissions, requiredPermission);
        
        if (!hasPermission)
        {
            _logger.LogWarning(
                "Permission denied: Certificate {Subject} does not have required permission {RequiredPermission} for method {Method}",
                certificate.Subject, requiredPermission, context.Method);
            throw new RpcException(new Status(StatusCode.PermissionDenied, 
                $"Insufficient permissions. Required: {requiredPermission}"));
        }
    }
    
    // Also validate using the permission validator (for consistency)
    var httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null && !_certificatePermissionValidator.HasAccess(certificate, acl.Permissions, httpContext))
    {
        _logger.LogWarning("Certificate does not have required permissions: {Subject}", subject);
        throw new RpcException(new Status(StatusCode.PermissionDenied, "Insufficient permissions"));
    }
}

private string? GetRequiredPermissionForMethod(string method)
{
    return method switch
    {
        var m when m.Contains("VerifyBiometric") => "identities:verify",
        _ => "identities:read" // Default for gRPC
    };
}

private bool HasPermission(IEnumerable<string> grantedPermissions, string requiredPermission)
{
    // Use same logic as IdentityPermissionValidator
    // ... (implement permission hierarchy check)
}
```

### Step 5: Update HTTP Routes

Add permission checks to HTTP routes (optional - can rely on middleware):

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Api/IdentityRoutes.cs`

```csharp
// Add permission check in beforeDispatch:
.Post<AddIdentity>("/api/identities",
    beforeDispatch: async ([FromBody] cmd, ctx) =>
    {
        // Permission check is handled by CertificateMiddleware
        // But we can add explicit check here if needed
        await Task.CompletedTask;
    },
    // ... rest of configuration
)
```

## Permission Definitions

### Standard Permissions

| Permission | Description | Includes |
|-----------|-------------|----------|
| `identities:read` | Read identity information | None |
| `identities:verify` | Verify identity biometrics | `identities:read` |
| `identities:write` | Create/update identities | `identities:read`, `identities:verify` |
| `identities:revoke` | Revoke identities | None (separate permission) |
| `identities:admin` | Full administrative access | All permissions |

### Service-Specific Permissions

| Service | Recommended Permissions | Operations Allowed |
|---------|------------------------|-------------------|
| DIDs Service | `identities:read`, `identities:verify` | GetIdentity, VerifyBiometric |
| Credentials Service | `identities:read`, `identities:verify` | GetIdentity, VerifyBiometric |
| ZKPs Service | `identities:read`, `identities:verify` | GetIdentity, VerifyBiometric |
| AccessControls Service | `identities:read`, `identities:verify` | GetIdentity, VerifyBiometric |
| Operations Service | `identities:read`, `identities:write` | All read/write operations |
| Sagas Service | `identities:read`, `identities:write` | All read/write operations |
| Notifications Service | `identities:read` | GetIdentity, FindIdentities |
| API Gateway | `identities:read`, `identities:write` | All operations |

## Testing

### Unit Tests

**File**: `tests/Mamey.FWID.Identities.Tests.Unit/Infrastructure/Security/IdentityPermissionValidatorTests.cs`

```csharp
public class IdentityPermissionValidatorTests
{
    [Fact]
    public void HasAccess_WithRequiredRead_AndGrantedRead_ReturnsTrue()
    {
        // Arrange
        var validator = new IdentityPermissionValidator(Mock.Of<ILogger<IdentityPermissionValidator>>());
        var certificate = CreateTestCertificate();
        var grantedPermissions = new[] { "identities:read" };
        var context = CreateHttpContext("GET", "/api/identities/123");
        
        // Act
        var result = validator.HasAccess(certificate, grantedPermissions, context);
        
        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void HasAccess_WithRequiredWrite_AndGrantedRead_ReturnsFalse()
    {
        // Arrange
        var validator = new IdentityPermissionValidator(Mock.Of<ILogger<IdentityPermissionValidator>>());
        var certificate = CreateTestCertificate();
        var grantedPermissions = new[] { "identities:read" };
        var context = CreateHttpContext("POST", "/api/identities");
        
        // Act
        var result = validator.HasAccess(certificate, grantedPermissions, context);
        
        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void HasAccess_WithRequiredRead_AndGrantedWrite_ReturnsTrue()
    {
        // Arrange
        var validator = new IdentityPermissionValidator(Mock.Of<ILogger<IdentityPermissionValidator>>());
        var certificate = CreateTestCertificate();
        var grantedPermissions = new[] { "identities:write" };
        var context = CreateHttpContext("GET", "/api/identities/123");
        
        // Act
        var result = validator.HasAccess(certificate, grantedPermissions, context);
        
        // Assert
        result.ShouldBeTrue(); // write includes read
    }
    
    // ... more tests
}
```

### Integration Tests

Test permission validation with real HTTP requests and certificates.

## Configuration Updates

### appsettings.json

Update ACL configuration with proper permissions:

```json
{
  "security": {
    "certificate": {
      "acl": {
        "dids-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "credentials-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "operations-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:write"]
        }
      }
    }
  }
}
```

## Implementation Checklist

- [ ] Create `IdentityPermissionValidator` class
- [ ] Register custom validator in `Extensions.cs`
- [ ] Create `PermissionMiddleware` (optional)
- [ ] Update `CertificateAuthenticationInterceptor` for gRPC permission checks
- [ ] Add unit tests for permission validator
- [ ] Add integration tests for permission validation
- [ ] Update ACL configuration in `appsettings.json`
- [ ] Update ACL configuration in environment-specific configs
- [ ] Document permission requirements for each endpoint
- [ ] Test permission validation with real certificates

## Notes

1. **Permission Hierarchy**: Implemented in `HasPermission` method
2. **Default Behavior**: If no specific permission is required, access is granted
3. **gRPC vs HTTP**: Both use the same permission validator, but extract required permissions differently
4. **Logging**: All permission checks should be logged for audit purposes
5. **Performance**: Permission checks should be fast (in-memory lookups)

