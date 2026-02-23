# Mamey Framework

**A modular, enterprise-grade .NET microservices framework**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-AGPL--3.0-blue.svg)](LICENSE)
[![Version](https://img.shields.io/badge/version-2.0.*-green.svg)](https://www.nuget.org/packages/Mamey)

## Overview

Mamey is a comprehensive .NET microservices and distributed systems framework designed for enterprise-grade applications. It consists of **110+ independent helper libraries** that work together to address common infrastructural challenges in building scalable, resilient, and maintainable microservices.

### Key Characteristics

- **Cloud-Agnostic**: Uses CNCF tools that work across cloud providers
- **Modular**: Libraries are independent and can be used separately
- **Type-Safe**: Strongly-typed identifiers prevent primitive obsession
- **Event-Driven**: Built for asynchronous, event-driven architectures
- **Domain-Focused**: Emphasizes Domain-Driven Design patterns
- **Production-Ready**: Designed with operational concerns as first-class citizens

### Design Philosophy

Mamey consists of **helper libraries** that are generally **independent from one another**. It is **not a framework nor a universal solution**. Rather, **it includes a collection of extension methods and additional abstractions** to address common infrastructural challenges like:

- Routing and API management
- Service discovery and load balancing
- Distributed tracing and observability
- Asynchronous messaging
- Persistence and data access
- Authentication and authorization
- Security and compliance

## Quick Start

### Installation

```bash
# Install the core Mamey package
dotnet add package Mamey

# Install additional packages as needed
dotnet add package Mamey.CQRS.Commands
dotnet add package Mamey.CQRS.Queries
dotnet add package Mamey.CQRS.Events
dotnet add package Mamey.MessageBrokers.RabbitMQ
dotnet add package Mamey.Auth.Jwt
dotnet add package Mamey.Microservice.Infrastructure
```

### Basic Setup

```csharp
using Mamey;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMamey()
    .AddMicroserviceSharedInfrastructure()
    .AddCommands()
    .AddQueries()
    .AddEvents()
    .AddRabbitMQ()
    .AddMessageOutbox()
    .AddMongo()
    .AddRedis()
    .AddJwt()
    .AddWebApi()
    .AddConsul()
    .AddJaeger()
    .AddLogging();

var app = builder.Build();

app.UseMamey()
    .UseJaeger()
    .UseSharedInfrastructure()
    .UseWebApi();

app.Run();
```

## Documentation

### [Complete Documentation](docs/)

**Start here:** [Documentation Index](docs/README.md)

### Getting Started

- **[Installation Guide](docs/getting-started/installation.md)** - Install and configure Mamey Framework
- **[Quick Start Tutorial](docs/getting-started/quick-start.md)** - Build your first microservice in minutes
- **[Architecture Overview](docs/guides/architecture.md)** - Understand the framework architecture

### Core Concepts

- **[Master Documentation](docs/guides/mamey-framework-master-documentation.md)** - Comprehensive reference with all concepts, examples, and patterns (74KB)
- **[Best Practices](docs/guides/best-practices.md)** - Best practices for building microservices
- **[CQRS Pattern](docs/libraries/cqrs/)** - Command Query Responsibility Segregation
- **[Event-Driven Architecture](docs/guides/)** - Building event-driven microservices

### Library Documentation

- **[All Libraries](docs/libraries/README.md)** - Complete library index (110+ libraries)
- **[Core Framework](docs/libraries/core/)** - Foundation library
- **[CQRS Libraries](docs/libraries/cqrs/)** - Commands, Queries, Events
- **[Message Brokers](docs/libraries/messaging/)** - RabbitMQ, Kafka, Azure Service Bus
- **[Authentication](docs/libraries/auth/)** - JWT, Azure AD, Custom authentication
- **[Identity](docs/libraries/identity/)** - Identity management libraries
- **[Persistence](docs/libraries/persistence/)** - MongoDB, PostgreSQL, Redis, SQL
- **[Observability](docs/libraries/observability/)** - Logging, Tracing, Metrics
- **[Infrastructure](docs/libraries/infrastructure/)** - Service Discovery, API Gateway, Load Balancing
- **[Standards](docs/libraries/standards/)** - ISO standards, AAMVA, Travel Identity
- **[Integration](docs/libraries/integration/)** - Stripe, Twilio, Azure, Blockchain, etc.
- **[UI & Client](docs/libraries/ui/)** - Blazor, MAUI, WebAssembly
- **[Utilities](docs/libraries/utilities/)** - Algorithms, Barcode, Excel, Image processing

### Guides & Examples

- **[How to Build a Microservice](docs/guides/how-to-build-a-microservice.md)** - Step-by-step guide
- **[Integration Patterns](docs/guides/)** - Common integration patterns
- **[Examples](docs/examples/)** - Complete working examples

## Key Features

### Modular Architecture
- **110+ independent libraries** that can be used separately or together
- **Pluggable components** for maximum flexibility
- **Clear separation of concerns** with well-defined interfaces

### CQRS Support
- **Command pattern** for write operations
- **Query pattern** for read operations
- **Event handling** for cross-service communication
- **Outbox pattern** for reliable messaging

### Message Brokers
- **RabbitMQ** - Reliable messaging with dead letter queues
- **Kafka** - High-throughput event streaming
- **Azure Service Bus** - Cloud-native messaging
- **Unified abstraction** for easy switching

### Authentication & Authorization
- **JWT** - Token-based authentication
- **Azure AD** - B2B and B2C integration
- **Custom authentication** - Flexible auth providers
- **Role-based authorization** - Built-in RBAC support

### Persistence
- **MongoDB** - Document database for flexible schemas
- **PostgreSQL** - Relational database for ACID compliance
- **Redis** - In-memory cache and session storage
- **Dual persistence** - Optimize reads and writes separately

### Observability
- **Structured logging** with Serilog
- **Distributed tracing** with Jaeger
- **Metrics collection** with Prometheus
- **Health checks** for monitoring

### Infrastructure
- **Service discovery** with Consul
- **Load balancing** with Fabio
- **API Gateway** with Ntrada
- **Secrets management** with Vault

## Library Categories

See [Complete Library Documentation](docs/libraries/README.md) for full details on all 110+ libraries organized by category.

### Core Framework (7 libraries)
- **Mamey** - Core framework with builder pattern and base abstractions
- **Mamey.Net** - Network utilities and HTTP client extensions
- **Mamey.Micro** - Microservice utilities
- **Mamey.Microservice.Abstractions** - Microservice abstractions
- **Mamey.Microservice.Infrastructure** - Microservice infrastructure
- **Mamey.MicroMonolith.Abstractions** - MicroMonolith abstractions
- **Mamey.MicroMonolith.Infrastructure** - MicroMonolith infrastructure

### CQRS (6 libraries)
- **Mamey.CQRS.Commands** - Command pattern implementation
- **Mamey.CQRS.Queries** - Query pattern implementation
- **Mamey.CQRS.Events** - Event handling and dispatching
- **Mamey.Logging.CQRS** - CQRS logging decorators
- **Mamey.MessageBrokers.CQRS** - Message broker CQRS integration
- **Mamey.WebApi.CQRS** - Web API CQRS integration

### Messaging (5 libraries)
- **Mamey.MessageBrokers** - Message broker abstractions
- **Mamey.MessageBrokers.RabbitMQ** - RabbitMQ implementation
- **Mamey.MessageBrokers.Outbox** - Outbox pattern
- **Mamey.MessageBrokers.Outbox.EntityFramework** - EF Core outbox
- **Mamey.MessageBrokers.Outbox.Mongo** - MongoDB outbox

### Authentication & Authorization (13 libraries)
- **Mamey.Auth** - Core authentication
- **Mamey.Auth.Abstractions** - Authentication abstractions
- **Mamey.Auth.Jwt** - JWT authentication
- **Mamey.Auth.Azure** - Azure AD integration
- **Mamey.Auth.Azure.B2B** - Azure B2B
- **Mamey.Auth.Azure.B2C** - Azure B2C
- **Mamey.Auth.Azure.B2B.BlazorWasm** - Azure B2B Blazor WebAssembly
- **Mamey.Auth.Jwt.BlazorWasm** - JWT Blazor WebAssembly
- **Mamey.Auth.Jwt.Server** - JWT server-side authentication
- **Mamey.Auth.Identity** - ASP.NET Core Identity
- **Mamey.Auth.Distributed** - Distributed authentication
- **Mamey.Auth.Decentralized** - Decentralized authentication
- **Mamey.Auth.DecentralizedIdentifiers** - DID authentication

### Identity Management (11 libraries)
- **Mamey.Identity.Core** - Core identity
- **Mamey.Identity.AspNetCore** - ASP.NET Core identity
- **Mamey.Identity.Azure** - Azure identity
- **Mamey.Identity.Blazor** - Blazor identity
- **Mamey.Identity.Jwt** - JWT Identity
- **Mamey.Identity.EntityFramework** - Entity Framework Identity
- **Mamey.Identity.Redis** - Redis Identity caching
- **Mamey.Identity.Decentralized** - Decentralized Identity
- **Mamey.Identity.Distributed** - Distributed Identity
- **Mamey.Azure.Identity** - Azure Identity
- **Mamey.Azure.Identity.BlazorWasm** - Azure Identity Blazor WebAssembly

### Persistence (8 libraries)
- **Mamey.Persistence.MongoDB** - MongoDB integration
- **Mamey.Persistence.PostgreSQL** - PostgreSQL integration
- **Mamey.Persistence.Redis** - Redis caching
- **Mamey.Persistence.SQL** - SQL database support
- **Mamey.Persistence.MySQL** - MySQL integration
- **Mamey.Persistence.Minio** - MinIO object storage
- **Mamey.Persistence.OpenStack** - OpenStack storage

### Observability (6 libraries)
- **Mamey.Logging** - Logging infrastructure
- **Mamey.Tracing.Jaeger** - Distributed tracing
- **Mamey.Tracing.Jaeger.RabbitMQ** - Jaeger RabbitMQ integration
- **Mamey.Metrics.Prometheus** - Prometheus metrics
- **Mamey.Metrics.AppMetrics** - AppMetrics
- **Mamey.OpenTracingContrib** - OpenTracing contributions

### Infrastructure (13 libraries)
- **Mamey.WebApi** - Web API framework
- **Mamey.WebApi.Security** - Web API security
- **Mamey.WebApi.Swagger** - Swagger/OpenAPI integration
- **Mamey.Microservice.Infrastructure** - Microservice infrastructure
- **Mamey.Discovery.Consul** - Consul service discovery
- **Mamey.LoadBalancing.Fabio** - Fabio load balancing
- **Mamey.Ntrada** - API Gateway
- **Mamey.Secrets.Vault** - HashiCorp Vault
- **Mamey.Security** - Security utilities
- **Mamey.Policies** - Policy enforcement
- **Mamey.Modules** - Modular architecture
- **Mamey.Modules.MessageBrokers** - Modular message brokers
- **Mamey.Modules.MessageBrokers.Outbox** - Modular outbox

### Integration (11 libraries)
- **Mamey.Stripe** - Stripe payment processing
- **Mamey.Visa** - Visa payment integration
- **Mamey.Mifos** - Mifos X financial services
- **Mamey.Emails** - Email services
- **Mamey.Twilio** - Twilio SMS and voice
- **Mamey.Graph** - Microsoft Graph API
- **Mamey.Blockchain** - Blockchain integration
- **Mamey.Web3** - Web3 integration
- **Mamey.OpenBanking** - Open Banking APIs
- **Mamey.Ktt** - Key Telex Transfer messaging
- **Mamey.Binimoy** - Binimoy protocol

### UI & Client (4 libraries)
- **Mamey.Blazor.Abstractions** - Blazor abstractions
- **Mamey.Blazor.Identity** - Blazor Identity
- **Mamey.BlazorWasm** - Blazor WebAssembly
- **Mamey.Maui** - .NET MAUI integration

### Utilities (8 libraries)
- **Mamey.Adobe** - Adobe PDF Services
- **Mamey.Algorithms** - Algorithms and data structures
- **Mamey.Barcode** - Barcode generation
- **Mamey.Excel** - Excel processing
- **Mamey.Image** - Image processing
- **Mamey.Word** - Word document processing
- **Mamey.Mock** - Mock data generation
- **Mamey.Templates** - Document templates

### Standards & Compliance (14 libraries)
- **Mamey.ISO.Abstractions** - ISO standard abstractions
- **Mamey.ISO.ISO3166** - ISO 3166 country codes
- **Mamey.ISO.ISO4217** - ISO 4217 currency codes
- **Mamey.ISO.ISO639** - ISO 639 language codes
- **Mamey.ISO.ISO13616** - ISO 13616 IBAN
- **Mamey.ISO.ISO20022** - ISO 20022 financial messaging
- **Mamey.ISO.ISO22301** - ISO 22301 business continuity
- **Mamey.ISO.ISO27001** - ISO 27001 information security
- **Mamey.ISO.ISO8583** - ISO 8583 financial transaction messages
- **Mamey.ISO.ISO9362** - ISO 9362 SWIFT/BIC codes
- **Mamey.ISO.PCI_DSS** - PCI DSS compliance
- **Mamey.AmvvaStandards** - AAMVA driver license standards
- **Mamey.TravelIdentityStandards** - ICAO travel document standards
- **Mamey.Biometrics** - Biometric processing

### Azure (2 libraries)
- **Mamey.Azure.Abstractions** - Azure abstractions
- **Mamey.Azure.Blobs** - Azure Blob Storage

**Note:** Azure Identity libraries are listed under Identity Management.

### HTTP (2 libraries)
- **Mamey.Http** - HTTP client utilities
- **Mamey.Http.RestEase** - RestEase integration

### Documentation (1 library)
- **Mamey.Docs.Swagger** - Swagger/OpenAPI documentation

See [Complete Library Documentation](docs/libraries/README.md) for full details on all 110+ libraries.

## Technology Stack

- **.NET 9.0** - Modern, cross-platform framework
- **CQRS** - Command Query Responsibility Segregation
- **Event-Driven** - Asynchronous, event-driven architecture
- **Message Brokers** - RabbitMQ, Kafka, Azure Service Bus
- **Databases** - PostgreSQL, MongoDB, Redis, MySQL
- **Authentication** - JWT, Azure AD, Custom providers
- **Observability** - Serilog, Jaeger, Prometheus
- **Service Discovery** - Consul
- **API Gateway** - Ntrada

## Requirements

- **.NET 9.0 SDK** or later
- **.NET Standard 2.1** compatible frameworks
- **Visual Studio 2022** or **VS Code** (recommended)
- **Docker** (for containerized services)

## Examples

### Complete E-Commerce Microservice
See [Master Documentation](docs/guides/mamey-framework-master-documentation.md) for a complete e-commerce order processing microservice example with:
- Domain models with aggregates
- Commands and queries
- Event handlers
- Read/write models
- API endpoints
- Complete service setup

### Integration Patterns
See [Integration Patterns](docs/guides/) for:
- CQRS with dual persistence
- Saga orchestration
- Outbox pattern
- Correlation tracking
- Multi-tenant architecture
- Caching strategies

## Contributing

We welcome contributions! Please see our [Contributing Guide](docs/guides/) for details.

## Support

- **[Documentation](docs/)** - Comprehensive documentation (110+ libraries)
- **[GitHub Issues](https://github.com/Mamey-io/Mamey/issues)** - Report bugs and request features
- **[Community Discussions](https://github.com/Mamey-io/Mamey/discussions)** - Ask questions and share ideas

## License

This project is licensed under the **AGPL-3.0 License** - see the LICENSE file for details.

## Organization

**Mamey Technologies (mamey.io)**  
Copyright © Mamey Technologies

---

**Ready to get started?** Check out the [Quick Start Tutorial](docs/getting-started/quick-start.md) or browse the [Complete Documentation](docs/).
