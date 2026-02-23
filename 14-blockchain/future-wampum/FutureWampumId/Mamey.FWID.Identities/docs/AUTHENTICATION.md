# Authentication & Authorization API Documentation

## Overview

The FutureWampumID Identity Service provides comprehensive authentication and authorization capabilities through a multi-authentication system. This document covers all authentication-related endpoints, MFA, permissions, roles, and confirmations.

**Base URL**: `http://localhost:5001` (Development)  
**API Version**: `v1`  
**Swagger UI**: `http://localhost:5001/docs`

## Table of Contents

1. [Authentication Methods](#authentication-methods)
2. [Authentication Endpoints](#authentication-endpoints)
3. [Multi-Factor Authentication (MFA)](#multi-factor-authentication-mfa)
4. [Permission Management](#permission-management)
5. [Role Management](#role-management)
6. [Email Confirmation](#email-confirmation)
7. [SMS Confirmation](#sms-confirmation)
8. [Session Management](#session-management)

---

## Authentication Methods

### 1. JWT Authentication (Primary)

JWT tokens are the primary authentication method for user authentication.

**Header Format**:
```
Authorization: Bearer <jwt-token>
```

**Configuration**:
- Algorithm: HS512
- Issuer: `auth.futurebdetbank.com`
- Audience: `localhost`
- Token Expiry: 30 minutes (default)
- Refresh Token Lifetime: 30 minutes

### 2. DID Authentication (Secondary)

Decentralized Identity (DID) authentication requires device registration.

**Header Format**:
```
Authorization: DidBearer <did-token>
X-Device-Id: <device-id>
```

### 3. Azure AD Authentication (Optional)

Azure AD, Azure B2B, and Azure B2C authentication can be enabled for enterprise integration.

**Configuration**: See `multiAuth.azure` section in `appsettings.json`

### 4. Certificate Authentication (Service-to-Service)

Certificate-based authentication for service-to-service communication.

**Certificate Location**: `certs/localhost.pfx` (default)

### Multi-Authentication Policy

Configured via `multiAuth.policy` in `appsettings.json`:

- **EitherOr**: Accept any enabled authentication method
- **JwtOnly**: Only JWT authentication is accepted
- **PriorityOrder**: Try authentication methods in priority order
- **AllRequired**: All enabled authentication methods must succeed

---

## Authentication Endpoints

### Sign In

**POST** `/api/auth/sign-in`

Signs in a user with username and password credentials.

**Authentication**: Not required (public endpoint)

**Request Body**:
```json
{
  "username": "testuser",
  "password": "password123",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0"
}
```

**Response**: `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-123",
  "sessionId": "123e4567-e89b-12d3-a456-426614174000",
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "expiresAt": "2024-01-15T11:00:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Invalid username or password
- `423 Locked`: Account is locked due to failed login attempts

---

### Sign Out

**POST** `/api/auth/sign-out`

Signs out a user and revokes the current session.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "sessionId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Response**: `200 OK`
```json
{
  "message": "Sign-out successful"
}
```

---

### Refresh Token

**POST** `/api/auth/refresh`

Refreshes an access token using a valid refresh token.

**Authentication**: Not required (uses refresh token)

**Request Body**:
```json
{
  "refreshToken": "refresh-token-123"
}
```

**Response**: `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "new-refresh-token-456",
  "sessionId": "123e4567-e89b-12d3-a456-426614174000",
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "expiresAt": "2024-01-15T11:30:00Z"
}
```

---

### Sign In with Biometric

**POST** `/api/auth/sign-in/biometric`

Signs in a user using biometric authentication. Requires prior authentication for biometric enrollment.

**Authentication**: Required (JWT - user must be authenticated first)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "biometricData": {
    "type": "Fingerprint",
    "data": "base64encodedfingerprintdata",
    "hash": "sha256hash"
  },
  "threshold": 0.85
}
```

**Response**: `200 OK`
```json
{
  "message": "Biometric sign-in successful"
}
```

---

## Multi-Factor Authentication (MFA)

### Setup MFA

**POST** `/api/auth/mfa/setup`

Initiates MFA setup for an identity. Returns QR code data for TOTP setup.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "method": "TOTP"
}
```

**Response**: `200 OK`
```json
{
  "secretKey": "JBSWY3DPEHPK3PXP",
  "qrCodeDataUrl": "data:image/png;base64,iVBORw0KGgo...",
  "backupCodes": ["code1", "code2", "code3"]
}
```

---

### Enable MFA

**POST** `/api/auth/mfa/enable`

Enables MFA for an identity after verification code is confirmed.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "method": "TOTP",
  "verificationCode": "123456"
}
```

**Response**: `200 OK`
```json
{
  "message": "MFA enabled successfully"
}
```

---

### Disable MFA

**POST** `/api/auth/mfa/disable`

Disables MFA for an identity.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "method": "TOTP"
}
```

**Response**: `200 OK`
```json
{
  "message": "MFA disabled successfully"
}
```

---

### Create MFA Challenge

**POST** `/api/auth/mfa/challenge`

Creates an MFA challenge for verification.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "method": "TOTP"
}
```

**Response**: `200 OK`
```json
{
  "message": "MFA challenge created",
  "challengeId": "challenge-123"
}
```

---

### Verify MFA Challenge

**POST** `/api/auth/mfa/verify`

Verifies an MFA challenge with a code.

**Authentication**: Required (JWT)

**Request Body**:
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "method": "TOTP",
  "code": "123456"
}
```

**Response**: `200 OK`
```json
{
  "message": "MFA challenge verified",
  "verified": true
}
```

---

### Get MFA Status

**GET** `/api/auth/mfa/status/{identityId}`

Retrieves the MFA status for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
{
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "multiFactorEnabled": true,
  "preferredMfaMethod": "TOTP",
  "enabledMethods": ["TOTP", "SMS"],
  "enabledAt": "2024-01-15T10:00:00Z"
}
```

---

## Permission Management

### Create Permission

**POST** `/api/auth/permissions`

Creates a new permission.

**Authentication**: Required (JWT)  
**Authorization**: `permissions:write`

**Request Body**:
```json
{
  "name": "test:permission",
  "description": "Test permission description"
}
```

**Response**: `201 Created`
```json
{
  "message": "Permission created successfully",
  "permissionId": "123e4567-e89b-12d3-a456-426614174000"
}
```

---

### Update Permission

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

---

### Delete Permission

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

---

### Assign Permission to Identity

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

---

### Remove Permission from Identity

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

---

### Get Identity Permissions

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

---

## Role Management

### Create Role

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

---

### Update Role

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

---

### Delete Role

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

---

### Assign Role to Identity

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

---

### Remove Role from Identity

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

---

### Get Identity Roles

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

---

### Add Permission to Role

**POST** `/api/auth/roles/{roleId}/permissions/{permissionId}`

Adds a permission to a role.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `roleId` (Guid): The role identifier
- `permissionId` (Guid): The permission identifier

**Response**: `200 OK`
```json
{
  "message": "Permission added to role successfully"
}
```

---

### Remove Permission from Role

**POST** `/api/auth/roles/{roleId}/permissions/{permissionId}/remove`

Removes a permission from a role.

**Authentication**: Required (JWT)  
**Authorization**: `roles:write`

**Path Parameters**:
- `roleId` (Guid): The role identifier
- `permissionId` (Guid): The permission identifier

**Response**: `200 OK`
```json
{
  "message": "Permission removed from role successfully"
}
```

---

## Email Confirmation

### Create Email Confirmation

**POST** `/api/auth/confirmations/email`

Creates an email confirmation request and sends a confirmation email.

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
  "message": "Email confirmation sent"
}
```

---

### Confirm Email

**POST** `/api/auth/confirmations/email/confirm`

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

---

### Resend Email Confirmation

**POST** `/api/auth/confirmations/email/resend`

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

---

### Get Email Confirmation Status

**GET** `/api/auth/confirmations/email/status/{identityId}`

Retrieves the email confirmation status for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
true
```

---

## SMS Confirmation

### Create SMS Confirmation

**POST** `/api/auth/confirmations/sms`

Creates an SMS confirmation request and sends a confirmation code via SMS.

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
  "message": "SMS confirmation sent"
}
```

---

### Confirm SMS

**POST** `/api/auth/confirmations/sms/confirm`

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

---

### Resend SMS Confirmation

**POST** `/api/auth/confirmations/sms/resend`

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

---

### Get SMS Confirmation Status

**GET** `/api/auth/confirmations/sms/status/{identityId}`

Retrieves the SMS confirmation status for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
true
```

---

## Session Management

### Get Active Sessions

**GET** `/api/auth/sessions/{identityId}`

Retrieves all active sessions for an identity.

**Authentication**: Required (JWT)

**Path Parameters**:
- `identityId` (Guid): The identity identifier

**Response**: `200 OK`
```json
[
  {
    "sessionId": "123e4567-e89b-12d3-a456-426614174000",
    "identityId": "123e4567-e89b-12d3-a456-426614174000",
    "status": "Active",
    "createdAt": "2024-01-15T10:00:00Z",
    "expiresAt": "2024-01-16T10:00:00Z",
    "lastAccessedAt": "2024-01-15T11:00:00Z",
    "ipAddress": "192.168.1.1",
    "userAgent": "Mozilla/5.0"
  }
]
```

---

### Get Session

**GET** `/api/auth/sessions/{sessionId}`

Retrieves a specific session by ID.

**Authentication**: Required (JWT)

**Path Parameters**:
- `sessionId` (Guid): The session identifier

**Response**: `200 OK`
```json
{
  "sessionId": "123e4567-e89b-12d3-a456-426614174000",
  "identityId": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Active",
  "createdAt": "2024-01-15T10:00:00Z",
  "expiresAt": "2024-01-16T10:00:00Z",
  "lastAccessedAt": "2024-01-15T11:00:00Z",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0"
}
```

---

## MFA Methods

The following MFA methods are supported:

- **TOTP**: Time-based One-Time Password (authenticator apps)
- **SMS**: SMS-based verification codes
- **Email**: Email-based verification codes
- **Biometric**: Biometric verification (fingerprint, facial, etc.)
- **BackupCode**: Backup codes for account recovery

---

## Configuration

### Multi-Authentication Configuration

Configure multi-authentication in `appsettings.json`:

```json
{
  "multiAuth": {
    "enabled": true,
    "enableJwt": true,
    "enableDid": true,
    "enableAzure": false,
    "enableIdentity": false,
    "enableDistributed": false,
    "enableCertificate": false,
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
    "backupCodeCount": 10,
    "challengeExpiration": "00:05:00",
    "maxFailedAttempts": 5,
    "lockoutDuration": "00:15:00"
  }
}
```

### Email Configuration

```json
{
  "email": {
    "enabled": true,
    "confirmationExpiration": "7.00:00:00",
    "codeLength": 6,
    "maxRequestsPerHour": 5,
    "fromEmail": "noreply@futurewampumid.com",
    "fromName": "FutureWampumID"
  }
}
```

### SMS Configuration

```json
{
  "sms": {
    "enabled": true,
    "confirmationExpiration": "00:15:00",
    "codeLength": 6,
    "maxRequestsPerHour": 5,
    "provider": "Twilio",
    "fromPhoneNumber": null
  }
}
```

### Cleanup Configuration

```json
{
  "cleanup": {
    "sessionCleanupEnabled": true,
    "sessionCleanupInterval": "01:00:00",
    "emailConfirmationCleanupEnabled": true,
    "emailConfirmationCleanupInterval": "06:00:00",
    "smsConfirmationCleanupEnabled": true,
    "smsConfirmationCleanupInterval": "06:00:00"
  }
}
```

---

## Error Handling

### Common Error Codes

| Error Code | Description |
|------------|-------------|
| `invalid_credentials` | Invalid username or password |
| `account_locked` | Account is locked due to failed login attempts |
| `session_not_found` | Session not found or expired |
| `mfa_not_enabled` | MFA is not enabled for this identity |
| `mfa_verification_failed` | MFA verification code is incorrect |
| `permission_not_found` | Permission not found |
| `role_not_found` | Role not found |
| `confirmation_expired` | Confirmation token/code has expired |
| `confirmation_not_found` | Confirmation not found |

---

## Support

For issues, questions, or contributions, please contact the Mamey Technologies team or refer to the project documentation.

**Copyright**: Mamey Technologies (mamey.io)  
**License**: AGPL-3.0

