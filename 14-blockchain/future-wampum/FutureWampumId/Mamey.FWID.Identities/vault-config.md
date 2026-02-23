# Vault Configuration - Mamey.FWID.Identities

## Overview

This document defines the HashiCorp Vault configuration for the Mamey.FWID.Identities microservice.

## Service Details

- **Service Name**: Mamey.FWID.Identities
- **Port**: 5001
- **Domain**: mamey.io

## Vault Role Configuration

### Role: identities

```hcl
path "database/creds/postgres-identities" {
  capabilities = ["read"]
}

path "database/creds/mongo-identities" {
  capabilities = ["read"]
}

path "rabbitmq/creds/identities" {
  capabilities = ["read"]
}

path "consul/creds/identities" {
  capabilities = ["read"]
}

path "kv/data/identities/*" {
  capabilities = ["read"]
}

path "pki_int/issue/mamey.io" {
  capabilities = ["create", "update"]
}

path "pki_int/certs/*" {
  capabilities = ["read"]
}
```

## Lease Types

### MongoDB Lease
- **Type**: database
- **Role**: identities
- **Templates**:
  - `connectionString`: `mongodb://{{username}}:{{password}}@mongo:27017`

### PostgreSQL Lease
- **Type**: database
- **Role**: identities-postgres
- **Templates**:
  - `connectionString`: `Host=postgres;Database=identities;Username={{username}};Password={{password}};Port=5432`

### RabbitMQ Lease
- **Type**: rabbitmq
- **Role**: identities
- **Templates**:
  - `connectionString`: `amqp://{{username}}:{{password}}@rabbitmq:5672`

### Consul Lease
- **Type**: consul
- **Role**: identities
- **Templates**:
  - `token`: `{{token}}`

## PKI Configuration

### Certificate Authority
- **Path**: `pki_int/issue/mamey.io`
- **Common Name**: `identities.mamey.io`
- **Allowed Domains**: `mamey.io`
- **Allow Subdomains**: `true`
- **Allow Localhost**: `true`

## Environment-Specific Notes

### Development
- Uses localhost certificates for development
- MongoDB and PostgreSQL may use default credentials

### Production
- All credentials retrieved from Vault
- Certificates auto-renewed via Vault Agent
- Database credentials rotated regularly

## Security Considerations

- Service account has minimal required permissions
- Database credentials are short-lived (1 hour leases)
- PKI certificates are auto-renewed
- Audit logging enabled for all operations</content>
<parameter name="filePath">FutureWampum/FutureWampumId/Mamey.FWID.Identities/vault-config.md