# Mamey.Metrics.Prometheus

**Library**: `Mamey.Metrics.Prometheus`  
**Location**: `Mamey/src/Mamey.Metrics.Prometheus/`  
**Type**: Observability Library - Prometheus Metrics  
**Version**: 2.0.*  
**Files**: Multiple C# files  
**Namespace**: `Mamey.Metrics.Prometheus`

## Overview

Mamey.Metrics.Prometheus provides comprehensive Prometheus metrics integration for the Mamey framework. It includes HTTP metrics, gRPC metrics, .NET runtime metrics, and system metrics with automatic collection.

### Key Features

- **HTTP Metrics**: Automatic HTTP request/response metrics
- **gRPC Metrics**: Automatic gRPC call metrics
- **.NET Runtime Metrics**: GC, JIT, thread pool, contention metrics
- **System Metrics**: CPU, memory, disk metrics
- **Metric Server**: Exposes metrics endpoint for Prometheus scraping
- **Middleware**: Automatic metric collection middleware
- **Security**: API key and host-based access control

## Installation

```bash
dotnet add package Mamey.Metrics.Prometheus
```

## Quick Start

```csharp
using Mamey.Metrics.Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddPrometheus();

var app = builder.Build();
app.UsePrometheus();
app.Run();
```

## Configuration

```json
{
  "prometheus": {
    "Enabled": true,
    "Endpoint": "/metrics",
    "ApiKey": "your-api-key",
    "AllowedHosts": ["localhost", "127.0.0.1"]
  }
}
```

## Core Components

- **PrometheusOptions**: Configuration options
- **PrometheusMiddleware**: Metrics collection middleware
- **PrometheusJob**: Background service for .NET runtime metrics

## Usage Examples

### Access Metrics Endpoint

```bash
curl http://localhost:5000/metrics?apiKey=your-api-key
```

## Related Libraries

- **Mamey.Logging**: Logging infrastructure
- **Mamey.Tracing.Jaeger**: Distributed tracing

## Tags

#prometheus #metrics #observability #monitoring #mamey















