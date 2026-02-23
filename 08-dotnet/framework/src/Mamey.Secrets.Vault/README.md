# Mamey.Secrets.Vault

**Library**: `Mamey.Secrets.Vault`  
**Location**: `Mamey/src/Mamey.Secrets.Vault/`  
**Type**: Secrets Library - HashiCorp Vault  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Secrets.Vault`

## Overview

Mamey.Secrets.Vault provides HashiCorp Vault integration for secret management in the Mamey framework. It includes Key-Value secrets, PKI certificate issuance, and dynamic secret leasing.

### Key Features

- **Key-Value Secrets**: Read secrets from Vault KV engine
- **PKI Integration**: Certificate issuance via Vault PKI
- **Dynamic Secrets**: Lease management for dynamic secrets
- **Configuration Integration**: Load configuration from Vault
- **Token Authentication**: Token-based authentication
- **UserPass Authentication**: Username/password authentication
- **Automatic Renewal**: Automatic lease renewal

## Installation

```bash
dotnet add package Mamey.Secrets.Vault
```

## Quick Start

```csharp
using Mamey.Secrets.Vault;

var builder = WebApplication.CreateBuilder(args);

builder.UseVault(keyValuePath: "secret/data/app");

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "vault": {
    "Enabled": true,
    "Url": "https://vault.example.com",
    "AuthType": "token",
    "Token": "your-vault-token",
    "Kv": {
      "Enabled": true,
      "EngineVersion": 2,
      "MountPoint": "secret",
      "Path": "secret/data/app"
    },
    "Pki": {
      "Enabled": true,
      "RoleName": "mamey-role",
      "MountPoint": "pki"
    }
  }
}
```

## Core Components

- **IKeyValueSecrets**: Key-value secrets interface
- **ICertificatesIssuer**: Certificate issuer interface
- **ILeaseService**: Lease management service
- **VaultOptions**: Configuration options
- **VaultHostedService**: Background service for lease renewal

## Usage Examples

### Get Secret

```csharp
@inject IKeyValueSecrets VaultSecrets

var secret = await VaultSecrets.GetAsync<T>("secret/data/app");
```

### Get Certificate

```csharp
@inject ICertificatesIssuer CertificatesIssuer

var certificate = await CertificatesIssuer.IssueAsync("mamey-role", "example.com");
```

## Related Libraries

- **Mamey.WebApi.Security**: Certificate authentication

## Tags

#vault #secrets #pki #certificates #hashi-corps #mamey















