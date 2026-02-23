# Mamey Microservice Implementation Prompt

## Overview
This prompt ensures consistent implementation of microservices using the Mamey Framework template, following established patterns for compliance, persistence layers, and Mamey library integration.

## Template Structure Compliance

### 1. Solution Structure
Always follow this exact structure for every microservice:

```
Mamey.ServiceName/
├── Mamey.ServiceName.sln
├── src/
│   ├── Mamey.ServiceName.Api/
│   ├── Mamey.ServiceName.Application/
│   ├── Mamey.ServiceName.BlazorWasm/
│   ├── Mamey.ServiceName.Contracts/
│   ├── Mamey.ServiceName.Domain/
│   ├── Mamey.ServiceName.Infrastructure/
│   ├── Mamey.ServiceName.Maui/
│   └── Mamey.ServiceName.Net/
└── tests/
    ├── Mamey.ServiceName.Tests.EndToEnd/
    ├── Mamey.ServiceName.Tests.Integration/
    ├── Mamey.ServiceName.Tests.Performance/
    ├── Mamey.ServiceName.Tests.Shared/
    └── Mamey.ServiceName.Tests.Unit/
```

### 2. Project References
Ensure proper project dependencies:
- **Application** → **Contracts**, **Domain**
- **Infrastructure** → **Application**, **Domain**, **Contracts**
- **Api** → **Application**, **Infrastructure**
- **BlazorWasm** → **Application** (as RCL)
- **Maui** → **Application**
- **Net** → **Application**

## Mamey Library Integration

### 3. Required Mamey Packages
Always include these core Mamey packages:

```xml
<!-- Core Framework -->
<PackageReference Include="Mamey" Version="2.0.*" />
<PackageReference Include="Mamey.Microservice.Infrastructure" Version="2.0.*" />

<!-- CQRS -->
<PackageReference Include="Mamey.CQRS.Commands" Version="2.0.*" />
<PackageReference Include="Mamey.CQRS.Events" Version="2.0.*" />
<PackageReference Include="Mamey.CQRS.Queries" Version="2.0.*" />

<!-- Persistence -->
<PackageReference Include="Mamey.Persistence.PostgreSQL" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.MongoDB" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.Redis" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.Minio" Version="2.0.*" />

<!-- Message Brokers -->
<PackageReference Include="Mamey.MessageBrokers.RabbitMQ" Version="2.0.*" />

<!-- Web API -->
<PackageReference Include="Mamey.WebApi" Version="2.0.*" />

<!-- BlazorWasm (for RCL) -->
<PackageReference Include="Mamey.BlazorWasm" Version="2.0.*" />
```

### 4. Infrastructure Extensions Pattern
Always implement this pattern in `Infrastructure/Extensions.cs`:

```csharp
public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
{
    builder.Services.AddInfrastructure();

    return builder
        .AddApplication()
        .AddErrorHandler<ExceptionToResponseMapper>()
        .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
        // PostgreSQL - Source of truth for transactional data
        .AddPostgres()
        // MongoDB - Read models and query optimization
        .AddMongoDb()
        // Redis - Caching and session management
        .AddRedisCache()
        // MinIO - Object storage for large files
        .AddMinIOStorage()
        .AddMicroserviceSharedInfrastructure();
}
```

## Data Distribution Strategy

### 5. Persistence Layer Implementation
Implement all four persistence layers with specific purposes:

#### PostgreSQL (OLTP - Source of Truth)
- **Purpose**: ACID-compliant transactional data, aggregate roots, audit trails
- **Implementation**: Entity Framework with PostgreSQL
- **Data**: Aggregate roots, events, outbox messages, audit logs, configuration

#### MongoDB (Read Models - Query Optimization)
- **Purpose**: Read-optimized projections, complex queries, analytics
- **Implementation**: MongoDB with read models
- **Data**: Denormalized projections, search indexes, analytics data, DID resolution cache

#### Redis (Caching & Sessions)
- **Purpose**: High-performance caching, session management, rate limiting
- **Implementation**: Redis with TTL-based caching
- **Data**: Hot cache entries, session data, rate limiting counters, temporary data

#### MinIO (Object Storage)
- **Purpose**: Large binary data, documents, backups
- **Implementation**: MinIO S3-compatible storage
- **Data**: File attachments, audit logs, backups, large documents

### 6. Data Seeding Implementation
Always implement `SimpleDataSeeder` for persistence layer initialization:

```csharp
internal sealed class SimpleDataSeeder : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_shouldSeed) return;
        
        await SeedMongoDBAsync();
        await WarmRedisCacheAsync();
        await InitializeMinIOBucketsAsync();
    }
}
```

## Domain-Driven Design Patterns

### 7. Domain Layer Structure
```
Domain/
├── Entities/
├── ValueObjects/
├── Enums/
├── Repositories/
└── Services/
```

### 8. Application Layer Structure
```
Application/
├── Commands/
├── Events/
├── Queries/
├── DTO/
├── Handlers/
└── Services/
```

### 9. Infrastructure Layer Structure
```
Infrastructure/
├── EF/
│   ├── Contexts/
│   ├── Repositories/
│   ├── Queries/
│   └── Extensions.cs
├── Mongo/
│   ├── Documents/
│   ├── Repositories/
│   ├── Queries/
│   └── Extensions.cs
├── Redis/
│   ├── Services/
│   └── Extensions.cs
├── MinIO/
│   ├── Services/
│   └── Extensions.cs
├── Seeding/
│   └── SimpleDataSeeder.cs
└── Extensions.cs
```

## BlazorWasm RCL Implementation

### 10. BlazorWasm as Razor Class Library
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
  </PropertyGroup>
</Project>
```

### 11. MVVM Pattern with ReactiveUI
```csharp
// ViewModelBase
public abstract class ViewModelBase : ReactiveObject, IDisposable
{
    protected CompositeDisposable Disposables { get; } = new();
}

// Service Registration
services.AddReactiveUI();
services.AddScoped<ISignalRService, SignalRService>();
services.AddScoped<INavigationService, NavigationService>();
```

## API Implementation

### 12. API Project Structure
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddMamey()
    .AddInfrastructure()
    .AddMicroserviceSharedInfrastructure();

var app = builder.Build();
app.UseInfrastructure();
app.UseMicroserviceSharedInfrastructure();
```

### 13. Controller Pattern
```csharp
[ApiController]
[Route("[controller]")]
public class EntityController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    
    // Implement CRUD operations using CQRS
}
```

## Testing Strategy

### 14. Test Project Structure
- **Unit Tests**: Domain logic, application handlers
- **Integration Tests**: Database operations, external services
- **End-to-End Tests**: Full API workflows
- **Performance Tests**: Load testing, benchmarking
- **Shared Tests**: Common test utilities and fixtures

## Configuration Management

### 15. Environment Configuration
```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=service;Username=postgres;Password=postgres",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379",
    "MinIO": "localhost:9000"
  },
  "Mamey": {
    "Seed": true,
    "Environment": "Development"
  }
}
```

## Compliance and Security

### 16. Audit Logging
- Implement comprehensive audit logging
- Use structured logging with correlation IDs
- Store audit logs in PostgreSQL and MinIO

### 17. Error Handling
- Implement global exception handling
- Use Mamey error handling patterns
- Provide meaningful error responses

### 18. Rate Limiting
- Implement Redis-based rate limiting
- Configure per-endpoint limits
- Support different rate limits for different user types

## Implementation Checklist

### 19. Pre-Implementation
- [ ] Generate new GUIDs for all projects
- [ ] Create solution file with proper structure
- [ ] Set up project references
- [ ] Configure NuGet packages

### 20. Domain Implementation
- [ ] Define entities and value objects
- [ ] Implement domain services
- [ ] Create repository interfaces
- [ ] Define enums and constants

### 21. Application Implementation
- [ ] Create commands and queries
- [ ] Implement command/query handlers
- [ ] Define DTOs
- [ ] Create application services

### 22. Infrastructure Implementation
- [ ] Implement EF Core context and repositories
- [ ] Create MongoDB documents and repositories
- [ ] Implement Redis caching services
- [ ] Create MinIO storage services
- [ ] Implement data seeding

### 23. API Implementation
- [ ] Create controllers
- [ ] Implement CQRS endpoints
- [ ] Add authentication/authorization
- [ ] Configure middleware pipeline

### 24. BlazorWasm Implementation
- [ ] Create RCL project
- [ ] Implement MVVM with ReactiveUI
- [ ] Add SignalR for real-time updates
- [ ] Create ViewModels and services

### 25. Testing Implementation
- [ ] Create test projects
- [ ] Implement unit tests
- [ ] Add integration tests
- [ ] Create end-to-end tests

### 26. Final Validation
- [ ] Build all projects successfully
- [ ] Run all tests
- [ ] Verify API endpoints
- [ ] Test data seeding
- [ ] Validate persistence layers

## Best Practices

### 27. Code Quality
- Use consistent naming conventions
- Implement proper error handling
- Add comprehensive logging
- Follow SOLID principles

### 28. Performance
- Implement caching strategies
- Use async/await patterns
- Optimize database queries
- Monitor performance metrics

### 29. Security
- Implement proper authentication
- Use HTTPS everywhere
- Validate all inputs
- Sanitize outputs

### 30. Documentation
- Document all APIs
- Provide usage examples
- Maintain README files
- Update architecture diagrams

## Example Implementation

### 31. Service Registration Pattern
```csharp
// Infrastructure/Extensions.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    // EF Core
    services.AddDbContext<ServiceDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    // MongoDB
    services.AddMongo();
    
    // Redis
    services.AddRedis();
    
    // MinIO
    services.AddMinIO();
    
    // Repositories
    services.AddScoped<IEntityRepository, EntityRepository>();
    
    // Services
    services.AddScoped<EntityCacheService>();
    services.AddScoped<EntityStorageService>();
    
    // Seeding
    services.AddHostedService<SimpleDataSeeder>();
    
    return services;
}
```

### 32. Data Seeding Pattern
```csharp
// Infrastructure/Seeding/SimpleDataSeeder.cs
internal sealed class SimpleDataSeeder : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_shouldSeed) return;
        
        await SeedMongoDBAsync();
        await WarmRedisCacheAsync();
        await InitializeMinIOBucketsAsync();
    }
}
```

This prompt ensures every microservice follows the established Mamey patterns, maintains consistency across the ecosystem, and leverages Mamey libraries effectively while providing comprehensive persistence layer support.
