# Mamey.Net

**Library**: `Mamey.Net`  
**Location**: `Mamey/src/Mamey.Net/`  
**Type**: Networking Library - SignalR & HTTP  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Net`

## Overview

Mamey.Net provides networking functionality for the Mamey framework, including SignalR hubs, HTTP utilities, and application startup infrastructure.

### Key Features

- **SignalR Hubs**: Base hub implementation with helper methods
- **HTTP Utilities**: HTTP context extensions and utilities
- **Application Startup**: Application initialization infrastructure
- **Correlation Context**: Request correlation tracking
- **Banner Display**: Application banner display on startup
- **Base User Types**: Base user type definitions

## Installation

```bash
dotnet add package Mamey.Net
```

## Quick Start

```csharp
using Mamey;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMameyNet();

var app = builder.Build();
app.UseMamey();
app.Run();
```

## Core Components

- **BaseHub**: Base SignalR hub class
- **MameyBuilder**: Mamey framework builder
- **BaseUser**: Base user type
- **IBaseUser**: Base user interface
- **HttpContextExtensions**: HTTP context extension methods

## Usage Examples

### SignalR Hub

```csharp
using Mamey.Net.Hubs;

public class ChatHub : BaseHub
{
    public async Task SendMessage(string message)
    {
        await SendMessageToAllClientsAsync(message);
    }
}
```

### Application Banner

```json
{
  "app": {
    "Name": "Mamey App",
    "Version": "1.0.0",
    "DisplayBanner": true,
    "DisplayVersion": true
  }
}
```

## Related Libraries

- **Mamey**: Core framework
- **Mamey.CQRS.Events**: Event handling

## Tags

#signalr #networking #http #hubs #mamey















