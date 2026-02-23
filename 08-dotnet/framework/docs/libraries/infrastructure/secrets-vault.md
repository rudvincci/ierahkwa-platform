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

### Basic Configuration

```json
{
  "vault": {
    "enabled": true,
    "url": "http://localhost:8200",
    "authType": "token",
    "token": "your-vault-token",
    "kv": {
      "enabled": true,
      "engineVersion": 2,
      "mountPoint": "kv",
      "path": "mamey/service-name/appsettings"
    },
    "pki": {
      "enabled": true,
      "roleName": "service-name",
      "commonName": "service-name.mamey.io"
    }
  }
}
```

### Complete Configuration Example

```json
{
  "vault": {
    "enabled": true,
    "url": "http://localhost:8200",
    "authType": "token",
    "token": "secret",
    "username": "user",
    "password": "secret",
    "kv": {
      "enabled": true,
      "engineVersion": 2,
      "mountPoint": "kv",
      "path": "mamey/service-name/appsettings"
    },
    "pki": {
      "enabled": true,
      "roleName": "service-name",
      "commonName": "service-name.mamey.io",
      "mountPoint": "pki"
    },
    "lease": {
      "mongo": {
        "type": "database",
        "roleName": "service-name",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "connectionString": "mongodb://{{username}}:{{password}}@mongo:27017"
        }
      },
      "postgres": {
        "type": "database",
        "roleName": "service-name-postgres",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "connectionString": "Host=postgres;Database=service-name;Username={{username}};Password={{password}};Port=5432"
        }
      },
      "rabbitmq": {
        "type": "rabbitmq",
        "roleName": "service-name",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "username": "{{username}}",
          "password": "{{password}}"
        }
      },
      "consul": {
        "type": "consul",
        "roleName": "service-name",
        "enabled": true,
        "autoRenewal": true,
        "templates": {
          "token": "{{token}}"
        }
      }
    }
  }
}
```

## Supported Lease Types

The library supports the following lease types for dynamic secret management:

### 1. Database Leases

Works with any database type supported by Vault's database secrets engine (MongoDB, PostgreSQL, MySQL, MSSQL, etc.).

**Configuration:**
```json
{
  "lease": {
    "mongo": {
      "type": "database",
      "roleName": "service-name",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "connectionString": "mongodb://{{username}}:{{password}}@mongo:27017"
      }
    },
    "postgres": {
      "type": "database",
      "roleName": "service-name-postgres",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "connectionString": "Host=postgres;Database=service-name;Username={{username}};Password={{password}};Port=5432"
      }
    }
  }
}
```

**Template Placeholders:**
- `{{username}}` - Database username from Vault
- `{{password}}` - Database password from Vault

**Vault Server Setup:**
Reference: `Mamey.Info/docs/vault-config.md` for complete Vault server configuration.

### 2. RabbitMQ Leases

Dynamic RabbitMQ user credentials.

**Configuration:**
```json
{
  "lease": {
    "rabbitmq": {
      "type": "rabbitmq",
      "roleName": "service-name",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "username": "{{username}}",
        "password": "{{password}}"
      }
    }
  }
}
```

**Template Placeholders:**
- `{{username}}` - RabbitMQ username from Vault
- `{{password}}` - RabbitMQ password from Vault

**Vault Server Setup:**
```bash
# Enable RabbitMQ secrets engine
vault secrets enable rabbitmq

# Configure RabbitMQ connection
vault write rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="admin" \
    password="secret"

# Create RabbitMQ role
vault write rabbitmq/roles/service-name \
    tags="service-name" \
    vhosts='{"/":{"configure":".*","write":".*","read":".*"}}' \
    default_ttl="1h" \
    max_ttl="24h"
```

### 3. Consul Leases

Dynamic Consul ACL tokens.

**Configuration:**
```json
{
  "lease": {
    "consul": {
      "type": "consul",
      "roleName": "service-name",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "token": "{{token}}"
      }
    }
  }
}
```

**Template Placeholders:**
- `{{token}}` - Consul ACL token from Vault

**Vault Server Setup:**
```bash
# Enable Consul secrets engine
vault secrets enable consul

# Configure Consul connection
vault write consul/config/access \
    address="http://consul:8500" \
    token="secret-consul-token"

# Create Consul role
vault write consul/roles/service-name \
    policy="service-name-policy" \
    ttl="1h" \
    max_ttl="24h"
```

### 4. Active Directory Leases

Dynamic Active Directory credentials.

**Configuration:**
```json
{
  "lease": {
    "activedirectory": {
      "type": "activedirectory",
      "roleName": "service-name",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "username": "{{username}}",
        "currentPassword": "{{currentPassword}}",
        "lastPassword": "{{lastPassword}}"
      }
    }
  }
}
```

### 5. Azure Leases

Dynamic Azure service principal credentials.

**Configuration:**
```json
{
  "lease": {
    "azure": {
      "type": "azure",
      "roleName": "service-name",
      "enabled": true,
      "autoRenewal": true,
      "templates": {
        "clientId": "{{clientId}}",
        "clientSecret": "{{clientSecret}}"
      }
    }
  }
}
```

## Key-Value Secrets

### Configuration

```json
{
  "vault": {
    "kv": {
      "enabled": true,
      "engineVersion": 2,
      "mountPoint": "kv",
      "path": "mamey/service-name/appsettings"
    }
  }
}
```

**Properties:**
- `enabled`: Enable/disable KV secrets loading
- `engineVersion`: KV engine version (1 or 2, default: 2)
- `mountPoint`: Vault mount point for KV engine (default: "kv")
- `path`: Path to the secret in Vault (for v2, use path without `/data/` prefix)

**Vault Server Setup:**
```bash
# Enable KV v2 engine
vault secrets enable -path="kv" -version=2 -description="Mamey KV" kv

# Store secrets
vault kv put kv/mamey/service-name/appsettings \
    Key1=Value1 \
    Key2=Value2 \
    ConnectionStrings__DefaultConnection="mongodb://..."
```

## PKI Certificates

### Configuration

```json
{
  "vault": {
    "pki": {
      "enabled": true,
      "roleName": "service-name",
      "commonName": "service-name.mamey.io",
      "mountPoint": "pki"
    }
  }
}
```

**Properties:**
- `enabled`: Enable/disable PKI certificate issuance
- `roleName`: Vault PKI role name
- `commonName`: Certificate common name (must use `mamey.io` domain)
- `mountPoint`: Vault mount point for PKI engine (default: "pki")

**Vault Server Setup:**
```bash
# Enable PKI secrets engine
vault secrets enable pki

# Configure PKI URLs
vault write pki/config/urls \
    issuing_certificates="http://localhost:8200/v1/pki/ca" \
    crl_distribution_points="http://localhost:8200/v1/pki/crl"

# Create PKI role
vault write pki/roles/service-name \
    allowed_domains=mamey.io \
    allow_localhost=true \
    allow_subdomains=true \
    max_ttl=72h
```

## Template Substitution

The library supports template substitution in lease configurations. Placeholders are automatically replaced with values from Vault.

### Available Placeholders

**Database Leases:**
- `{{username}}` - Database username
- `{{password}}` - Database password

**RabbitMQ Leases:**
- `{{username}}` - RabbitMQ username
- `{{password}}` - RabbitMQ password

**Consul Leases:**
- `{{token}}` - Consul ACL token

**Active Directory Leases:**
- `{{username}}` - AD username
- `{{currentPassword}}` - Current password
- `{{lastPassword}}` - Last password

**Azure Leases:**
- `{{clientId}}` - Azure client ID
- `{{clientSecret}}` - Azure client secret

### Connection String Templates

**MongoDB:**
```
mongodb://{{username}}:{{password}}@mongo:27017
```

**PostgreSQL:**
```
Host=postgres;Database=service-name;Username={{username}};Password={{password}};Port=5432
```

**MySQL:**
```
Server=mysql;Database=service-name;Uid={{username}};Pwd={{password}};Port=3306
```

## Environment-Specific Configuration

### Local Development

```json
{
  "vault": {
    "enabled": true,
    "url": "http://localhost:8200",
    "authType": "token",
    "token": "secret"
  }
}
```

### Docker

```json
{
  "vault": {
    "enabled": true,
    "url": "http://vault:8200",
    "authType": "token",
    "token": "secret"
  }
}
```

### Production

```json
{
  "vault": {
    "enabled": true,
    "url": "https://vault.production.mamey.io",
    "authType": "approle",
    "roleId": "${VAULT_ROLE_ID}",
    "secretId": "${VAULT_SECRET_ID}"
  }
}
```

**Production Best Practices:**
- Use AppRole or Kubernetes authentication instead of static tokens
- Store credentials in environment variables or secure configuration
- Enable audit logging
- Rotate tokens regularly
- Use proper TLS certificates

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

var secret = await VaultSecrets.GetAsync<T>("kv/mamey/service-name/appsettings");
```

### Get Certificate

```csharp
@inject ICertificatesIssuer CertificatesIssuer

var certificate = await CertificatesIssuer.IssueAsync("service-name", "service-name.mamey.io");
```

### Access Lease Credentials

Lease credentials are automatically injected into configuration. Access them via `IConfiguration`:

```csharp
public class MyService
{
    private readonly IConfiguration _configuration;
    
    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GetMongoConnectionString()
    {
        // Lease credentials are available at: {leaseKey}:{templateProperty}
        return _configuration["mongo:connectionString"];
    }
    
    public string GetRabbitMqUsername()
    {
        return _configuration["rabbitmq:username"];
    }
    
    public string GetConsulToken()
    {
        return _configuration["consul:token"];
    }
}
```

## Service-Specific Configuration

Each microservice should have a `vault-config.md` file in its root directory with service-specific Vault configuration instructions. This file should include:

- Service-specific Vault role configurations
- Database lease configurations (MongoDB, PostgreSQL)
- RabbitMQ lease configuration (if used)
- Consul lease configuration (if used)
- PKI role configuration
- KV secrets path configuration
- Environment-specific notes

**Reference:** See `Mamey.Info/docs/vault-config.md` for Vault server setup and service-specific examples.

## Vault Server Setup

For complete Vault server setup instructions, including:
- Initial Vault configuration
- Secret engine setup
- Database connection configuration
- PKI root CA setup
- Service-specific role creation

**Reference:** `Mamey.Info/docs/vault-config.md`

## Related Libraries

- **Mamey.WebApi.Security**: Certificate authentication
- **Mamey.Persistence.MongoDB**: MongoDB persistence
- **Mamey.Persistence.PostgreSQL**: PostgreSQL persistence
- **Mamey.MessageBrokers.RabbitMQ**: RabbitMQ message broker
- **Mamey.Discovery.Consul**: Consul service discovery

## Tags

#vault #secrets #pki #certificates #hashi-corps #mamey #dynamic-secrets #lease-management
