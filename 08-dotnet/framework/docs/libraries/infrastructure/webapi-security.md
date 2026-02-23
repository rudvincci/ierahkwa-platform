# Mamey.WebApi.Security

**Library**: `Mamey.WebApi.Security`  
**Location**: `Mamey/src/Mamey.WebApi.Security/`  
**Type**: Security Library - Certificate Authentication  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.WebApi.Security`

## Overview

Mamey.WebApi.Security provides certificate-based authentication for Web API applications in the Mamey framework. It includes client certificate validation, access control lists (ACL), and permission-based authorization.

### Key Features

- **Certificate Authentication**: Client certificate authentication
- **Access Control Lists**: ACL-based authorization
- **Certificate Forwarding**: Support for proxy certificate forwarding
- **Permission Validation**: Customizable permission validation
- **Domain Validation**: Domain and host-based access control
- **Revocation Check**: Certificate revocation checking

## Installation

```bash
dotnet add package Mamey.WebApi.Security
```

## Quick Start

```csharp
using Mamey.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddCertificateAuthentication();

var app = builder.Build();
app.UseCertificateAuthentication();
app.Run();
```

## Configuration

```json
{
  "security": {
    "Certificate": {
      "Enabled": true,
      "Header": "X-Client-Certificate",
      "AllowedDomains": ["example.com"],
      "AllowedHosts": ["localhost"],
      "Acl": {
        "CN=client.example.com": {
          "ValidIssuer": "CN=CA",
          "ValidThumbprint": "...",
          "Permissions": ["read", "write"]
        }
      }
    }
  }
}
```

## Core Components

- **ICertificatePermissionValidator**: Permission validator interface
- **DefaultCertificatePermissionValidator**: Default permission validator
- **CertificateMiddleware**: Certificate validation middleware
- **SecurityOptions**: Configuration options

## Usage Examples

### Custom Permission Validator

```csharp
public class CustomPermissionValidator : ICertificatePermissionValidator
{
    public bool HasAccess(
        X509Certificate2 certificate,
        IEnumerable<string> permissions,
        HttpContext context)
    {
        // Custom permission validation logic
        return true;
    }
}

builder.Services.AddCertificateAuthentication(
    permissionValidatorType: typeof(CustomPermissionValidator));
```

## Related Libraries

- **Mamey.Identity**: Identity authentication
- **Mamey.Secrets.Vault**: Certificate management

## Tags

#security #certificate #authentication #acl #mamey

