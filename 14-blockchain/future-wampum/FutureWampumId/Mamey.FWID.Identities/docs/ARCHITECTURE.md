# Mamey.FWID.Identities Architecture Documentation

## Overview

Mamey.FWID.Identities is the central authentication and identity provider for the FutureWampumID ecosystem. It provides comprehensive authentication, authorization, multi-factor authentication, and identity management capabilities for all applications (BIIS, SICB, Future BDET Bank, Government, Holistic Medicine, RedWebNetwork).

**Service**: Mamey.FWID.Identities  
**Port**: 5001  
**Type**: Identity & Authentication Service  
**Version**: 1.0.0

---

## System Architecture

### High-Level Architecture

```mermaid
graph TB
    subgraph "Client Applications"
        BIIS[BIIS]
        SICB[SICB]
        BDET[Future BDET Bank]
        GOV[Government]
        HM[Holistic Medicine]
        RWN[RedWebNetwork]
    end

    subgraph "Mamey.FWID.Identities Service"
        API[API Layer]
        AUTH[Multi-Auth Layer]
        APP[Application Services]
        DOM[Domain Layer]
        INFRA[Infrastructure Layer]
    end

    subgraph "Data Stores"
        PG[(PostgreSQL<br/>Write Model)]
        MONGO[(MongoDB<br/>Read Model)]
        REDIS[(Redis<br/>Cache)]
    end

    subgraph "Message Bus"
        RABBIT[RabbitMQ]
    end

    subgraph "External Services"
        EMAIL[Email Service]
        SMS[SMS Service]
        NOTIF[Notifications Service]
    end

    BIIS --> API
    SICB --> API
    BDET --> API
    GOV --> API
    HM --> API
    RWN --> API

    API --> AUTH
    AUTH --> APP
    APP --> DOM
    DOM --> INFRA

    INFRA --> PG
    INFRA --> MONGO
    INFRA --> REDIS
    INFRA --> RABBIT

    RABBIT --> EMAIL
    RABBIT --> SMS
    RABBIT --> NOTIF
```

---

## Authentication Flow

### Sign-In Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant AuthService
    participant IdentityRepo
    participant SessionRepo
    participant JWTManager
    participant EventBus

    Client->>API: POST /api/auth/sign-in
    API->>AuthService: SignInAsync(username, password)
    AuthService->>IdentityRepo: FindAsync(username)
    IdentityRepo-->>AuthService: Identity
    AuthService->>AuthService: VerifyPassword()
    AuthService->>Identity: SignIn()
    AuthService->>IdentityRepo: UpdateAsync(identity)
    AuthService->>SessionRepo: CreateSession()
    AuthService->>JWTManager: CreateTokenAsync()
    JWTManager-->>AuthService: AccessToken
    AuthService-->>API: AuthenticationResult
    API->>EventBus: Publish(SignInIntegrationEvent)
    API-->>Client: 200 OK (tokens)
```

### Token Refresh Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant AuthService
    participant SessionRepo
    participant JWTManager

    Client->>API: POST /api/auth/refresh
    API->>AuthService: RefreshTokenAsync(refreshToken)
    AuthService->>SessionRepo: GetByRefreshTokenAsync()
    SessionRepo-->>AuthService: Session
    AuthService->>AuthService: ValidateSession()
    AuthService->>JWTManager: CreateTokenAsync()
    JWTManager-->>AuthService: NewAccessToken
    AuthService->>Session: UpdateTokens()
    AuthService->>SessionRepo: UpdateAsync(session)
    AuthService-->>API: AuthenticationResult
    API-->>Client: 200 OK (new tokens)
```

### MFA Setup Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant MFAService
    participant IdentityRepo
    participant MfaConfigRepo
    participant SecurityProvider

    Client->>API: POST /api/auth/mfa/setup
    API->>MFAService: SetupMfaAsync(identityId, method)
    MFAService->>IdentityRepo: GetAsync(identityId)
    IdentityRepo-->>MFAService: Identity
    MFAService->>MfaConfigRepo: GetByIdentityAndMethodAsync()
    MfaConfigRepo-->>MFAService: null (not exists)
    MFAService->>SecurityProvider: GenerateSecretKey()
    SecurityProvider-->>MFAService: SecretKey
    MFAService->>MfaConfigRepo: AddAsync(config)
    MFAService->>SecurityProvider: GenerateBackupCodes()
    SecurityProvider-->>MFAService: BackupCodes
    MFAService->>SecurityProvider: GenerateQrCode()
    SecurityProvider-->>MFAService: QrCodeDataUrl
    MFAService-->>API: MfaSetupResult
    API-->>Client: 200 OK (secret, QR code, backup codes)
```

### MFA Verification Flow

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant MFAService
    participant MfaConfigRepo
    participant SecurityProvider

    Client->>API: POST /api/auth/mfa/verify
    API->>MFAService: VerifyMfaChallengeAsync(identityId, method, code)
    MFAService->>MfaConfigRepo: GetByIdentityAndMethodAsync()
    MfaConfigRepo-->>MFAService: MfaConfiguration
    MFAService->>SecurityProvider: VerifyTotpCode()
    SecurityProvider-->>MFAService: Valid
    MFAService->>MfaConfiguration: MarkAsUsed()
    MFAService->>MfaConfigRepo: UpdateAsync(config)
    MFAService->>Identity: (via domain event)
    MFAService-->>API: MfaVerificationResult
    API->>EventBus: Publish(MfaVerifiedIntegrationEvent)
    API-->>Client: 200 OK (verified)
```

---

## Data Flow

### Write Flow (CQRS)

```mermaid
graph LR
    A[Client Request] --> B[API Endpoint]
    B --> C[Command Handler]
    C --> D[Application Service]
    D --> E[Domain Entity]
    E --> F[Domain Events]
    F --> G[Event Processor]
    G --> H[EventMapper]
    H --> I[Integration Events]
    I --> J[RabbitMQ]
    D --> K[PostgreSQL Repository]
    K --> L[(PostgreSQL)]
    G --> M[Background Sync]
    M --> N[(MongoDB)]
    M --> O[(Redis)]
```

### Read Flow (CQRS)

```mermaid
graph LR
    A[Client Request] --> B[API Endpoint]
    B --> C[Query Handler]
    C --> D[Application Service]
    D --> E{Check Cache}
    E -->|Hit| F[Redis]
    E -->|Miss| G[MongoDB Repository]
    G --> H{Found?}
    H -->|Yes| I[(MongoDB)]
    H -->|No| J[PostgreSQL Repository]
    J --> K[(PostgreSQL)]
    I --> L[Return Result]
    K --> L
    F --> L
    L --> M[Cache Result]
    M --> N[(Redis)]
```

---

## Domain Model

### Identity Aggregate Relationships

```mermaid
erDiagram
    Identity ||--o{ Session : "has"
    Identity ||--o{ MfaConfiguration : "has"
    Identity ||--o{ IdentityRole : "has"
    Identity ||--o{ IdentityPermission : "has"
    Identity ||--o{ EmailConfirmation : "has"
    Identity ||--o{ SmsConfirmation : "has"
    
    Role ||--o{ IdentityRole : "assigned to"
    Role ||--o{ Permission : "contains"
    
    Permission ||--o{ IdentityPermission : "assigned to"
    Permission ||--o{ Role : "included in"
    
    Identity {
        Guid Id
        string Username
        string PasswordHash
        bool EmailConfirmed
        bool PhoneConfirmed
        bool MultiFactorEnabled
        IdentityStatus Status
    }
    
    Session {
        Guid Id
        Guid IdentityId
        string AccessToken
        string RefreshToken
        DateTime ExpiresAt
        SessionStatus Status
    }
    
    MfaConfiguration {
        Guid Id
        Guid IdentityId
        MfaMethod Method
        bool IsEnabled
        string SecretKey
    }
    
    Role {
        Guid Id
        string Name
        RoleStatus Status
    }
    
    Permission {
        Guid Id
        string Name
        PermissionStatus Status
    }
```

---

## Multi-Authentication Architecture

### Authentication Policy Flow

```mermaid
graph TB
    A[HTTP Request] --> B[MultiAuthMiddleware]
    B --> C{Policy Type?}
    
    C -->|EitherOr| D[Try JWT]
    C -->|EitherOr| E[Try DID]
    C -->|EitherOr| F[Try Azure]
    D --> G{Success?}
    E --> G
    F --> G
    G -->|Yes| H[Authenticated]
    G -->|No| I[Try Next Method]
    I --> G
    
    C -->|JwtOnly| J[JWT Only]
    J --> K{Valid?}
    K -->|Yes| H
    K -->|No| L[401 Unauthorized]
    
    C -->|PriorityOrder| M[Try in Priority Order]
    M --> N[JWT First]
    N --> O{Success?}
    O -->|No| P[DID Second]
    P --> Q{Success?}
    Q -->|No| R[Azure Third]
    R --> S{Success?}
    S -->|Yes| H
    S -->|No| L
    
    C -->|AllRequired| T[All Methods Required]
    T --> U[JWT + DID + Azure]
    U --> V{All Valid?}
    V -->|Yes| H
    V -->|No| L
```

---

## Background Services

### Cleanup Service Flow

```mermaid
graph TB
    A[Cleanup Service] --> B{Service Type}
    
    B -->|Session| C[SessionCleanupService]
    C --> D[Query Expired Sessions]
    D --> E[Revoke Sessions]
    E --> F[Publish SessionRevoked Events]
    
    B -->|Email| G[EmailConfirmationCleanupService]
    G --> H[Query Expired Confirmations]
    H --> I[Mark as Expired]
    
    B -->|SMS| J[SmsConfirmationCleanupService]
    J --> K[Query Expired Confirmations]
    K --> L[Mark as Expired]
    
    F --> M[RabbitMQ]
    I --> N[(PostgreSQL)]
    L --> N
    E --> N
```

### Sync Service Flow

```mermaid
graph LR
    A[PostgreSQL Write] --> B[Domain Event]
    B --> C[Event Processor]
    C --> D[EventMapper]
    D --> E[Integration Event]
    E --> F[RabbitMQ]
    
    C --> G[Background Sync Service]
    G --> H{Entity Type}
    H -->|Identity| I[IdentityMongoSyncService]
    H -->|Identity| J[IdentityRedisSyncService]
    I --> K[(MongoDB)]
    J --> L[(Redis)]
```

---

## Integration Events Flow

### Event Publishing Flow

```mermaid
sequenceDiagram
    participant Domain
    participant EventProcessor
    participant EventMapper
    participant BusPublisher
    participant RabbitMQ
    participant Notifications

    Domain->>Domain: Raise Domain Event
    Domain->>EventProcessor: ProcessAsync(events)
    EventProcessor->>EventMapper: Map(domainEvent)
    EventMapper->>EventMapper: Convert to Integration Event
    EventMapper-->>EventProcessor: IntegrationEvent
    EventProcessor->>BusPublisher: PublishAsync(event)
    BusPublisher->>RabbitMQ: Publish Message
    RabbitMQ->>Notifications: Deliver Event
    Notifications->>Notifications: Handle Event
```

### Event Types

```mermaid
graph TB
    A[Domain Events] --> B[EventMapper]
    B --> C[Integration Events]
    
    C --> D[Authentication Events]
    C --> E[MFA Events]
    C --> F[Confirmation Events]
    C --> G[Permission Events]
    C --> H[Role Events]
    
    D --> I[SignInIntegrationEvent]
    D --> J[SignOutIntegrationEvent]
    D --> K[TokenRefreshedIntegrationEvent]
    
    E --> L[MfaEnabledIntegrationEvent]
    E --> M[MfaDisabledIntegrationEvent]
    E --> N[MfaVerifiedIntegrationEvent]
    
    F --> O[EmailConfirmedIntegrationEvent]
    F --> P[SmsConfirmedIntegrationEvent]
    
    G --> Q[PermissionAssignedIntegrationEvent]
    G --> R[PermissionRemovedIntegrationEvent]
    
    H --> S[RoleAssignedIntegrationEvent]
    H --> T[RoleRemovedIntegrationEvent]
```

---

## Permission and Role Management

### Permission Assignment Flow

```mermaid
sequenceDiagram
    participant Admin
    participant API
    participant PermissionService
    participant PermissionRepo
    participant IdentityPermissionRepo
    participant EventBus

    Admin->>API: POST /api/auth/identities/{id}/permissions/{pid}
    API->>PermissionService: AssignPermissionToIdentityAsync()
    PermissionService->>PermissionRepo: GetAsync(permissionId)
    PermissionRepo-->>PermissionService: Permission
    PermissionService->>IdentityPermissionRepo: AddAsync(assignment)
    PermissionService->>Identity: (via domain event)
    PermissionService->>EventBus: Publish(PermissionAssignedIntegrationEvent)
    PermissionService-->>API: Success
    API-->>Admin: 200 OK
```

### Role Assignment Flow

```mermaid
sequenceDiagram
    participant Admin
    participant API
    participant RoleService
    participant RoleRepo
    participant IdentityRoleRepo
    participant EventBus

    Admin->>API: POST /api/auth/identities/{id}/roles/{rid}
    API->>RoleService: AssignRoleToIdentityAsync()
    RoleService->>RoleRepo: GetAsync(roleId)
    RoleRepo-->>RoleService: Role
    RoleService->>IdentityRoleRepo: AddAsync(assignment)
    RoleService->>Identity: (via domain event)
    RoleService->>EventBus: Publish(RoleAssignedIntegrationEvent)
    RoleService-->>API: Success
    API-->>Admin: 200 OK
```

---

## Email/SMS Confirmation Flow

### Email Confirmation Flow

```mermaid
sequenceDiagram
    participant User
    participant API
    participant EmailService
    participant EmailConfirmationRepo
    participant EmailProvider
    participant EventBus

    User->>API: POST /api/auth/confirmations/email
    API->>EmailService: CreateEmailConfirmationAsync()
    EmailService->>EmailConfirmationRepo: AddAsync(confirmation)
    EmailService->>EmailProvider: SendEmail()
    EmailProvider-->>EmailService: Sent
    EmailService-->>API: Success
    API-->>User: 200 OK
    
    User->>API: POST /api/auth/confirmations/email/confirm
    API->>EmailService: ConfirmEmailAsync(token)
    EmailService->>EmailConfirmationRepo: GetByTokenAsync()
    EmailConfirmationRepo-->>EmailService: Confirmation
    EmailService->>EmailService: ValidateToken()
    EmailService->>Identity: ConfirmEmail()
    EmailService->>EventBus: Publish(EmailConfirmedIntegrationEvent)
    EmailService-->>API: Success
    API-->>User: 200 OK
```

### SMS Confirmation Flow

```mermaid
sequenceDiagram
    participant User
    participant API
    participant SmsService
    participant SmsConfirmationRepo
    participant SmsProvider
    participant EventBus

    User->>API: POST /api/auth/confirmations/sms
    API->>SmsService: CreateSmsConfirmationAsync()
    SmsService->>SmsConfirmationRepo: AddAsync(confirmation)
    SmsService->>SmsProvider: SendSms(code)
    SmsProvider-->>SmsService: Sent
    SmsService-->>API: Success
    API-->>User: 200 OK
    
    User->>API: POST /api/auth/confirmations/sms/confirm
    API->>SmsService: ConfirmSmsAsync(code)
    SmsService->>SmsConfirmationRepo: GetByCodeAsync()
    SmsConfirmationRepo-->>SmsService: Confirmation
    SmsService->>SmsService: ValidateCode()
    SmsService->>Identity: ConfirmPhone()
    SmsService->>EventBus: Publish(SmsConfirmedIntegrationEvent)
    SmsService-->>API: Success
    API-->>User: 200 OK
```

---

## Repository Pattern

### Composite Repository Pattern

```mermaid
graph TB
    A[Application Service] --> B[IRepository Interface]
    B --> C[Composite Repository]
    
    C --> D[PostgreSQL Repository]
    C --> E[MongoDB Repository]
    C --> F[Redis Repository]
    
    D --> G[(PostgreSQL<br/>Write Model)]
    E --> H[(MongoDB<br/>Read Model)]
    F --> I[(Redis<br/>Cache)]
    
    J[Read Operation] --> K{Check Redis}
    K -->|Hit| I
    K -->|Miss| L{Check MongoDB}
    L -->|Hit| H
    L -->|Miss| G
    
    M[Write Operation] --> G
    G --> N[Invalidate Cache]
    N --> I
    G --> O[Sync to MongoDB]
    O --> H
```

---

## Security Architecture

### Authentication Security Flow

```mermaid
graph TB
    A[Client Request] --> B[MultiAuthMiddleware]
    B --> C{Authentication Method}
    
    C -->|JWT| D[Validate JWT Token]
    C -->|DID| E[Validate DID Token]
    C -->|Azure| F[Validate Azure Token]
    C -->|Certificate| G[Validate Certificate]
    
    D --> H{Valid?}
    E --> H
    F --> H
    G --> H
    
    H -->|Yes| I[Extract Claims]
    H -->|No| J[401 Unauthorized]
    
    I --> K[Permission Check]
    K --> L{Has Permission?}
    L -->|Yes| M[Authorized]
    L -->|No| N[403 Forbidden]
```

### Account Lockout Flow

```mermaid
stateDiagram-v2
    [*] --> Active: Sign In Success
    Active --> FailedAttempt: Invalid Password
    FailedAttempt --> Active: Valid Password
    FailedAttempt --> Locked: Max Attempts Reached
    Locked --> Active: Lockout Duration Expired
    Locked --> Active: Admin Unlock
    Active --> [*]: Sign Out
```

---

## Configuration Architecture

### Configuration Hierarchy

```mermaid
graph TB
    A[appsettings.json] --> B[MultiAuth Options]
    A --> C[JWT Options]
    A --> D[MFA Options]
    A --> E[Email Options]
    A --> F[SMS Options]
    A --> G[Cleanup Options]
    
    B --> H[Authentication Policy]
    B --> I[Enable Flags]
    B --> J[Scheme Names]
    
    C --> K[Token Settings]
    C --> L[Issuer/Audience]
    
    D --> M[MFA Methods]
    D --> N[Challenge Settings]
    
    E --> O[Email Provider]
    E --> P[Template Settings]
    
    F --> Q[SMS Provider]
    F --> R[Code Settings]
    
    G --> S[Cleanup Intervals]
    G --> T[Service Flags]
```

---

## Deployment Architecture

### Container Architecture

```mermaid
graph TB
    subgraph "Kubernetes Cluster"
        subgraph "Pod: identities-service"
            A[Mamey.FWID.Identities]
            B[Health Check]
        end
        
        subgraph "StatefulSet: postgres"
            C[(PostgreSQL)]
        end
        
        subgraph "StatefulSet: mongo"
            D[(MongoDB)]
        end
        
        subgraph "StatefulSet: redis"
            E[(Redis)]
        end
        
        subgraph "Deployment: rabbitmq"
            F[RabbitMQ]
        end
    end
    
    A --> C
    A --> D
    A --> E
    A --> F
    B --> A
```

---

## Testing Architecture

### Test Pyramid

```mermaid
graph TB
    A[End-to-End Tests] --> B[Integration Tests]
    B --> C[Unit Tests]
    
    A --> D[Authentication Flows]
    A --> E[MFA Flows]
    A --> F[Permission Flows]
    
    B --> G[Service Tests]
    B --> H[Repository Tests]
    B --> I[Background Worker Tests]
    
    C --> J[Domain Entity Tests]
    C --> K[Service Tests]
    C --> L[Handler Tests]
    C --> M[Infrastructure Tests]
```

---

## Support

For issues, questions, or contributions, please contact the Mamey Technologies team or refer to the project documentation.

**Copyright**: Mamey Technologies (mamey.io)  
**License**: AGPL-3.0
