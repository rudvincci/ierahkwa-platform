# Mamey Microservice Implementation Rules

## Project Structure Compliance
- Follow exact solution structure: `Mamey.ServiceName/` with `src/` and `tests/` folders
- Include all required projects: Api, Application, BlazorWasm, Contracts, Domain, Infrastructure, Maui, Net
- Generate new GUIDs for all projects using `uuidgen` command
- Ensure proper project references: Application → Contracts, Domain; Infrastructure → Application, Domain, Contracts

## Mamey Library Integration
Always include these core Mamey packages with version `2.0.*`:
```xml
<PackageReference Include="Mamey" Version="2.0.*" />
<PackageReference Include="Mamey.Microservice.Infrastructure" Version="2.0.*" />
<PackageReference Include="Mamey.CQRS.Commands" Version="2.0.*" />
<PackageReference Include="Mamey.CQRS.Events" Version="2.0.*" />
<PackageReference Include="Mamey.CQRS.Queries" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.PostgreSQL" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.MongoDB" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.Redis" Version="2.0.*" />
<PackageReference Include="Mamey.Persistence.Minio" Version="2.0.*" />
<PackageReference Include="Mamey.MessageBrokers.RabbitMQ" Version="2.0.*" />
<PackageReference Include="Mamey.WebApi" Version="2.0.*" />
<PackageReference Include="Mamey.BlazorWasm" Version="2.0.*" />
```

## Data Distribution Strategy
Implement all four persistence layers with specific purposes:

### PostgreSQL (OLTP - Source of Truth)
- Purpose: ACID-compliant transactional data, aggregate roots, audit trails
- Implementation: Entity Framework with PostgreSQL
- Data: Aggregate roots, events, outbox messages, audit logs, configuration

### MongoDB (Read Models - Query Optimization)
- Purpose: Read-optimized projections, complex queries, analytics
- Implementation: MongoDB with read models
- Data: Denormalized projections, search indexes, analytics data, DID resolution cache

### Redis (Caching & Sessions)
- Purpose: High-performance caching, session management, rate limiting
- Implementation: Redis with TTL-based caching
- Data: Hot cache entries, session data, rate limiting counters, temporary data

### MinIO (Object Storage)
- Purpose: Large binary data, documents, backups
- Implementation: MinIO S3-compatible storage
- Data: File attachments, audit logs, backups, large documents

## Infrastructure Extensions Pattern
Always implement this pattern in `Infrastructure/Extensions.cs`:
```csharp
public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
{
    builder.Services.AddInfrastructure();

    return builder
        .AddApplication()
        .AddErrorHandler<ExceptionToResponseMapper>()
        .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
        .AddPostgres()
        .AddMongoDb()
        .AddRedisCache()
        .AddMinIOStorage()
        .AddMicroserviceSharedInfrastructure();
}
```

## Domain-Driven Design Structure
```
Domain/
├── Entities/
├── ValueObjects/
├── Enums/
├── Repositories/
└── Services/

Application/
├── Commands/
├── Events/
├── Queries/
├── DTO/
├── Handlers/
└── Services/

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
- Use `Microsoft.NET.Sdk.Razor` SDK
- Add `<AddRazorSupportForMvc>true</AddRazorSupportForMvc>`
- Implement MVVM pattern with ReactiveUI
- Include SignalR for real-time updates
- Register services: `ISignalRService`, `INavigationService`, ViewModels

## Data Seeding Implementation
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

## API Implementation Pattern
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

## Testing Strategy
- Unit Tests: Domain logic, application handlers
- Integration Tests: Database operations, external services
- End-to-End Tests: Full API workflows
- Performance Tests: Load testing, benchmarking
- Shared Tests: Common test utilities and fixtures

## Configuration Management
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

## Implementation Checklist
1. Generate new GUIDs for all projects
2. Create solution file with proper structure
3. Set up project references
4. Configure NuGet packages
5. Define entities and value objects
6. Implement domain services
7. Create repository interfaces
8. Create commands and queries
9. Implement command/query handlers
10. Define DTOs
11. Create application services
12. Implement EF Core context and repositories
13. Create MongoDB documents and repositories
14. Implement Redis caching services
15. Create MinIO storage services
16. Implement data seeding
17. Create controllers
18. Implement CQRS endpoints
19. Add authentication/authorization
20. Configure middleware pipeline
21. Create RCL project
22. Implement MVVM with ReactiveUI
23. Add SignalR for real-time updates
24. Create ViewModels and services
25. Create test projects
26. Implement unit tests
27. Add integration tests
28. Create end-to-end tests
29. Build all projects successfully
30. Run all tests
31. Verify API endpoints
32. Test data seeding
33. Validate persistence layers

## Best Practices
- Use consistent naming conventions
- Implement proper error handling
- Add comprehensive logging
- Follow SOLID principles
- Implement caching strategies
- Use async/await patterns
- Optimize database queries
- Monitor performance metrics
- Implement proper authentication
- Use HTTPS everywhere
- Validate all inputs
- Sanitize outputs
- Document all APIs
- Provide usage examples
- Maintain README files
- Update architecture diagrams

## Service Registration Pattern
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

## Compliance Requirements
- Implement comprehensive audit logging
- Use structured logging with correlation IDs
- Store audit logs in PostgreSQL and MinIO
- Implement global exception handling
- Use Mamey error handling patterns
- Provide meaningful error responses
- Implement Redis-based rate limiting
- Configure per-endpoint limits
- Support different rate limits for different user types
