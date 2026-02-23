# Mamey.Identity.EntityFramework

**Library**: `Mamey.Identity.EntityFramework`  
**Location**: `Mamey/src/Mamey.Identity.EntityFramework/`  
**Type**: Identity Library - Entity Framework Core  
**Version**: 2.0.*  
**Files**: 14 C# files  
**Namespace**: `Mamey.Identity.EntityFramework`

## Overview

Mamey.Identity.EntityFramework provides Entity Framework Core integration for ASP.NET Core Identity in the Mamey framework. It includes custom user and role stores, enhanced user management, and PostgreSQL database support.

### Key Features

- **Entity Framework Core Integration**: Full EF Core Identity storage
- **PostgreSQL Support**: Optimized for PostgreSQL database
- **Custom User Store**: Enhanced user store with eager loading
- **Custom Role Store**: Enhanced role store implementation
- **Claims Management**: Custom claims principal factory
- **Multi-Tenant Support**: Tenant isolation for user data
- **Extended User Properties**: Additional user properties (FirstName, LastName, etc.)

## Installation

```bash
dotnet add package Mamey.Identity.EntityFramework
```

## Quick Start

```csharp
using Mamey.Identity.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityEntityFramework<MyDbContext>(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
app.Run();
```

## Core Components

- **IdentityDbContext**: Custom EF Core identity context
- **ApplicationUser**: Extended user entity
- **ApplicationRole**: Extended role entity
- **MameyUserStore**: Custom user store with eager loading
- **MameyRoleStore**: Custom role store
- **MameyClaimsPrincipalFactory**: Custom claims factory

## Database Schema

### Users Table

```sql
CREATE TABLE "Users" (
    "Id" TEXT PRIMARY KEY,
    "UserName" TEXT NOT NULL,
    "NormalizedUserName" TEXT NOT NULL,
    "Email" TEXT,
    "NormalizedEmail" TEXT,
    "EmailConfirmed" BOOLEAN NOT NULL,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL,
    "TwoFactorEnabled" BOOLEAN NOT NULL,
    "LockoutEnd" TIMESTAMP,
    "LockoutEnabled" BOOLEAN NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "FullName" TEXT,
    "TenantId" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt" TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "ProfilePictureUrl" TEXT,
    "TimeZone" TEXT,
    "Locale" TEXT
);
```

## Usage Examples

### Create User

```csharp
var user = new ApplicationUser
{
    UserName = "john.doe",
    Email = "john.doe@example.com",
    FirstName = "John",
    LastName = "Doe",
    TenantId = "tenant123"
};

var result = await UserManager.CreateAsync(user, "SecurePassword123!");
```

### Find User with Roles

```csharp
var user = await UserManager.FindByEmailAsync("user@example.com");
// User includes roles and claims automatically
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Identity.AspNetCore**: ASP.NET Core Identity integration

## Tags

#identity #entityframework #postgresql #aspnet-core #database #mamey

