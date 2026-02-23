# Mamey.FWID.Notifications

**Service**: Mamey.FWID.Notifications  
**Port**: 5007  
**Type**: Notification Service  
**Version**: 1.0.0

## Overview

Mamey.FWID.Notifications is a microservice that handles notifications for all FWID-related events (Identity, DID, ZKP, Credential, AccessControl). The service subscribes to integration events from all 6 FWID services and sends notifications via Email, SMS, Push, and InApp channels.

## Features

- **Multi-Channel Notifications**: Supports Email, SMS, Push, and InApp notifications
- **Email Templates**: Razor-based email templates for welcome, DID creation, credential issuance, and more
- **Real-Time Notifications**: SignalR hub for real-time in-app notifications
- **Integration Events**: Subscribes to events from all FWID services (Identities, DIDs, ZKPs, Credentials, AccessControls)
- **Service Client Verification**: Verifies data from source of truth services before processing
- **gRPC Support**: gRPC service with JWT and Certificate authentication
- **MinIO Storage**: Optional storage for notification attachments

## Architecture

### Domain Layer

- **NotificationId**: Value object that includes `IdentityId`
- **NotificationType**: Enum with flags for Email, SMS, Push, InApp
- **NotificationStatus**: Enum for Pending, Sent, Failed, Read, Deleted
- **Notification**: Aggregate root with domain methods and events

### Application Layer

- **Integration Events**: Subscribes to events from all 6 FWID services
- **Integration Commands**: Handles `SendNotificationIntegrationCommand`
- **Service Clients**: Interfaces for calling other FWID services
- **Notification Service**: Interface for sending notifications
- **Email Templates**: Razor templates for various notification types
- **SignalR Hub**: Real-time notification hub

### Infrastructure Layer

- **Service Clients**: HTTP (Identities) and gRPC (others) implementations with certificate authentication
- **Notification Service**: Implementation using `Mamey.Emails` for email sending
- **Event Mapper**: Maps domain events to integration events
- **MessageBusSubscriber**: Subscribes to integration events and commands
- **Repositories**: PostgreSQL (write model) and MongoDB (read model)
- **MinIO Storage**: Service for storing notification attachments

## How to Start the Service

### Local Development

```bash
cd FutureWampum/Mamey.FWID.Notifications/src/Mamey.FWID.Notifications.Api
dotnet run
```

By default, the service will be available at:
- **REST API**: http://localhost:5007
- **gRPC**: http://localhost:5007
- **SignalR Hub**: http://localhost:5007/notifications/hub

### Docker

```bash
# Build
docker build -t mamey.fwid.notifications .

# Run
docker run -p 5007:5007 mamey.fwid.notifications
```

## Configuration

### appsettings.json

```json
{
  "httpClient": {
    "services": {
      "identities": "https://localhost:5001",
      "dids": "https://localhost:5002",
      "zkps": "https://localhost:5003",
      "accesscontrols": "https://localhost:5004",
      "credentials": "https://localhost:5005",
      "operations": "https://localhost:5006"
    }
  },
  "email": {
    "enabled": true,
    "type": "smtp",
    "emailId": "noreply@futurewampumid.com",
    "name": "FutureWampumID",
    "mailkit": {
      "host": "smtp.example.com",
      "port": 587,
      "username": "user",
      "password": "password",
      "useSsl": true
    }
  },
  "notifications": {
    "webClientUrl": "https://app.futurewampumid.com"
  }
}
```

## API Endpoints

### REST API

- **GET** `/api/notifications/{identityId}` - Get notifications for an identity
- **GET** `/api/notifications` - Get all notifications
- **POST** `/api/notifications` - Create a new notification
- **POST** `/api/notifications/{notificationId}/read` - Mark notification as read

### gRPC Service

- **GetNotifications** - Get notifications for an identity
- **MarkAsRead** - Mark a notification as read
- **SendNotification** - Send a new notification

### SignalR Hub

- **Hub Path**: `/notifications/hub`
- **Methods**:
  - `JoinIdentityGroup(Guid identityId)` - Join identity-specific group
  - `LeaveIdentityGroup(Guid identityId)` - Leave identity-specific group
- **Client Events**:
  - `ReceiveNotification` - Receive real-time notification

## Integration Events

The service subscribes to the following integration events:

### From Identities Service
- `IdentityCreatedIntegrationEvent` - Sends welcome email and notification

### From DIDs Service
- `DIDCreatedIntegrationEvent` - Sends DID creation notification

### From ZKPs Service
- `ZKPProofGeneratedIntegrationEvent` - Sends ZKP proof generation notification

### From Credentials Service
- `CredentialIssuedIntegrationEvent` - Sends credential issuance notification

### From AccessControls Service
- `ZoneAccessGrantedIntegrationEvent` - Sends access granted notification

## Integration Commands

The service subscribes to the following integration commands:

- `SendNotificationIntegrationCommand` - Send a notification from another service

## Email Templates

The service includes the following email templates:

- **Welcome**: Sent when an identity is created
- **DIDCreated**: Sent when a DID is created
- **CredentialIssued**: Sent when a credential is issued

Templates are located in `Application/Templates/` and use Razor syntax.

## Authentication

### gRPC Authentication

The gRPC service supports two authentication methods:

1. **JWT Authentication**: For external clients
   - Header: `authorization: Bearer {token}`

2. **Certificate Authentication**: For internal service-to-service communication
   - Header: `x-client-certificate: {base64-certificate}`

### REST API Authentication

REST API endpoints currently do not require authentication (auth: false). This can be configured per endpoint in `NotificationRoutes.cs`.

## Testing

### Integration Tests

Integration tests are located in `tests/Mamey.FWID.Notifications.Tests.Integration/`.

Run tests:
```bash
cd FutureWampum/Mamey.FWID.Notifications/tests/Mamey.FWID.Notifications.Tests.Integration
dotnet test
```

### gRPC Testing

See [GRPC_INTEGRATION_TESTING.md](../GRPC_INTEGRATION_TESTING.md) for detailed gRPC testing instructions.

## Dependencies

### External Services

- **Identities Service** (Port 5001) - HTTP client for identity verification
- **DIDs Service** (Port 5002) - gRPC client for DID verification
- **ZKPs Service** (Port 5003) - gRPC client for ZKP proof verification
- **Credentials Service** (Port 5005) - gRPC client for credential verification
- **AccessControls Service** (Port 5004) - gRPC client for access control verification

### Infrastructure

- **PostgreSQL** - Write model (notifications)
- **MongoDB** - Read model (notification documents)
- **RabbitMQ** - Message broker for integration events
- **MinIO** - Object storage for notification attachments (optional)
- **SignalR** - Real-time notifications

## Service Client Pattern

All service clients follow the `SamplesServiceClient` pattern:

- Direct dependencies: `HttpClientOptions`, `ICertificatesService`, `VaultOptions`, `SecurityOptions`
- Service addresses from `options.Services["service-name"]`
- Vault-based certificate authentication for internal calls

## Event-Driven Architecture

The service uses an event-driven architecture:

1. **Integration Events**: Received from other FWID services via RabbitMQ
2. **Domain Events**: Generated by domain entities (NotificationCreated, NotificationSent, NotificationRead)
3. **Integration Events**: Published to other services (NotificationSentIntegrationEvent, NotificationReadIntegrationEvent)

## MinIO Storage

The service includes MinIO storage for notification attachments:

- **Bucket**: `notification-attachments`
- **Service**: `INotificationStorageService`
- **Operations**: Upload, Download, Delete, Presigned URLs, Metadata

## Security

- **Mamey.Security**: Applied to sensitive notification data
- **Encryption**: Sensitive fields can be encrypted using `[Encrypted]` attribute
- **Hashing**: Sensitive fields can be hashed using `[Hashed]` attribute

## Logging

The service uses structured logging with correlation IDs for request tracking.

## Health Checks

Health check endpoint: `GET /health`

## Migration

### PostgreSQL Migrations

```bash
cd FutureWampum/Mamey.FWID.Notifications/src/Mamey.FWID.Notifications.Infrastructure
dotnet ef migrations add Initial --project ./Mamey.FWID.Notifications.Infrastructure.csproj -s ../Mamey.FWID.Notifications.Api/Mamey.FWID.Notifications.Api.csproj -o PostgreSQL/Migrations --context NotificationDbContext
```

## License

Copyright © Mamey Technologies. All rights reserved.







