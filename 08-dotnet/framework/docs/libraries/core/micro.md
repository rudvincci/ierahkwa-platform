# Mamey.Micro

**Library**: `Mamey.Micro`  
**Location**: `Mamey/src/Mamey.Micro/`  
**Type**: Microservice Infrastructure Library  
**Version**: 2.0.*  
**Files**: Minimal C# files  
**Namespace**: `Mamey.Micro`

## Overview

Mamey.Micro provides microservice infrastructure support for the Mamey framework. This library serves as a foundation for building microservices with the Mamey framework.

## Installation

```bash
dotnet add package Mamey.Micro
```

## Quick Start

```csharp
using Mamey.Micro;

var builder = WebApplication.CreateBuilder(args);

// Microservice infrastructure setup
builder.Services.AddMamey();

var app = builder.Build();
app.Run();
```

## Related Libraries

- **Mamey**: Core framework
- **Mamey.Microservice.Infrastructure**: Full microservice infrastructure

## Tags

#microservice #infrastructure #mamey

