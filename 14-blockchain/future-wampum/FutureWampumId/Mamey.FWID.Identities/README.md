![MDS](https://www.mameydigitalsolutions.com)

# Mamey.FWID.Identities

**What is Mamey.FWID.Identities?**
----------------

Mamey.FWID.Identities is the **central authentication and identity provider** for the FutureWampumID ecosystem. It provides comprehensive authentication, authorization, multi-factor authentication, and identity management capabilities for all applications (BIIS, SICB, Future BDET Bank, Government, Holistic Medicine, RedWebNetwork).

**Service**: Mamey.FWID.Identities  
**Port**: 5001  
**Type**: Identity & Authentication Service  
**Version**: 1.0.0

## Features

- **Multi-Authentication**: Supports JWT (primary), DID (secondary), Azure AD/B2B/B2C, and Certificate authentication
- **Multi-Factor Authentication (MFA)**: TOTP, SMS, Email, Biometric, and Backup Code support
- **Session Management**: Secure session handling with automatic cleanup
- **Permission Management**: Fine-grained permission system with role-based access control (RBAC)
- **Email/SMS Confirmation**: Email and phone number verification workflows
- **Integration Events**: Event-driven architecture with RabbitMQ integration
- **Background Services**: Automated cleanup of expired sessions and confirmations

## Quick Start

**How to start the application?**
----------------

Service can be started locally via `dotnet run` command (executed in the `/src/Mamey.FWID.Identities.Api` directory) or by running `./scripts/start.sh` shell script in the root folder of repository.

By default, the service will be available under http://localhost:5001.

You can also start the service via Docker, either by building a local Dockerfile: 

```bash
docker build -t Mamey.FWID.Identities . 
```

or using the official one: 

```bash
docker pull mamey.azurecr.io/Mamey.FWID.Identities
```

## API Documentation

**What HTTP requests can be sent to the microservice API?**
----------------

You can find the list of all HTTP requests in [Mamey.FWID.Identities.rest](https://dev.azure.com/mds-pr/MDS/_git/Mamey.FWID.Identities?path=%2FMamey.FWID.Identities.rest) file placed in the root folder of the repository.
This file is compatible with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin for [Visual Studio Code](https://code.visualstudio.com).

### API Endpoints

- **Authentication**: `/api/auth/sign-in`, `/api/auth/sign-out`, `/api/auth/refresh`
- **MFA**: `/api/auth/mfa/setup`, `/api/auth/mfa/enable`, `/api/auth/mfa/verify`
- **Permissions**: `/api/auth/permissions`, `/api/auth/identities/{id}/permissions`
- **Roles**: `/api/auth/roles`, `/api/auth/identities/{id}/roles`
- **Confirmations**: `/api/auth/confirmations/email`, `/api/auth/confirmations/sms`

For complete API documentation, see [docs/API.md](docs/API.md) and [docs/AUTHENTICATION.md](docs/AUTHENTICATION.md).

## Architecture

For detailed architecture documentation, see [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

### Key Components

- **Domain Layer**: Identity, Session, MfaConfiguration, Role, Permission aggregates
- **Application Layer**: Authentication, MFA, Permission, Role, Confirmation services
- **Infrastructure Layer**: PostgreSQL (write), MongoDB (read), Redis (cache), RabbitMQ (events)

## Configuration

### Multi-Authentication

Configure authentication methods in `appsettings.json`:

```json
{
  "multiAuth": {
    "enabled": true,
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": false,
    "policy": "EitherOr"
  }
}
```

### MFA Configuration

```json
{
  "mfa": {
    "enabled": true,
    "defaultMethod": "TOTP",
    "totpIssuer": "FutureWampumID",
    "backupCodeCount": 10
  }
}
```

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for complete configuration options.

## Database Migrations

Adding Migrations from the Infrastructure folder:
```bash
dotnet ef migrations add Initial --project ./Mamey.FWID.Identities.Infrastructure.csproj  -s ../Mamey.FWID.Identities.Api/Mamey.FWID.Identities.Api.csproj -o EF/Migrations --context IdentityDbContext
```

## Testing

The service includes comprehensive test coverage:

- **Unit Tests**: Domain entities, services, handlers
- **Integration Tests**: Services, repositories, background workers
- **End-to-End Tests**: Complete authentication flows, MFA flows, permission/role flows

Run tests:
```bash
dotnet test
```

## Documentation

- [API Documentation](docs/API.md) - Complete API reference
- [Authentication API](docs/AUTHENTICATION.md) - Authentication endpoints documentation
- [Architecture Documentation](docs/ARCHITECTURE.md) - System architecture with diagrams

## Support

For issues, questions, or contributions, please contact the Mamey Technologies team or refer to the project documentation.

**Copyright**: Mamey Technologies (mamey.io)  
**License**: AGPL-3.0