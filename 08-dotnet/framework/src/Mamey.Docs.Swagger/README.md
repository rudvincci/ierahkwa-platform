# Mamey.Docs.Swagger

**Library**: `Mamey.Docs.Swagger`  
**Location**: `Mamey/src/Mamey.Docs.Swagger/`  
**Type**: Documentation Library - Swagger/OpenAPI  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Docs.Swagger`

## Overview

Mamey.Docs.Swagger provides Swagger/OpenAPI documentation integration for the Mamey framework. It includes Swagger UI, ReDoc support, and security definitions.

### Key Features

- **Swagger UI**: Interactive API documentation
- **ReDoc Support**: Alternative documentation UI
- **OpenAPI 3.0**: OpenAPI 3.0 specification support
- **Security Definitions**: JWT Bearer authentication support
- **Annotations**: XML documentation support

## Installation

```bash
dotnet add package Mamey.Docs.Swagger
```

## Quick Start

```csharp
using Mamey.Docs.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddSwaggerDocs(options =>
    {
        options.Enabled = true;
        options.Title = "Mamey API";
        options.Version = "1.0";
        options.IncludeSecurity = true;
    });

var app = builder.Build();
app.UseSwaggerDocs();
app.Run();
```

## Configuration

```json
{
  "swagger": {
    "Enabled": true,
    "ReDocEnabled": false,
    "Name": "v1",
    "Title": "Mamey API",
    "Version": "1.0",
    "RoutePrefix": "swagger",
    "IncludeSecurity": true
  }
}
```

## Core Components

- **SwaggerOptions**: Configuration options
- **ISwaggerOptionsBuilder**: Fluent options builder
- **SwaggerOptionsBuilder**: Options builder implementation

## Usage Examples

### Access Swagger UI

```bash
http://localhost:5000/swagger
```

### Access ReDoc

```bash
http://localhost:5000/swagger (when ReDocEnabled = true)
```

## Related Libraries

- **Mamey.WebApi**: Web API framework

## Tags

#swagger #openapi #documentation #api-docs #mamey















