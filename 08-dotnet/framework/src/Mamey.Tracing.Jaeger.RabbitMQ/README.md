# Mamey.Tracing.Jaeger.RabbitMQ

**Library**: `Mamey.Tracing.Jaeger.RabbitMQ`  
**Location**: `Mamey/src/Mamey.Tracing.Jaeger.RabbitMQ/`  
**Type**: Observability Library - Jaeger Tracing for RabbitMQ  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Tracing.Jaeger.RabbitMQ`

## Overview

Mamey.Tracing.Jaeger.RabbitMQ provides Jaeger distributed tracing integration for RabbitMQ message processing in the Mamey framework. It includes span context propagation and message tracing.

### Key Features

- **RabbitMQ Tracing**: Automatic tracing for RabbitMQ messages
- **Span Context Propagation**: Propagate span context through messages
- **Message Tracing**: Trace message processing with Jaeger
- **Error Tracking**: Automatic error tracking in spans

## Installation

```bash
dotnet add package Mamey.Tracing.Jaeger.RabbitMQ
```

## Quick Start

```csharp
using Mamey.Tracing.Jaeger.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddRabbitMq()
    .AddRabbitMqPlugins(plugins => plugins.AddJaegerRabbitMqPlugin());

var app = builder.Build();
app.Run();
```

## Core Components

- **JaegerPlugin**: RabbitMQ plugin for Jaeger tracing
- **RabbitMqPlugin**: Base plugin class

## Usage Examples

### Add Jaeger Plugin

```csharp
builder.Services
    .AddRabbitMq()
    .AddRabbitMqPlugins(plugins => plugins.AddJaegerRabbitMqPlugin());
```

## Related Libraries

- **Mamey.Tracing.Jaeger**: Jaeger tracing infrastructure
- **Mamey.MessageBrokers.RabbitMQ**: RabbitMQ message broker

## Tags

#jaeger #tracing #rabbitmq #distributed-tracing #mamey















