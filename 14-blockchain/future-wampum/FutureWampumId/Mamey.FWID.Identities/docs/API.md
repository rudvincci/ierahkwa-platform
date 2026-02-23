# FutureWampumID Identity Service API Documentation

## Overview

The FutureWampumID Identity Service provides a comprehensive API for managing digital identities, including registration, verification, biometric management, and permission synchronization. This service is part of the FutureWampumID (FWID) ecosystem and supports both JWT and certificate-based authentication.

**Base URL**: `http://localhost:5001` (Development)  
**API Version**: `v1`  
**Swagger UI**: `http://localhost:5001/docs`

## Table of Contents

1. [Authentication](#authentication)
2. [Authorization](#authorization)
3. [API Endpoints](#api-endpoints)
4. [Data Models](#data-models)
5. [Error Handling](#error-handling)
6. [ACL Configuration](#acl-configuration)
7. [Permission Definitions](#permission-definitions)
8. [Environment-Specific Settings](#environment-specific-settings)

---

## Authentication

The Identity Service supports multiple authentication methods through the Multi-Authentication system:

### 1. JWT Authentication (Primary)

JWT tokens are the primary authentication method for user authentication. Tokens are issued by the authentication service and must be included in the `Authorization` header:

```
Authorization: Bearer <jwt-token>
```

**Configuration**:
- **Algorithm**: HS512
- **Issuer**: `auth.futurebdetbank.com`
- **Audience**: `localhost`
- **Token Expiry**: 30 minutes (default)
- **Refresh Token Lifetime**: 30 minutes

**Endpoints**:
- `POST /api/auth/sign-in` - Sign in with username/password
- `POST /api/auth/sign-out` - Sign out and revoke session
- `POST /api/auth/refresh` - Refresh access token

### 2. DID Authentication (Secondary)

Decentralized Identity (DID) authentication is a secondary authentication method that requires device registration. Users must register their device before using DID authentication.

```
Authorization: DidBearer <did-token>
X-Device-Id: <device-id>
```

**Endpoints**:
- Device registration (via DIDs service)
- DID-based sign-in (requires registered device)

### 3. Azure AD Authentication (Optional)

Azure AD, Azure B2B, and Azure B2C authentication can be enabled for enterprise integration.

**Configuration**: See `multiAuth.azure` section in `appsettings.json`

### 4. Certificate Authentication (Service-to-Service)

Certificate-based authentication is used for service-to-service communication. Services must present a valid X.509 certificate that matches the ACL configuration.

**Certificate Requirements**:
- Must be issued by a trusted Certificate Authority (CA)
- Subject (CN) must match the service name in ACL configuration
- Must be valid (not expired)
- Must have required permissions for the requested operation

**Certificate Location**: `certs/localhost.pfx` (default)

### Multi-Authentication Policy

The service supports different authentication policies configured via `multiAuth.policy`:

- **EitherOr**: Accept any enabled authentication method
- **JwtOnly**: Only JWT authentication is accepted
- **PriorityOrder**: Try authentication methods in priority order
- **AllRequired**: All enabled authentication methods must succeed

---

## Authorization

### Permission Hierarchy

The Identity Service implements a hierarchical permission system:

```
identities:admin > identities:write > identities:verify > identities:read
```

**Permission Hierarchy Rules**:
- `identities:admin` grants all permissions (admin, write, verify, read)
- `identities:write` grants write, verify, and read permissions
- `identities:verify` grants verify and read permissions
- `identities:read` grants only read permissions

### Permission Definitions

| Permission | Description | Operations |
|-----------|-------------|------------|
| `identities:read` | Read identity information | GET `/api/identities`, GET `/api/identities/{id}` |
| `identities:verify` | Verify biometric data | POST `/api/identities/{id}/verify` |
| `identities:write` | Create and update identities | POST `/api/identities`, PUT `/api/identities/{id}/*` |
| `identities:admin` | Full administrative access | All operations, including permission synchronization |

### ACL Configuration

Access Control Lists (ACLs) define which services can access which endpoints. ACLs are configured in `appsettings.json`:

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
        },
        "sagas-service": {
          "validIssuer": "localhost",
          "permissions": ["identities:read", "identities:write"]
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

**ACL Configuration Properties**:
- **Service Name**: Must match the certificate subject (CN)
- **validIssuer**: The certificate issuer that must match
- **permissions**: List of permissions granted to this service

---

## API Endpoints

### Identity Management

#### Register Identity

**POST** `/api/identities`

Registers a new identity in the system.

**Authentication**: Not required (public endpoint)

**Request Body**:
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": {
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "suffix": "Jr."
  },
  "personalDetails": {
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "placeOfBirth": "Springfield, IL",
    "gender": "Male",
    "nationality": "USA"
  },
  "contactInformation": {
    "email": "john.doe@example.com",
    "phone": {
      "number": "555-1234",
      "type": "Mobile"
    },
    "address": {
      "street": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA",
      "type": "Residential"
    }
  },
  "biometricData": {
    "type": "Fingerprint",
    "data": "base64encodedfingerprintdata",
    "hash": "sha256hash"
  },
  "zone": "zone-001",
  "clanRegistrarId": null
}
```

**Response**: `201 Created`
```json
{
  "message": "Identity registered successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data or validation errors
- `409 Conflict`: Identity already exists

---

#### Get Identity

**GET** `/api/identities/{identityId}`

Retrieves an identity by ID.

**Authentication**: Not required (public endpoint)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
{
  "id": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "name": {
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "suffix": "Jr."
  },
  "personalDetails": {
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "placeOfBirth": "Springfield, IL",
    "gender": "Male",
    "nationality": "USA"
  },
  "contactInformation": {
    "email": "john.doe@example.com",
    "phone": {
      "number": "555-1234",
      "type": "Mobile"
    },
    "address": {
      "street": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA",
      "type": "Residential"
    }
  },
  "biometricData": {
    "type": "Fingerprint",
    "hash": "sha256hash"
  },
  "zone": "zone-001",
  "status": "Verified",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `400 Bad Request`: Invalid identity ID format

---

#### Find Identities

**GET** `/api/identities`

Retrieves a list of identities, optionally filtered by zone and status.

**Authentication**: Not required (public endpoint)

**Query Parameters**:
- `zone` (string, optional): Filter by zone
- `status` (string, optional): Filter by status (Pending, Verified, Revoked)

**Response**: `200 OK`
```json
[
  {
    "id": {
      "value": "123e4567-e89b-12d3-a456-426614174000"
    },
    "name": {
      "firstName": "John",
      "lastName": "Doe"
    },
    "status": "Verified",
    "zone": "zone-001"
  }
]
```

---

#### Verify Biometric

**POST** `/api/identities/{identityId}/verify`

Verifies biometric data against a stored identity.

**Authentication**: Not required (public endpoint)  
**Authorization**: `identities:verify` (for certificate authentication)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Request Body**:
```json
{
  "providedBiometric": {
    "type": "Fingerprint",
    "data": "base64encodedfingerprintdata",
    "hash": "sha256hash"
  },
  "threshold": 0.95
}
```

**Response**: `200 OK`
```json
{
  "message": "Biometric verified successfully"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions

---

#### Update Biometric

**PUT** `/api/identities/{identityId}/biometric`

Updates biometric data for an identity.

**Authentication**: Not required (public endpoint)  
**Authorization**: `identities:write` (for certificate authentication)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Request Body**:
```json
{
  "newBiometric": {
    "type": "Facial",
    "data": "base64encodedfacialdata",
    "hash": "sha256hash"
  },
  "verificationBiometric": {
    "type": "Fingerprint",
    "data": "base64encodedfingerprintdata",
    "hash": "sha256hash"
  }
}
```

**Response**: `200 OK`
```json
{
  "message": "Biometric updated successfully"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `400 Bad Request`: Invalid biometric data or verification failed
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions

---

#### Revoke Identity

**POST** `/api/identities/{identityId}/revoke`

Revokes an identity, marking it as revoked.

**Authentication**: Not required (public endpoint)  
**Authorization**: `identities:write` (for certificate authentication)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Request Body**:
```json
{
  "identityId": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "reason": "Identity revoked by administrator",
  "revokedBy": "789e0123-e45b-67c8-d901-234567890abc"
}
```

**Response**: `200 OK`
```json
{
  "message": "Identity revoked successfully"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions

---

#### Update Zone

**PUT** `/api/identities/{identityId}/zone`

Updates the zone for an identity.

**Authentication**: Not required (public endpoint)  
**Authorization**: `identities:write` (for certificate authentication)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Request Body**:
```json
{
  "identityId": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "zone": "zone-002"
}
```

**Response**: `200 OK`
```json
{
  "message": "Zone updated successfully"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `400 Bad Request`: Invalid zone value
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions

---

#### Update Contact Information

**PUT** `/api/identities/{identityId}/contact`

Updates contact information for an identity.

**Authentication**: Not required (public endpoint)  
**Authorization**: `identities:write` (for certificate authentication)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Request Body**:
```json
{
  "identityId": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "contactInformation": {
    "email": "new.email@example.com",
    "phone": {
      "number": "555-9999",
      "type": "Mobile"
    },
    "address": {
      "street": "456 New St",
      "city": "Chicago",
      "state": "IL",
      "zipCode": "60601",
      "country": "USA",
      "type": "Residential"
    }
  }
}
```

**Response**: `200 OK`
```json
{
  "message": "Contact information updated successfully"
}
```

**Error Responses**:
- `404 Not Found`: Identity not found
- `400 Bad Request`: Invalid contact information
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions

---

### Permission Synchronization

#### Sync Permissions

**POST** `/api/permissions/sync`

Synchronizes permission scopes for an external service. This endpoint allows other microservices to register their permission requirements with the Identity Service.

**Authentication**: Required (certificate authentication)  
**Authorization**: `identities:admin`

**Request Body**:
```json
{
  "serviceName": "dids-service",
  "permissions": {
    "identities:read": "Read identity information",
    "identities:verify": "Verify biometric data"
  },
  "certificateSubject": "CN=dids-service",
  "certificateIssuer": "CN=localhost",
  "certificateThumbprint": "abc123def456..."
}
```

**Response**: `200 OK`
```json
{
  "message": "Permissions synchronized successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid or missing certificate
- `403 Forbidden`: Insufficient permissions

---

## Data Models

### IdentityDto

```json
{
  "id": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "name": {
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "suffix": "Jr."
  },
  "personalDetails": {
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "placeOfBirth": "Springfield, IL",
    "gender": "Male",
    "nationality": "USA"
  },
  "contactInformation": {
    "email": "john.doe@example.com",
    "phone": {
      "number": "555-1234",
      "type": "Mobile"
    },
    "address": {
      "street": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA",
      "type": "Residential"
    }
  },
  "biometricData": {
    "type": "Fingerprint",
    "hash": "sha256hash"
  },
  "zone": "zone-001",
  "status": "Verified",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

### IdentityStatus

- `Pending`: Identity is pending verification
- `Verified`: Identity has been verified
- `Revoked`: Identity has been revoked

### BiometricType

- `Fingerprint`: Fingerprint biometric
- `Facial`: Facial recognition biometric
- `Iris`: Iris scan biometric
- `Voice`: Voice recognition biometric
- `DNA`: DNA-based biometric

---

## Error Handling

### Error Response Format

All error responses follow this format:

```json
{
  "code": "identity_not_found",
  "reason": "Identity with ID: '123e4567-e89b-12d3-a456-426614174000' was not found."
}
```

### HTTP Status Codes

| Status Code | Description | Common Causes |
|-------------|-------------|----------------|
| `200 OK` | Request successful | - |
| `201 Created` | Resource created successfully | - |
| `400 Bad Request` | Invalid request data | Missing required fields, validation errors |
| `401 Unauthorized` | Authentication required | Missing or invalid JWT token/certificate |
| `403 Forbidden` | Insufficient permissions | Certificate doesn't have required permissions |
| `404 Not Found` | Resource not found | Identity ID doesn't exist |
| `409 Conflict` | Resource conflict | Identity already exists |
| `500 Internal Server Error` | Server error | Unexpected server error |

### Common Error Codes

| Error Code | Description |
|------------|-------------|
| `identity_not_found` | Identity with the specified ID was not found |
| `identity_already_exists` | Identity with the specified ID already exists |
| `invalid_biometric_data` | Biometric data validation failed |
| `biometric_verification_failed` | Biometric verification did not match |
| `invalid_certificate` | Certificate validation failed |
| `insufficient_permissions` | Certificate doesn't have required permissions |

---

## ACL Configuration

### Configuration Structure

ACL configuration is defined in `appsettings.json` under `security.certificate.acl`:

```json
{
  "security": {
    "certificate": {
      "acl": {
        "service-name": {
          "validIssuer": "issuer-name",
          "permissions": ["permission1", "permission2"]
        }
      }
    }
  }
}
```

### Service-Specific ACLs

The following services have pre-configured ACLs:

#### DIDs Service
```json
{
  "dids-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Credentials Service
```json
{
  "credentials-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### ZKPs Service
```json
{
  "zkps-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Access Controls Service
```json
{
  "access-controls-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Operations Service
```json
{
  "operations-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

#### Sagas Service
```json
{
  "sagas-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

#### Notifications Service
```json
{
  "notifications-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read"]
  }
}
```

#### API Gateway
```json
{
  "api-gateway": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

---

## Permission Definitions

### Permission Scope Format

Permissions follow the format: `{resource}:{action}`

### Available Permissions

| Permission | Resource | Action | Description |
|-----------|----------|--------|-------------|
| `identities:read` | identities | read | Read identity information |
| `identities:verify` | identities | verify | Verify biometric data |
| `identities:write` | identities | write | Create and update identities |
| `identities:admin` | identities | admin | Full administrative access |

### Permission Hierarchy

```
identities:admin
  ├── identities:write
  │   ├── identities:verify
  │   │   └── identities:read
  │   └── identities:read
  └── identities:verify
      └── identities:read
```

**Rules**:
- `identities:admin` implicitly grants all other permissions
- `identities:write` implicitly grants `identities:verify` and `identities:read`
- `identities:verify` implicitly grants `identities:read`

---

## Environment-Specific Settings

### Development Environment

**File**: `appsettings.Development.json`

**Key Settings**:
- **PostgreSQL**: `localhost:5432`
- **MongoDB**: `localhost:27017`
- **Redis**: `localhost:6379`
- **MinIO**: `localhost:9000`
- **RabbitMQ**: `localhost:5672`
- **Swagger**: Enabled at `/docs`

### Docker Environment

**File**: `appsettings.Docker.json`

**Key Settings**:
- **PostgreSQL**: `postgres:5432`
- **MongoDB**: `mongo:27017`
- **Redis**: `redis:6379`
- **MinIO**: `minio:9000`
- **RabbitMQ**: `rabbitmq:5672`
- **Swagger**: Enabled at `/docs`

### Local Environment

**File**: `appsettings.Local.json`

**Key Settings**:
- **PostgreSQL**: `localhost:5432`
- **MongoDB**: `localhost:27017`
- **Redis**: `localhost:6379`
- **MinIO**: `localhost:9000`
- **RabbitMQ**: `localhost:5672`
- **Swagger**: Enabled at `/docs`

### Production Environment

**File**: `appsettings.Production.json` (not included in repository)

**Key Settings**:
- **PostgreSQL**: Production connection string (from secrets)
- **MongoDB**: Production connection string (from secrets)
- **Redis**: Production connection string (from secrets)
- **MinIO**: Production endpoint (from secrets)
- **RabbitMQ**: Production endpoint (from secrets)
- **Swagger**: Disabled (security best practice)

---

## Additional Resources

- **Swagger UI**: `http://localhost:5001/docs` (Development)
- **Health Check**: `http://localhost:5001/health`
- **gRPC Endpoints**: See `Protos/` directory for Protocol Buffer definitions
- **Integration Guide**: See `docs/Integration.md` for service integration details

---

## Support

For issues, questions, or contributions, please contact the Mamey Technologies team or refer to the project documentation.

**Copyright**: Mamey Technologies (mamey.io)  
**License**: AGPL-3.0


- `409 Conflict`: Permission already exists

---

#### Update Permission

**PUT** `/api/auth/permissions/{permissionId}`

Updates an existing permission.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:write`

**Path Parameters**:
- `permissionId` (Guid): The permission identifier

**Request Body**:
```json
{
  "name": "test:permission:updated",
  "description": "Updated permission description"
}
```

**Response**: `200 OK`
```json
{
  "message": "Permission updated successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Permission not found

---

#### Delete Permission

**POST** `/api/auth/permissions/{permissionId}/delete`

Deletes a permission.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:write`

**Path Parameters**:
- `permissionId` (Guid): The permission identifier

**Response**: `200 OK`
```json
{
  "message": "Permission deleted successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Permission not found

---

#### Assign Permission to Identity

**POST** `/api/auth/identities/{identityId}/permissions/{permissionId}`

Assigns a permission to an identity.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:write`

**Path Parameters**:
- `identityId` (Guid): The identity identifier
- `permissionId` (Guid): The permission identifier

**Response**: `200 OK`
```json
{
  "message": "Permission assigned successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity or permission not found
- `409 Conflict`: Permission already assigned

---

#### Remove Permission from Identity

**POST** `/api/auth/identities/{identityId}/permissions/{permissionId}/remove`

Removes a permission from an identity.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:write`

**Path Parameters**:
- `identityId` (Guid): The identity identifier
- `permissionId` (Guid): The permission identifier

**Response**: `200 OK`
```json
{
  "message": "Permission removed successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity, permission, or assignment not found

---

#### Get Identity Permissions

**GET** `/api/auth/identities/{identityId}/permissions`

Retrieves all permissions assigned to an identity.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:read`

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
[
  {
    "permissionId": "123e4567-e89b-12d3-a456-426614174000",
    "name": "test:permission",
    "description": "Test permission",
    "assignedAt": "2024-01-15T10:00:00Z"
  }
]
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity not found

---

### Role Management Endpoints

#### Create Role

**POST** `/api/auth/roles`

Creates a new role.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Request Body**:
```json
{
  "name": "admin",
  "description": "Administrator role"
}
```

**Response**: `201 Created`
```json
{
  "message": "Role created successfully",
  "roleId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid role name
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `409 Conflict`: Role already exists

---

#### Update Role

**PUT** `/api/auth/roles/{roleId}`

Updates an existing role.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `roleId` (Guid): The role identifier

**Request Body**:
```json
{
  "name": "admin:updated",
  "description": "Updated administrator role"
}
```

**Response**: `200 OK`
```json
{
  "message": "Role updated successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Role not found

---

#### Delete Role

**POST** `/api/auth/roles/{roleId}/delete`

Deletes a role.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `roleId` (Guid): The role identifier

**Response**: `200 OK`
```json
{
  "message": "Role deleted successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Role not found

---

#### Assign Role to Identity

**POST** `/api/auth/identities/{identityId}/roles/{roleId}`

Assigns a role to an identity.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `identityId` (Guid): The identity identifier
- `roleId` (Guid): The role identifier

**Response**: `200 OK`
```json
{
  "message": "Role assigned successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity or role not found
- `409 Conflict`: Role already assigned

---

#### Remove Role from Identity

**POST** `/api/auth/identities/{identityId}/roles/{roleId}/remove`

Removes a role from an identity.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `identityId` (Guid): The identity identifier
- `roleId` (Guid): The role identifier

**Response**: `200 OK`
```json
{
  "message": "Role removed successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity, role, or assignment not found

---

#### Get Identity Roles

**GET** `/api/auth/identities/{identityId}/roles`

Retrieves all roles assigned to an identity.

**Authentication**: Required (JWT)  
**Authorization**: `roles:read`

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
[
  {
    "roleId": "123e4567-e89b-12d3-a456-426614174000",
    "name": "admin",
    "description": "Administrator role",
    "assignedAt": "2024-01-15T10:00:00Z"
  }
]
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Identity not found

---

### Email Confirmation Endpoints

#### Create Email Confirmation

**POST** `/api/auth/email/confirm/create`

Creates an email confirmation request and sends a confirmation email.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com"
}
```

**Response**: `201 Created`
```json
{
  "message": "Email confirmation created",
  "confirmationId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid email address
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found
- `429 Too Many Requests`: Too many confirmation requests

---

#### Confirm Email

**POST** `/api/auth/email/confirm`

Confirms an email address using a confirmation token.

**Authentication**: Not required (public endpoint)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "token": "confirmation-token-123"
}
```

**Response**: `200 OK`
```json
{
  "message": "Email confirmed successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid or expired token
- `404 Not Found`: Confirmation not found

---

#### Resend Email Confirmation

**POST** `/api/auth/email/confirm/resend`

Resends an email confirmation.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com"
}
```

**Response**: `200 OK`
```json
{
  "message": "Email confirmation resent"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid email address
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found
- `429 Too Many Requests`: Too many confirmation requests

---

#### Get Email Confirmation Status

**GET** `/api/auth/email/confirm/status/{identityId}`

Retrieves the email confirmation status for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com",
  "emailConfirmed": true,
  "emailConfirmedAt": "2024-01-15T10:00:00Z"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found

---

### SMS Confirmation Endpoints

#### Create SMS Confirmation

**POST** `/api/auth/sms/confirm/create`

Creates an SMS confirmation request and sends a confirmation code via SMS.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "phoneNumber": "+1234567890"
}
```

**Response**: `201 Created`
```json
{
  "message": "SMS confirmation created",
  "confirmationId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid phone number
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found
- `429 Too Many Requests`: Too many confirmation requests

---

#### Confirm SMS

**POST** `/api/auth/sms/confirm`

Confirms a phone number using a confirmation code.

**Authentication**: Not required (public endpoint)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "code": "123456"
}
```

**Response**: `200 OK`
```json
{
  "message": "SMS confirmed successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid or expired code
- `404 Not Found`: Confirmation not found

---

#### Resend SMS Confirmation

**POST** `/api/auth/sms/confirm/resend`

Resends an SMS confirmation code.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "phoneNumber": "+1234567890"
}
```

**Response**: `200 OK`
```json
{
  "message": "SMS confirmation resent"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid phone number
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found
- `429 Too Many Requests`: Too many confirmation requests

---

#### Get SMS Confirmation Status

**GET** `/api/auth/sms/confirm/status/{identityId}`

Retrieves the SMS confirmation status for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "phoneNumber": "+1234567890",
  "phoneConfirmed": true,
  "phoneConfirmedAt": "2024-01-15T10:00:00Z"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Identity not found

---

### Permission Synchronization

#### Sync Permissions

**POST** `/api/permissions/sync`

Synchronizes permission scopes for an external service. This endpoint allows other microservices to register their permission requirements with the Identity Service.

**Authentication**: Required (certificate authentication)  
**Authorization**: `identities:admin`

**Request Body**:
```json
{
  "serviceName": "dids-service",
  "permissions": ["identities:read", "identities:verify"]
}
```

**Response**: `200 OK`
```json
{
  "message": "Permissions synchronized successfully"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid or missing certificate
- `403 Forbidden`: Insufficient permissions

---

## Data Models

### IdentityDto

```json
{
  "id": {
    "value": "123e4567-e89b-12d3-a456-426614174000"
  },
  "name": {
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "suffix": "Jr."
  },
  "personalDetails": {
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "placeOfBirth": "Springfield, IL",
    "gender": "Male",
    "nationality": "USA"
  },
  "contactInformation": {
    "email": "john.doe@example.com",
    "phone": {
      "number": "555-1234",
      "type": "Mobile"
    },
    "address": {
      "street": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA",
      "type": "Residential"
    }
  },
  "biometricData": {
    "type": "Fingerprint",
    "hash": "sha256hash"
  },
  "zone": "zone-001",
  "status": "Verified",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

### IdentityStatus

- `Pending`: Identity is pending verification
- `Verified`: Identity has been verified
- `Revoked`: Identity has been revoked

### BiometricType

- `Fingerprint`: Fingerprint biometric
- `Facial`: Facial recognition biometric
- `Iris`: Iris scan biometric
- `Voice`: Voice recognition biometric
- `DNA`: DNA-based biometric

---

## Error Handling

### Error Response Format

All error responses follow this format:

```json
{
  "code": "identity_not_found",
  "reason": "Identity with ID: '123e4567-e89b-12d3-a456-426614174000' was not found."
}
```

### HTTP Status Codes

| Status Code | Description | Common Causes |
|-------------|-------------|----------------|
| `200 OK` | Request successful | - |
| `201 Created` | Resource created successfully | - |
| `400 Bad Request` | Invalid request data | Missing required fields, validation errors |
| `401 Unauthorized` | Authentication required | Missing or invalid JWT token/certificate |
| `403 Forbidden` | Insufficient permissions | Certificate doesn't have required permissions |
| `404 Not Found` | Resource not found | Identity ID doesn't exist |
| `409 Conflict` | Resource conflict | Identity already exists |
| `500 Internal Server Error` | Server error | Unexpected server error |

### Common Error Codes

| Error Code | Description |
|------------|-------------|
| `identity_not_found` | Identity with the specified ID was not found |
| `identity_already_exists` | Identity with the specified ID already exists |
| `invalid_biometric_data` | Biometric data validation failed |
| `biometric_verification_failed` | Biometric verification did not match |
| `invalid_certificate` | Certificate validation failed |
| `insufficient_permissions` | Certificate doesn't have required permissions |

---

## ACL Configuration

### Configuration Structure

ACL configuration is defined in `appsettings.json` under `security.certificate.acl`:

```json
{
  "security": {
    "certificate": {
      "acl": {
        "service-name": {
          "validIssuer": "issuer-name",
          "permissions": ["permission1", "permission2"]
        }
      }
    }
  }
}
```

### Service-Specific ACLs

The following services have pre-configured ACLs:

#### DIDs Service
```json
{
  "dids-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Credentials Service
```json
{
  "credentials-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### ZKPs Service
```json
{
  "zkps-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Access Controls Service
```json
{
  "access-controls-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:verify"]
  }
}
```

#### Operations Service
```json
{
  "operations-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

#### Sagas Service
```json
{
  "sagas-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

#### Notifications Service
```json
{
  "notifications-service": {
    "validIssuer": "localhost",
    "permissions": ["identities:read"]
  }
}
```

#### API Gateway
```json
{
  "api-gateway": {
    "validIssuer": "localhost",
    "permissions": ["identities:read", "identities:write"]
  }
}
```

---

## Permission Definitions

### Permission Scope Format

Permissions follow the format: `{resource}:{action}`

### Available Permissions

| Permission | Resource | Action | Description |
|-----------|----------|--------|-------------|
| `identities:read` | identities | read | Read identity information |
| `identities:verify` | identities | verify | Verify biometric data |
| `identities:write` | identities | write | Create and update identities |
| `identities:admin` | identities | admin | Full administrative access |

### Permission Hierarchy

```
identities:admin
  ├── identities:write
  │   ├── identities:verify
  │   │   └── identities:read
  │   └── identities:read
  └── identities:verify
      └── identities:read
```

**Rules**:
- `identities:admin` implicitly grants all other permissions
- `identities:write` implicitly grants `identities:verify` and `identities:read`
- `identities:verify` implicitly grants `identities:read`

---

## Environment-Specific Settings

### Development Environment

**File**: `appsettings.Development.json`

**Key Settings**:
- **PostgreSQL**: `localhost:5432`
- **MongoDB**: `localhost:27017`
- **Redis**: `localhost:6379`
- **MinIO**: `localhost:9000`
- **RabbitMQ**: `localhost:5672`
- **Swagger**: Enabled at `/docs`

### Docker Environment

**File**: `appsettings.Docker.json`

**Key Settings**:
- **PostgreSQL**: `postgres:5432`
- **MongoDB**: `mongo:27017`
- **Redis**: `redis:6379`
- **MinIO**: `minio:9000`
- **RabbitMQ**: `rabbitmq:5672`
- **Swagger**: Enabled at `/docs`

### Local Environment

**File**: `appsettings.Local.json`

**Key Settings**:
- **PostgreSQL**: `localhost:5432`
- **MongoDB**: `localhost:27017`
- **Redis**: `localhost:6379`
- **MinIO**: `localhost:9000`
- **RabbitMQ**: `localhost:5672`
- **Swagger**: Enabled at `/docs`

### Production Environment

**File**: `appsettings.Production.json` (not included in repository)

**Key Settings**:
- **PostgreSQL**: Production connection string (from secrets)
- **MongoDB**: Production connection string (from secrets)
- **Redis**: Production connection string (from secrets)
- **MinIO**: Production endpoint (from secrets)
- **RabbitMQ**: Production endpoint (from secrets)
- **Swagger**: Disabled (security best practice)

---

## Additional Resources

- **Swagger UI**: `http://localhost:5001/docs` (Development)
- **Health Check**: `http://localhost:5001/health`
- **gRPC Endpoints**: See `Protos/` directory for Protocol Buffer definitions
- **Integration Guide**: See `docs/Integration.md` for service integration details

---

## Support

For issues, questions, or contributions, please contact the Mamey Technologies team or refer to the project documentation.

**Copyright**: Mamey Technologies (mamey.io)  
**License**: AGPL-3.0

