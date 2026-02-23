# Mamey.MessageBrokers.Outbox.EntityFramework

**Library**: `Mamey.MessageBrokers.Outbox.EntityFramework`  
**Location**: `Mamey/src/Mamey.MessageBrokers.Outbox.EntityFramework/`  
**Type**: Integration Library - Outbox Pattern with Entity Framework  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.MessageBrokers.Outbox.EntityFramework`

## Overview

Mamey.MessageBrokers.Outbox.EntityFramework provides Entity Framework Core implementation of the Outbox pattern for reliable message publishing in the Mamey framework. It ensures messages are stored in the database before being published to message brokers.

### Key Features

- **Outbox Pattern**: Reliable message publishing using database transactions
- **Entity Framework Integration**: EF Core-based outbox storage
- **Transactional Guarantees**: Messages stored in same transaction as domain events
- **Background Processing**: Background service for publishing messages
- **Idempotency**: Prevents duplicate message publishing
- **PostgreSQL Support**: Optimized for PostgreSQL database

## Installation

```bash
dotnet add package Mamey.MessageBrokers.Outbox.EntityFramework
```

## Quick Start

```csharp
using Mamey.MessageBrokers.Outbox.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMessageBrokers()
    .AddRabbitMQ()
    .AddMessageOutbox()
    .AddEntityFramework<OutboxDbContext>(connectionString);

var app = builder.Build();
app.Run();
```

## Core Components

- **OutboxDbContext**: EF Core context for outbox messages
- **OutboxMessage**: Outbox message entity
- **OutboxMessageRepository**: Repository for outbox messages
- **OutboxPublisher**: Background service for publishing messages

## Usage Examples

### Store Message in Outbox

```csharp
@inject IMessageOutbox Outbox

await Outbox.SendAsync(message);
// Message stored in database transaction
```

## Related Libraries

- **Mamey.MessageBrokers**: Message broker abstractions
- **Mamey.MessageBrokers.Outbox**: Outbox pattern abstractions

## Tags

#outbox #entityframework #message-brokers #reliable-messaging #mamey

