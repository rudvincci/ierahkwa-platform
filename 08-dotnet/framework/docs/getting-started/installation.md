# Installation Guide

This guide will help you install and set up the Mamey Framework for your .NET application.

## Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 (17.8+) or VS Code with C# extension
- Git (for cloning the repository)

## Installation Methods

### 1. NuGet Package Manager

Install individual Mamey packages using the NuGet Package Manager:

```bash
# Core framework
dotnet add package Mamey

# CQRS pattern
dotnet add package Mamey.CQRS.Commands
dotnet add package Mamey.CQRS.Events
dotnet add package Mamey.CQRS.Queries

# Authentication
dotnet add package Mamey.Auth
dotnet add package Mamey.Auth.Jwt

# Microservices infrastructure
dotnet add package Mamey.Microservice.Infrastructure

# Message brokers
dotnet add package Mamey.MessageBrokers.RabbitMQ

# Persistence
dotnet add package Mamey.Persistence.MongoDB
dotnet add package Mamey.Persistence.Redis

# Observability
dotnet add package Mamey.Logging
dotnet add package Mamey.Tracing.Jaeger
```

### 2. Package Manager Console

In Visual Studio, use the Package Manager Console:

```powershell
Install-Package Mamey
Install-Package Mamey.CQRS.Commands
Install-Package Mamey.CQRS.Events
Install-Package Mamey.CQRS.Queries
```

### 3. PackageReference in .csproj

Add package references directly to your project file:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Mamey" Version="2.0.4" />
    <PackageReference Include="Mamey.CQRS.Commands" Version="2.0.4" />
    <PackageReference Include="Mamey.CQRS.Events" Version="2.0.4" />
    <PackageReference Include="Mamey.CQRS.Queries" Version="2.0.4" />
    <PackageReference Include="Mamey.Microservice.Infrastructure" Version="2.0.4" />
  </ItemGroup>
</Project>
```

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Mamey-io/Mamey.git
cd Mamey
```

### 2. Build the Solution

```bash
# Build all projects
dotnet build

# Build specific project
dotnet build src/Mamey/src/Mamey/Mamey.csproj
```

### 3. Run Tests

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test src/Mamey.Tests/
```

### 4. Pack NuGet Packages

```bash
# Pack all packages
./pack.sh src http://localhost:4000/v3/index.json

# Pack specific package
dotnet pack src/Mamey/src/Mamey/Mamey.csproj -c Release -o ./nupkgs
```

## Configuration

### 1. Add Mamey to Your Application

```csharp
using Mamey;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Configure your services
mameyBuilder
    .AddMicroserviceSharedInfrastructure()
    .AddCommandHandlers()
    .AddEventHandlers();

var app = builder.Build();

// Use Mamey middleware
app.UseSharedInfrastructure();

app.Run();
```

### 2. Configuration Files

Create `appsettings.json`:

```json
{
  "Mamey": {
    "Logging": {
      "Level": "Information"
    },
    "MessageBroker": {
      "Type": "RabbitMQ",
      "ConnectionString": "amqp://localhost:5672"
    },
    "Persistence": {
      "MongoDB": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "mamey"
      }
    }
  }
}
```

## Next Steps

1. [Quick Start Tutorial](quick-start.md) - Build your first microservice
2. [Architecture Overview](../guides/architecture.md) - Understand the framework architecture
3. [Core Libraries](../libraries/core/mamey.md) - Learn about the core framework

## Troubleshooting

### Common Issues

**Package not found**: Ensure you're using the correct package source and version.

**Build errors**: Make sure you have .NET 9.0 SDK installed and all dependencies are restored.

**Configuration errors**: Verify your `appsettings.json` configuration matches the expected schema.

### Getting Help

- Check the FAQ
- Review Troubleshooting Guide
- Open an issue on [GitHub](https://github.com/Mamey-io/Mamey/issues)
