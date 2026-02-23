# Mamey.Logging.CQRS

**Library**: `Mamey.Logging.CQRS`  
**Location**: `Mamey/src/Mamey.Logging.CQRS/`  
**Type**: Cross-Cutting Library - CQRS Logging  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Logging.CQRS`

## Overview

Mamey.Logging.CQRS provides automatic logging decorators for CQRS command and event handlers in the Mamey framework. It adds structured logging before, after, and on errors for all handlers using the decorator pattern.

### Key Features

- **Automatic Logging**: Decorator-based logging for handlers
- **Command Handler Logging**: Log commands before/after execution
- **Event Handler Logging**: Log events before/after execution
- **Error Logging**: Structured error logging with exception templates
- **Template Support**: Customizable log templates using SmartFormat
- **Non-Intrusive**: Zero changes to handler code required

## Installation

```bash
dotnet add package Mamey.Logging.CQRS
```

## Quick Start

```csharp
using Mamey.Logging.CQRS;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddCommandHandlersLogging()
    .AddEventHandlersLogging();

var app = builder.Build();
app.Run();
```

## Core Components

- **CommandHandlerLoggingDecorator**: Logging decorator for command handlers
- **EventHandlerLoggingDecorator**: Logging decorator for event handlers
- **HandlerLogTemplate**: Log template model
- **IMessageToLogTemplateMapper**: Template mapper interface

## Usage Examples

### Custom Log Template Mapper

```csharp
public class CustomLogTemplateMapper : IMessageToLogTemplateMapper
{
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        return new HandlerLogTemplate
        {
            Before = "Processing {Type} with ID {Id}",
            After = "Completed {Type} with ID {Id}",
            OnError = new Dictionary<Type, string>
            {
                { typeof(ValidationException), "Validation failed for {Type}: {Message}" },
                { typeof(Exception), "Error processing {Type}: {Message}" }
            }
        };
    }
}
```

## Related Libraries

- **Mamey.CQRS.Commands**: Command handlers
- **Mamey.CQRS.Events**: Event handlers
- **Mamey.Logging**: Logging infrastructure

## Tags

#logging #cqrs #decorator #commands #events #mamey















