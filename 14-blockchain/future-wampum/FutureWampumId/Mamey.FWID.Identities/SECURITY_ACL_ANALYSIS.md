# Security ACL Configuration Analysis - Mamey.FWID.Identities

## Overview

This document analyzes the `Mamey.WebApi.Security` library and identifies the missing ACL (Access Control List) configuration for certificate authentication in the Identities service.

## Mamey.WebApi.Security Library Analysis

### SecurityOptions Structure

**Location**: `Mamey/src/Mamey.WebApi.Security/src/Mamey.WebApi.Security/SecurityOptions.cs`

```csharp
public class SecurityOptions
{
    public CertificateOptions Certificate { get; set; }

    public class CertificateOptions
    {
        public bool Enabled { get; set; }
        public string Header { get; set; }
        public bool AllowSubdomains { get; set; }
        public IEnumerable<string> AllowedDomains { get; set; }
        public IEnumerable<string> AllowedHosts { get; set; }
        public IDictionary<string, AclOptions> Acl { get; set; }  // ← ACL Configuration
        public bool SkipRevocationCheck { get; set; }

        public class AclOptions
        {
            public string ValidIssuer { get; set; }
            public string ValidThumbprint { get; set; }
            public string ValidSerialNumber { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }
    }
}
```

### ACL Purpose

The ACL (Access Control List) defines which **services are allowed to query this service** using certificate authentication. It maps certificate subjects (service names) to their allowed permissions.

**Key Points:**
- ACL is a dictionary where the **key** is the certificate subject (e.g., `"identities-service"` or `"CN=identities-service"`)
- ACL is used by both:
  - `CertificateMiddleware` (for HTTP requests)
  - `CertificateAuthenticationInterceptor` (for gRPC requests)

## Current Configuration

### Identities Service ACL (Current)

**File**: `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Api/appsettings.json`

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
        "wallets-service": {
          "validIssuer": "localhost",
          "permissions": ["users:read"]
        }
      }
    }
  }
}
```

**Current ACL Entries:**
- ✅ `wallets-service` - Allowed with `users:read` permission

## Missing ACL Entries

### Services That Call Identities Service

Based on the codebase analysis, the following services may call the Identities service via gRPC:

1. **DIDs Service** (Port 5002)
   - May call Identities for identity verification
   - Should have ACL entry: `"dids-service"`

2. **Credentials Service** (Port 5005)
   - May call Identities for identity verification
   - Should have ACL entry: `"credentials-service"`

3. **ZKPs Service** (Port 5003)
   - May call Identities for identity verification
   - Should have ACL entry: `"zkps-service"`

4. **AccessControls Service** (Port 5004)
   - May call Identities for identity verification
   - Should have ACL entry: `"access-controls-service"`

5. **Operations Service** (Port 5007)
   - May call Identities for operations
   - Should have ACL entry: `"operations-service"`

6. **Sagas Service** (Port 5006)
   - May call Identities for saga orchestration
   - Should have ACL entry: `"sagas-service"`

7. **Notifications Service** (Port 5008)
   - May call Identities for notification delivery
   - Should have ACL entry: `"notifications-service"`

8. **API Gateway** (Port 5000)
   - Routes requests to Identities service
   - Should have ACL entry: `"api-gateway"` or `"fwid-api-gateway"`

## Recommended ACL Configuration

### Complete ACL Configuration

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
        "wallets-service": {
          "validIssuer": "localhost",
          "permissions": ["users:read"]
        },
        "dids-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "credentials-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "zkps-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "access-controls-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:verify"]
        },
        "operations-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:write"]
        },
        "sagas-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:write"]
        },
        "notifications-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read"]
        },
        "api-gateway": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:write"]
        }
      }
    }
  }
}
```

## Permission Recommendations

### Permission Types

Based on the Identities service operations:

- **`identities:read`** - Read identity information (GetIdentity, FindIdentities)
- **`identities:write`** - Create/update identities (AddIdentity, UpdateBiometric, UpdateContactInformation, UpdateZone)
- **`identities:verify`** - Verify identity biometrics (VerifyBiometric, VerifyIdentity)
- **`identities:revoke`** - Revoke identities (RevokeIdentity)
- **`identities:admin`** - Full administrative access

### Service-Specific Permissions

| Service | Recommended Permissions | Reason |
|---------|------------------------|--------|
| DIDs Service | `identities:read`, `identities:verify` | Needs to verify identities for DID creation |
| Credentials Service | `identities:read`, `identities:verify` | Needs to verify identities for credential issuance |
| ZKPs Service | `identities:read`, `identities:verify` | Needs to verify identities for ZKP generation |
| AccessControls Service | `identities:read`, `identities:verify` | Needs to verify identities for access control |
| Operations Service | `identities:read`, `identities:write` | May need to create/update identities for operations |
| Sagas Service | `identities:read`, `identities:write` | May need to create/update identities during saga orchestration |
| Notifications Service | `identities:read` | Needs to read identity information for notifications |
| API Gateway | `identities:read`, `identities:write` | Routes all requests, needs full access |

## Implementation Notes

### Certificate Subject Format

The ACL key can be either:
- Simple format: `"identities-service"` (will be converted to `"CN=identities-service"`)
- Full format: `"CN=identities-service"`

The `CertificateMiddleware` automatically converts simple format to full format:
```csharp
var subject = key.StartsWith("CN=") ? key : $"CN={key}";
```

### ACL Validation Flow

1. **Certificate Extraction**: Certificate is extracted from HTTP context or gRPC metadata
2. **Subject Lookup**: Certificate subject is looked up in ACL dictionary
3. **Issuer Validation**: If `ValidIssuer` is specified, certificate issuer must match
4. **Thumbprint Validation**: If `ValidThumbprint` is specified, certificate thumbprint must match
5. **Serial Number Validation**: If `ValidSerialNumber` is specified, certificate serial number must match
6. **Permission Validation**: If `Permissions` are specified, certificate must have required permissions

### Error Responses

- **401 Unauthorized**: Certificate not found or invalid
- **403 Forbidden**: Certificate not in ACL or insufficient permissions

## Next Steps

1. **Update appsettings.json**: Add missing ACL entries for all services that call Identities
2. **Update appsettings.Development.json**: Add development-specific ACL entries
3. **Update appsettings.Docker.json**: Add Docker-specific ACL entries
4. **Update appsettings.Local.json**: Add local-specific ACL entries
5. **Verify Certificate Subjects**: Ensure all services use consistent certificate subject names
6. **Test ACL Validation**: Verify that ACL validation works correctly for all services

## References

- `Mamey/src/Mamey.WebApi.Security/src/Mamey.WebApi.Security/SecurityOptions.cs`
- `Mamey/src/Mamey.WebApi.Security/src/Mamey.WebApi.Security/CertificateMiddleware.cs`
- `FutureWampum/Mamey.FWID.Identities/src/Mamey.FWID.Identities.Infrastructure/Grpc/Interceptors/CertificateAuthenticationInterceptor.cs`
- `Mamey/docs/libraries/infrastructure/webapi-security.md`

