# Mamey.MessageBrokers.Outbox.Mongo

**Library**: `Mamey.MessageBrokers.Outbox.Mongo`  
**Location**: `Mamey/src/Mamey.MessageBrokers.Outbox.Mongo/`  
**Type**: Integration Library - Outbox Pattern with MongoDB  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.MessageBrokers.Outbox.Mongo`

## Overview

Mamey.MessageBrokers.Outbox.Mongo provides MongoDB implementation of the Outbox pattern for reliable message publishing in the Mamey framework. It ensures messages are stored in MongoDB before being published to message brokers.

### Key Features

- **Outbox Pattern**: Reliable message publishing using MongoDB transactions
- **MongoDB Integration**: MongoDB-based outbox storage
- **Transactional Guarantees**: Messages stored in same transaction as domain events
- **Background Processing**: Background service for publishing messages
- **Idempotency**: Prevents duplicate message publishing
- **High Performance**: Optimized for MongoDB operations

## Installation

```bash
dotnet add package Mamey.MessageBrokers.Outbox.Mongo
```

## Quick Start

```csharp
using Mamey.MessageBrokers.Outbox.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMessageBrokers()
    .AddRabbitMQ()
    .AddMessageOutbox()
    .AddMongo(connectionString);

var app = builder.Build();
app.Run();
```

## Core Components

- **OutboxMessage**: MongoDB document for outbox messages
- **OutboxMessageRepository**: Repository for outbox messages
- **OutboxPublisher**: Background service for publishing messages

## Usage Examples

### Store Message in Outbox

```csharp
@inject IMessageOutbox Outbox

await Outbox.SendAsync(message);
// Message stored in MongoDB transaction
```

## Related Libraries

- **Mamey.MessageBrokers**: Message broker abstractions
- **Mamey.MessageBrokers.Outbox**: Outbox pattern abstractions
- **Mamey.Persistence.MongoDB**: MongoDB persistence

## Tags

#outbox #mongodb #message-brokers #reliable-messaging #mamey

