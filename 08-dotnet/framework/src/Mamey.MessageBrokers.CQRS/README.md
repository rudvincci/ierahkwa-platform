# Mamey.MessageBrokers.CQRS

**Library**: `Mamey.MessageBrokers.CQRS`  
**Location**: `Mamey/src/Mamey.MessageBrokers.CQRS/`  
**Type**: Integration Library - Message Brokers & CQRS  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.MessageBrokers.CQRS`

## Overview

Mamey.MessageBrokers.CQRS provides integration between Mamey's CQRS pattern and message broker infrastructure. It enables commands and events to be sent and received through message brokers like RabbitMQ, enabling distributed CQRS scenarios.

### Key Features

- **Command Publishing**: Send commands through message brokers
- **Event Publishing**: Publish events through message brokers
- **Command Subscriptions**: Subscribe to commands from message brokers
- **Event Subscriptions**: Subscribe to events from message brokers
- **Service Bus Dispatcher**: Message broker-based command/event dispatcher
- **Correlation Context**: Maintain correlation context across services

## Installation

```bash
dotnet add package Mamey.MessageBrokers.CQRS
```

## Quick Start

```csharp
using Mamey.MessageBrokers.CQRS;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMessageBrokers()
    .AddRabbitMQ()
    .AddServiceBusCommandDispatcher()
    .AddServiceBusEventDispatcher();

var app = builder.Build();
app.Run();
```

## Usage Examples

### Send Command via Message Broker

```csharp
@inject IBusPublisher BusPublisher

await BusPublisher.SendAsync(command, messageContext);
```

### Publish Event via Message Broker

```csharp
await BusPublisher.PublishAsync(@event, messageContext);
```

### Subscribe to Commands

```csharp
@inject IBusSubscriber BusSubscriber

BusSubscriber.SubscribeCommand<CreateUserCommand>();
```

### Subscribe to Events

```csharp
BusSubscriber.SubscribeEvent<UserCreatedEvent>();
```

## Core Components

- **ServiceBusMessageDispatcher**: Message broker-based dispatcher
- **Extension Methods**: Integration helpers for CQRS and message brokers

## Related Libraries

- **Mamey.CQRS.Commands**: Command pattern
- **Mamey.CQRS.Events**: Event pattern
- **Mamey.MessageBrokers**: Message broker abstractions

## Tags

#cqrs #message-brokers #rabbitmq #commands #events #mamey















