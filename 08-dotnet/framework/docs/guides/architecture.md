# Mamey Framework Architecture

This document provides a comprehensive overview of the Mamey .NET microservices framework architecture, including its design principles, components, and how they work together to build scalable, maintainable microservices applications.

## Table of Contents

- [Overview](#overview)
- [Design Principles](#design-principles)
- [Architecture Layers](#architecture-layers)
- [Core Components](#core-components)
- [CQRS Architecture](#cqrs-architecture)
- [Message Broker Architecture](#message-broker-architecture)
- [Authentication Architecture](#authentication-architecture)
- [Persistence Architecture](#persistence-architecture)
- [Observability Architecture](#observability-architecture)
- [Infrastructure Architecture](#infrastructure-architecture)
- [Deployment Architecture](#deployment-architecture)
- [Security Architecture](#security-architecture)
- [Best Practices](#best-practices)

## Overview

Mamey is built on a modular, layered architecture that promotes separation of concerns, scalability, and maintainability. The framework follows modern microservices patterns and provides a comprehensive set of tools for building distributed systems.

### Key Architectural Principles

- **Modularity**: Components are loosely coupled and can be used independently
- **Scalability**: Designed to scale horizontally and vertically
- **Maintainability**: Clear separation of concerns and well-defined interfaces
- **Testability**: Components are designed for easy unit and integration testing
- **Observability**: Built-in support for logging, tracing, and metrics
- **Security**: Security-first design with built-in authentication and authorization

## Design Principles

### 1. Modular Architecture

Mamey follows a modular architecture where each component has a specific responsibility and can be used independently or in combination with other components.

```
┌─────────────────────────────────────────────────────────┐
│                  Mamey Modular Architecture             │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Core Framework                                        │
│  ├── CQRS (Commands, Queries, Events)                  │
│  ├── Message Brokers (RabbitMQ, Kafka, Azure SB)     │
│  ├── Authentication (JWT, Azure AD, Custom)            │
│  ├── Persistence (MongoDB, PostgreSQL, Redis, SQL)    │
│  ├── Observability (Logging, Tracing, Metrics)        │
│  └── Infrastructure (Consul, Fabio, Ntrada)          │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 2. Layered Architecture

The framework follows a layered architecture pattern with clear separation between different concerns:

- **Presentation Layer**: Web API, Controllers, DTOs
- **Application Layer**: Commands, Queries, Handlers, Services
- **Domain Layer**: Entities, Value Objects, Domain Services
- **Infrastructure Layer**: Repositories, External Services, Message Brokers
- **Cross-Cutting Concerns**: Logging, Security, Caching, Monitoring

### 3. Event-Driven Architecture

Mamey promotes event-driven architecture through its CQRS and message broker implementations, enabling loose coupling and scalability.

## Architecture Layers

### 1. Core Framework Layer

The core framework provides the foundation for all other components:

- **Service Registration**: Dependency injection and service registration
- **Configuration Management**: Centralized configuration management
- **Health Checks**: Built-in health check system
- **Exception Handling**: Global exception handling and error management
- **Logging**: Structured logging with correlation tracking

### 2. Application Layer

The application layer contains business logic and orchestration:

- **Commands**: Write operations that change system state
- **Queries**: Read operations optimized for performance
- **Events**: Domain events for cross-service communication
- **Handlers**: Command, query, and event handlers
- **Services**: Application services for business logic

### 3. Domain Layer

The domain layer contains business rules and domain logic:

- **Aggregate Roots**: Consistency boundaries with business invariants
- **Entities**: Domain entities with identity
- **Value Objects**: Immutable objects without identity
- **Domain Events**: Significant business occurrences
- **Domain Services**: Domain-specific business logic

### 4. Infrastructure Layer

The infrastructure layer provides technical capabilities:

- **Repositories**: Data access abstractions
- **Message Brokers**: Asynchronous messaging
- **External Services**: Third-party integrations
- **Caching**: Cache implementations
- **Storage**: File and object storage

## CQRS Architecture

### Command Side

The command side handles write operations:

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   Client    │────▶│   Command    │────▶│  Handler    │
│             │     │              │     │             │
└─────────────┘     └──────────────┘     └──────┬──────┘
                                                 │
                                                 ▼
                                         ┌─────────────┐
                                         │  Aggregate  │
                                         │    Root     │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │  Repository │
                                         │  (Write DB) │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │   Outbox    │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │    Event    │
                                         └─────────────┘
```

### Query Side

The query side handles read operations:

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   Client    │────▶│    Query     │────▶│  Handler    │
│             │     │              │     │             │
└─────────────┘     └──────────────┘     └──────┬──────┘
                                                 │
                                                 ▼
                                         ┌─────────────┐
                                         │   Cache     │
                                         │  (Redis)    │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │  Read Model │
                                         │   (MongoDB) │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │     DTO     │
                                         └─────────────┘
```

## Message Broker Architecture

### Outbox Pattern

The outbox pattern ensures reliable message publishing:

```
┌─────────────┐
│   Command   │
│   Handler   │
└──────┬──────┘
       │
       ├──▶ Write to Database
       │
       └──▶ Write to Outbox (Same Transaction)
                │
                ▼
         ┌─────────────┐
         │   Outbox    │
         │  (Database) │
         └──────┬──────┘
                │
                ▼
         ┌─────────────┐
         │ Background  │
         │  Processor  │
         └──────┬──────┘
                │
                ▼
         ┌─────────────┐
         │   RabbitMQ  │
         └─────────────┘
```

### Event Flow

Events flow through the system asynchronously:

```
┌─────────────┐
│  Publisher  │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Exchange  │
└──────┬──────┘
       │
       ├──▶ Queue 1 ───▶ Handler 1
       ├──▶ Queue 2 ───▶ Handler 2
       └──▶ Queue 3 ───▶ Handler 3
```

## Authentication Architecture

### JWT Authentication Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   Client    │────▶│   Login API  │────▶│   Auth      │
│             │     │              │     │  Service    │
└─────────────┘     └──────────────┘     └──────┬──────┘
                                                 │
                                                 ▼
                                         ┌─────────────┐
                                         │  Generate   │
                                         │  JWT Token  │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │  Return     │
                                         │   Token     │
                                         └──────┬──────┘
                                                │
                                                ▼
                                         ┌─────────────┐
                                         │  Validate   │
                                         │   Token     │
                                         └─────────────┘
```

## Persistence Architecture

### Dual Persistence Strategy

Mamey supports dual persistence for optimal performance:

**Write Model (PostgreSQL)**:
- ACID transactions
- Domain logic enforcement
- Optimized for writes
- Strong consistency

**Read Model (MongoDB)**:
- Denormalized data
- Optimized for queries
- Fast reads
- Eventual consistency

**Cache Layer (Redis)**:
- Frequently accessed data
- Session storage
- Distributed caching

## Observability Architecture

### Logging Architecture

```
┌─────────────┐
│ Application │
└──────┬──────┘
       │
       ├──▶ Console Sink
       ├──▶ File Sink
       ├──▶ Seq Sink
       └──▶ ELK Stack
```

### Tracing Architecture

```
┌─────────────┐
│ Application │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Jaeger    │
│  Collector  │
└──────┬──────┘
       │
       ├──▶ Storage
       └──▶ UI
```

### Metrics Architecture

```
┌─────────────┐
│ Application │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  Prometheus │
│   Scraper   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Grafana   │
│  Dashboard  │
└─────────────┘
```

## Infrastructure Architecture

### Service Discovery

```
┌─────────────┐
│  Microservice│
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Consul    │
│  Registry   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Fabio     │
│ Load Balancer│
└─────────────┘
```

### API Gateway

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Ntrada    │
│  API Gateway│
└──────┬──────┘
       │
       ├──▶ Service 1
       ├──▶ Service 2
       └──▶ Service 3
```

## Deployment Architecture

### Docker Compose

```
┌─────────────────────────────────────────┐
│         Docker Compose Stack             │
├─────────────────────────────────────────┤
│                                         │
│  ┌──────────┐  ┌──────────┐            │
│  │ Consul   │  │ RabbitMQ │            │
│  └──────────┘  └──────────┘            │
│                                         │
│  ┌──────────┐  ┌──────────┐            │
│  │ MongoDB  │  │PostgreSQL│            │
│  └──────────┘  └──────────┘            │
│                                         │
│  ┌──────────┐  ┌──────────┐            │
│  │  Redis   │  │  Jaeger  │            │
│  └──────────┘  └──────────┘            │
│                                         │
│  ┌──────────┐  ┌──────────┐            │
│  │ Service1 │  │ Service2 │            │
│  └──────────┘  └──────────┘            │
│                                         │
└─────────────────────────────────────────┘
```

### Kubernetes

```
┌─────────────────────────────────────────┐
│         Kubernetes Cluster              │
├─────────────────────────────────────────┤
│                                         │
│  ┌──────────────┐                      │
│  │  Ingress     │                      │
│  └──────┬───────┘                      │
│         │                               │
│  ┌──────▼───────┐                      │
│  │ API Gateway  │                      │
│  └──────┬───────┘                      │
│         │                               │
│  ┌──────▼───────┐                      │
│  │  Services    │                      │
│  │  (Deployments)│                    │
│  └──────┬───────┘                      │
│         │                               │
│  ┌──────▼───────┐                      │
│  │  StatefulSets│                      │
│  │  (Databases) │                      │
│  └──────────────┘                      │
│                                         │
└─────────────────────────────────────────┘
```

## Security Architecture

### Authentication Flow

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  API Gateway│
└──────┬──────┘
       │
       ├──▶ Validate Token
       │
       ▼
┌─────────────┐
│  Microservice│
└──────┬──────┘
       │
       ├──▶ Check Authorization
       │
       ▼
┌─────────────┐
│   Handler   │
└─────────────┘
```

### Authorization Flow

```
┌─────────────┐
│   Request   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  Extract    │
│   Claims    │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  Check      │
│  Permissions│
└──────┬──────┘
       │
       ├──▶ Allowed ───▶ Process Request
       │
       └──▶ Denied  ───▶ Return 403
```

## Best Practices

### Architecture Best Practices

1. **Modularity**: Keep components loosely coupled
2. **Separation of Concerns**: Clear boundaries between layers
3. **Single Responsibility**: Each component has one reason to change
4. **Dependency Inversion**: Depend on abstractions, not concretions
5. **Interface Segregation**: Use specific interfaces over general ones

### CQRS Best Practices

1. **Separate Models**: Use different models for commands and queries
2. **Event Sourcing**: Consider event sourcing for audit trails
3. **Read Models**: Denormalize data for read optimization
4. **Eventual Consistency**: Accept eventual consistency in read models
5. **Outbox Pattern**: Use outbox pattern for reliable messaging

### Message Broker Best Practices

1. **Idempotency**: Design handlers to be idempotent
2. **Dead Letter Queues**: Use DLQ for failed messages
3. **Message Versioning**: Version messages for compatibility
4. **Correlation IDs**: Use correlation IDs for request tracking
5. **Retry Policies**: Implement appropriate retry policies

### Persistence Best Practices

1. **Dual Persistence**: Use different databases for reads and writes
2. **Caching Strategy**: Implement multi-level caching
3. **Connection Pooling**: Configure connection pooling
4. **Indexing**: Use appropriate indexes for queries
5. **Pagination**: Always paginate large result sets

### Observability Best Practices

1. **Structured Logging**: Use structured logging with correlation IDs
2. **Distributed Tracing**: Implement distributed tracing
3. **Metrics Collection**: Collect relevant metrics
4. **Health Checks**: Implement comprehensive health checks
5. **Alerting**: Set up appropriate alerts

### Security Best Practices

1. **HTTPS**: Always use HTTPS in production
2. **Input Validation**: Validate all input data
3. **Token Expiration**: Set appropriate token expiration
4. **Rate Limiting**: Implement rate limiting
5. **Audit Logging**: Log all security events

## Additional Resources

- [Best Practices Documentation](best-practices.md)
- [Master Documentation](mamey-framework-master-documentation.md)
- Integration Patterns
- Deployment Guide

