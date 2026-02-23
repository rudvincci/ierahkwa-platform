# Mamey.Identity.Redis

**Library**: `Mamey.Identity.Redis`  
**Location**: `Mamey/src/Mamey.Identity.Redis/`  
**Type**: Identity Library - Redis Token Cache  
**Version**: 2.0.*  
**Files**: 3 C# files  
**Namespace**: `Mamey.Identity.Redis`

## Overview

Mamey.Identity.Redis provides Redis-based token caching and blacklisting for the Mamey Identity framework. It enables distributed token storage, token revocation, and scalable token management across multiple application instances.

### Key Features

- **Token Caching**: Redis-based token storage
- **Token Blacklisting**: Revoke tokens by storing in Redis
- **Distributed Storage**: Shared token cache across instances
- **Automatic Expiration**: TTL-based token expiration
- **High Performance**: Fast token lookups
- **Scalability**: Support for Redis cluster

## Installation

```bash
dotnet add package Mamey.Identity.Redis
```

## Quick Start

```csharp
using Mamey.Identity.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMameyIdentityRedis(
    connectionString: "localhost:6379",
    keyPrefix: "mamey:identity");

var app = builder.Build();
app.Run();
```

## Configuration

```json
{
  "Mamey:Identity:Redis": {
    "ConnectionString": "localhost:6379",
    "KeyPrefix": "mamey:identity",
    "DefaultExpiration": "01:00:00",
    "DefaultBlacklistExpiration": "7.00:00:00",
    "Database": 0
  }
}
```

## Core Components

- **IRedisTokenCache**: Redis token cache interface
- **RedisTokenCache**: Redis token cache implementation
- **RedisTokenCacheOptions**: Configuration options

## Usage Examples

### Cache Token

```csharp
@inject IRedisTokenCache TokenCache

await TokenCache.SetTokenAsync(
    key: "user123",
    token: "jwt-token-here",
    expiration: TimeSpan.FromHours(1));
```

### Get Cached Token

```csharp
var token = await TokenCache.GetTokenAsync("user123");
```

### Blacklist Token

```csharp
await TokenCache.BlacklistTokenAsync(
    tokenId: "token123",
    expiration: TimeSpan.FromDays(7));
```

### Check Token Blacklist

```csharp
var isBlacklisted = await TokenCache.IsTokenBlacklistedAsync("token123");
```

## Related Libraries

- **Mamey.Identity.Core**: Core identity abstractions
- **Mamey.Identity.Jwt**: JWT token generation

## Tags

#identity #redis #caching #token #distributed #mamey















