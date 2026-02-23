# Mamey.Microservice.Infrastructure

The Mamey.Microservice.Infrastructure library provides a comprehensive infrastructure layer for building microservices with the Mamey framework. It includes shared infrastructure components, authentication, context management, messaging, persistence, observability, and security features that are essential for production-ready microservices.

## Technical Overview

Mamey.Microservice.Infrastructure implements a complete microservice infrastructure stack that includes:

- **Shared Infrastructure**: Common infrastructure components for all microservices
- **Authentication & Authorization**: JWT-based authentication with role-based access control
- **Context Management**: Request context and correlation tracking across services
- **Messaging**: Message brokers, outbox pattern, and event processing
- **Persistence**: MongoDB, Redis, and SQL database integrations
- **Observability**: Logging, metrics, and distributed tracing with Jaeger
- **Security**: Certificate authentication, encryption, and security utilities
- **Service Discovery**: Consul integration for service registration and discovery
- **Load Balancing**: Fabio load balancer integration
- **API Documentation**: Swagger/OpenAPI integration
- **Serialization**: JSON serialization with custom converters

## Architecture

The library follows a layered microservice architecture:

```mermaid
graph TB
    subgraph "Application Layer"
        A[Controllers] --> B[Command Handlers]
        C[Query Handlers] --> D[Event Handlers]
    end
    
    subgraph "Infrastructure Layer"
        E[Authentication] --> F[Context Management]
        G[Messaging] --> H[Persistence]
        I[Observability] --> J[Security]
    end
    
    subgraph "External Services"
        K[Consul] --> L[Fabio]
        M[Jaeger] --> N[Redis]
        O[MongoDB] --> P[RabbitMQ]
    end
    
    B --> E
    D --> G
    F --> I
    H --> K
    J --> M
```

## Core Components

### Authentication & Authorization
- **AuthManager**: JWT token creation and management
- **BearerTokenMiddleware**: Bearer token authentication middleware
- **AuthOptions**: Authentication configuration options
- **DisabledAuthenticationPolicyEvaluator**: Policy evaluator for disabled authentication

### Context Management
- **Context**: Request context with correlation tracking
- **ContextFactory**: Factory for creating request contexts
- **IdentityContext**: User identity context management
- **CorrelationContext**: Request correlation tracking

### Messaging Infrastructure
- **MessageBroker**: Unified message broker interface
- **EventProcessor**: Event processing and handling
- **Outbox Decorators**: Command and event handler decorators for outbox pattern
- **Redis Streams**: Redis streaming support

### Persistence
- **MongoDB Integration**: Complete MongoDB support with repositories
- **Redis Integration**: Redis caching and streaming
- **SQL Integration**: Entity Framework Core integration
- **PostgreSQL Integration**: PostgreSQL-specific features

### Observability
- **Jaeger Integration**: Distributed tracing with Jaeger
- **Metrics Collection**: AppMetrics integration
- **Structured Logging**: Comprehensive logging infrastructure
- **Health Checks**: Service health monitoring

### Security
- **Certificate Authentication**: X.509 certificate-based authentication
- **Private Key Service**: Cryptographic key management
- **Security Options**: Security configuration and policies
- **Encryption Utilities**: Data encryption and decryption

## Installation

### NuGet Package
```bash
dotnet add package Mamey.Microservice.Infrastructure
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)
- Mamey.Microservice.Abstractions
- All referenced Mamey libraries

## Key Features

### Core Infrastructure Features

- **Shared Infrastructure**: Common components for all microservices
- **Authentication**: JWT-based authentication with role support
- **Context Management**: Request correlation and identity tracking
- **Messaging**: Message brokers with outbox pattern support
- **Persistence**: Multiple database support (MongoDB, Redis, SQL)
- **Observability**: Comprehensive logging, metrics, and tracing
- **Security**: Certificate authentication and encryption
- **Service Discovery**: Consul integration
- **Load Balancing**: Fabio load balancer support
- **API Documentation**: Swagger/OpenAPI integration

### Advanced Features

- **Outbox Pattern**: Reliable message publishing with outbox pattern
- **Distributed Tracing**: Jaeger integration for distributed tracing
- **Metrics Collection**: AppMetrics integration for performance monitoring
- **Health Checks**: Service health monitoring and reporting
- **Serialization**: Custom JSON serialization with converters
- **Caching**: Redis-based caching with streaming support
- **Event Processing**: Event-driven architecture support
- **Saga Support**: Long-running business process support
- **Multi-Tenant**: Multi-tenant application support
- **Compliance**: Built-in compliance and security features

### Performance Features

- **Connection Pooling**: Database connection pooling
- **Caching**: Multi-level caching strategies
- **Async Processing**: Asynchronous message processing
- **Batch Operations**: Batch database operations
- **Streaming**: Real-time data streaming
- **Load Balancing**: Intelligent load balancing
- **Circuit Breakers**: Fault tolerance patterns
- **Retry Logic**: Automatic retry mechanisms

## Quick Start

### Basic Setup

```csharp
using Mamey;
using Mamey.Microservice.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Create Mamey builder
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Add microservice infrastructure
mameyBuilder.AddMicroserviceSharedInfrastructure();

var app = builder.Build();

// Use shared infrastructure
app.UseSharedInfrastructure();

app.Run();
```

### Configuration

```json
{
  "mongo": {
    "connectionString": "mongodb://localhost:27017",
    "database": "mamey_microservice"
  },
  "redis": {
    "connectionString": "localhost:6379"
  },
  "consul": {
    "enabled": true,
    "url": "http://localhost:8500"
  },
  "jaeger": {
    "enabled": true,
    "agentHost": "localhost",
    "agentPort": 6831
  },
  "auth": {
    "issuerSigningKey": "your-secret-key",
    "issuer": "Mamey.Microservice",
    "expiry": "01:00:00"
  }
}
```

### Basic Usage

```csharp
using Mamey.Microservice.Infrastructure;
using Mamey.CQRS.Commands;

public class CreateUserCommand : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IContext _context;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IContext context, ILogger<CreateUserCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user {Name} with correlation ID {CorrelationId}", 
            command.Name, _context.CorrelationId);

        // Create user logic here
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow
        };

        // Save user to database
        await _userRepository.AddAsync(user);

        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
    }
}
```

### Authentication Usage

```csharp
using Mamey.Microservice.Infrastructure.Auth;

public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validate user credentials
            if (!ValidateUser(request.Username, request.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Create JWT token
            var token = _authManager.CreateToken(
                userId: request.Username,
                role: "User",
                audience: "Mamey.Microservice",
                claims: new Dictionary<string, IEnumerable<string>>
                {
                    ["email"] = new[] { request.Email },
                    ["permissions"] = new[] { "read", "write" }
                });

            _logger.LogInformation("User {Username} logged in successfully", request.Username);

            return Ok(new
            {
                AccessToken = token.AccessToken,
                Expires = token.Expires,
                Role = token.Role
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", request.Username);
            return BadRequest("Login failed");
        }
    }

    private bool ValidateUser(string username, string password)
    {
        // Implement user validation logic
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}
```

### Context Usage

```csharp
using Mamey.Microservice.Infrastructure.Contexts;

public class UserService
{
    private readonly IContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(IContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        _logger.LogInformation("Getting user {UserId} for request {RequestId}", 
            userId, _context.RequestId);

        // Use correlation ID for distributed tracing
        using var activity = ActivitySource.StartActivity("GetUser");
        activity?.SetTag("user.id", userId.ToString());
        activity?.SetTag("correlation.id", _context.CorrelationId.ToString());

        // Get user from database
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for request {RequestId}", 
                userId, _context.RequestId);
            throw new UserNotFoundException($"User {userId} not found");
        }

        _logger.LogInformation("User {UserId} retrieved successfully for request {RequestId}", 
            userId, _context.RequestId);

        return user;
    }
}
```

## API Reference

### Core Interfaces

#### IAuthManager

Interface for JWT token management.

```csharp
public interface IAuthManager
{
    JsonWebToken CreateToken(string userId, string role = null, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null);
}
```

**Methods:**
- `CreateToken(string userId, string role, string audience, IDictionary<string, IEnumerable<string>> claims)`: Creates a JWT token with the specified parameters

#### IContext

Interface for request context management.

```csharp
public interface IContext
{
    string RequestId { get; }
    IIdentityContext Identity { get; }
    OrganizationId? OrganizationHeader { get; }
    Guid CorrelationId { get; }
    string TraceId { get; }
    string IpAddress { get; }
    string UserAgent { get; }
}
```

**Properties:**
- `RequestId`: Unique request identifier
- `Identity`: User identity context
- `OrganizationHeader`: Organization identifier
- `CorrelationId`: Request correlation ID
- `TraceId`: Distributed tracing ID
- `IpAddress`: Client IP address
- `UserAgent`: Client user agent

#### IContextFactory

Interface for creating request contexts.

```csharp
public interface IContextFactory
{
    IContext Create();
    IContext Create(OrganizationId? organizationId);
    IContext Create(CorrelationContext context);
}
```

**Methods:**
- `Create()`: Creates a new context
- `Create(OrganizationId? organizationId)`: Creates a context with organization
- `Create(CorrelationContext context)`: Creates a context from correlation context

### Core Classes

#### AuthManager

JWT token creation and management.

```csharp
public sealed class AuthManager : IAuthManager
{
    public AuthManager(AuthOptions options, IClock clock);
    public JsonWebToken CreateToken(string userId, string role = null, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null);
}
```

**Constructor:**
- `AuthManager(AuthOptions options, IClock clock)`: Creates a new auth manager

**Methods:**
- `CreateToken(string userId, string role, string audience, IDictionary<string, IEnumerable<string>> claims)`: Creates a JWT token

#### Context

Request context implementation.

```csharp
internal sealed class Context : IContext
{
    public string RequestId { get; }
    public IIdentityContext Identity { get; }
    public OrganizationId? OrganizationHeader { get; }
    public Guid CorrelationId { get; }
    public string TraceId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
}
```

**Properties:**
- `RequestId`: Unique request identifier
- `Identity`: User identity context
- `OrganizationHeader`: Organization identifier
- `CorrelationId`: Request correlation ID
- `TraceId`: Distributed tracing ID
- `IpAddress`: Client IP address
- `UserAgent`: Client user agent

#### AuthOptions

Authentication configuration options.

```csharp
public class AuthOptions
{
    public string IssuerSigningKey { get; set; }
    public string Issuer { get; set; }
    public TimeSpan? Expiry { get; set; }
}
```

**Properties:**
- `IssuerSigningKey`: JWT signing key
- `Issuer`: JWT issuer
- `Expiry`: Token expiry time

## Usage Examples

### Example 1: Complete Microservice Setup

```csharp
using Mamey;
using Mamey.Microservice.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Create Mamey builder
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Add microservice infrastructure
mameyBuilder.AddMicroserviceSharedInfrastructure();

var app = builder.Build();

// Use shared infrastructure
app.UseSharedInfrastructure();

app.Run();
```

### Example 2: Custom Authentication

```csharp
using Mamey.Microservice.Infrastructure.Auth;

public class CustomAuthService
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<CustomAuthService> _logger;

    public CustomAuthService(IAuthManager authManager, ILogger<CustomAuthService> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    public async Task<JsonWebToken> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Validate credentials
            var user = await ValidateCredentialsAsync(username, password);
            if (user == null)
            {
                throw new AuthenticationException("Invalid credentials");
            }

            // Create token with custom claims
            var claims = new Dictionary<string, IEnumerable<string>>
            {
                ["email"] = new[] { user.Email },
                ["permissions"] = user.Permissions,
                ["tenant"] = new[] { user.TenantId.ToString() }
            };

            var token = _authManager.CreateToken(
                userId: user.Id.ToString(),
                role: user.Role,
                audience: "Mamey.Microservice",
                claims: claims);

            _logger.LogInformation("User {Username} authenticated successfully", username);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication failed for user {Username}", username);
            throw;
        }
    }

    private async Task<User> ValidateCredentialsAsync(string username, string password)
    {
        // Implement credential validation
        return await _userRepository.GetByUsernameAsync(username);
    }
}
```

### Example 3: Context-Aware Service

```csharp
using Mamey.Microservice.Infrastructure.Contexts;

public class OrderService
{
    private readonly IContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly IMessageBroker _messageBroker;

    public OrderService(IContext context, ILogger<OrderService> logger, IMessageBroker messageBroker)
    {
        _context = context;
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        _logger.LogInformation("Creating order for user {UserId} with correlation ID {CorrelationId}", 
            _context.Identity.UserId, _context.CorrelationId);

        // Create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _context.Identity.UserId,
            Items = request.Items,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = _context.CorrelationId
        };

        // Save order
        await _orderRepository.AddAsync(order);

        // Publish order created event
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            CorrelationId = _context.CorrelationId
        };

        await _messageBroker.PublishAsync(orderCreatedEvent);

        _logger.LogInformation("Order {OrderId} created successfully with correlation ID {CorrelationId}", 
            order.Id, _context.CorrelationId);

        return order;
    }
}
```

### Example 4: MongoDB Integration

```csharp
using Mamey.Microservice.Infrastructure.Mongo;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoSessionFactory sessionFactory) : base(sessionFactory)
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetByRoleAsync(string role)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Role, role);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize)
    {
        var filter = Builders<User>.Filter.Empty;
        var totalCount = await Collection.CountDocumentsAsync(filter);
        
        var users = await Collection
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = users,
            TotalCount = (int)totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
```

## Integration Patterns

### Integration with ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);
mameyBuilder.AddMicroserviceSharedInfrastructure();

var app = builder.Build();
app.UseSharedInfrastructure();

app.Run();
```

### Integration with CQRS

```csharp
using Mamey.CQRS.Commands;
using Mamey.Microservice.Infrastructure;

public class CreateProductCommand : ICommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IContext _context;
    private readonly IProductRepository _productRepository;
    private readonly IMessageBroker _messageBroker;

    public CreateProductCommandHandler(
        IContext context,
        IProductRepository productRepository,
        IMessageBroker messageBroker)
    {
        _context = context;
        _productRepository = productRepository;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Create product
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Price = command.Price,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = _context.CorrelationId
        };

        await _productRepository.AddAsync(product);

        // Publish event
        var productCreatedEvent = new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            CorrelationId = _context.CorrelationId
        };

        await _messageBroker.PublishAsync(productCreatedEvent);
    }
}
```

## Configuration Reference

### Service Registration

```csharp
// Add microservice infrastructure
mameyBuilder.AddMicroserviceSharedInfrastructure();

// Add saga infrastructure
mameyBuilder.AddSagaInfrastructure();
```

### Configuration Options

```json
{
  "mongo": {
    "connectionString": "mongodb://localhost:27017",
    "database": "mamey_microservice"
  },
  "redis": {
    "connectionString": "localhost:6379"
  },
  "consul": {
    "enabled": true,
    "url": "http://localhost:8500"
  },
  "jaeger": {
    "enabled": true,
    "agentHost": "localhost",
    "agentPort": 6831
  },
  "auth": {
    "issuerSigningKey": "your-secret-key",
    "issuer": "Mamey.Microservice",
    "expiry": "01:00:00"
  },
  "rabbitmq": {
    "connectionString": "amqp://guest:guest@localhost:5672"
  }
}
```

### Environment Variables

```bash
# MongoDB Configuration
MONGO__CONNECTIONSTRING=mongodb://localhost:27017
MONGO__DATABASE=mamey_microservice

# Redis Configuration
REDIS__CONNECTIONSTRING=localhost:6379

# Consul Configuration
CONSUL__ENABLED=true
CONSUL__URL=http://localhost:8500

# Jaeger Configuration
JAEGER__ENABLED=true
JAEGER__AGENTHOST=localhost
JAEGER__AGENTPORT=6831

# Auth Configuration
AUTH__ISSUERSIGNINGKEY=your-secret-key
AUTH__ISSUER=Mamey.Microservice
AUTH__EXPIRY=01:00:00
```

## Best Practices

1. **Context Usage**: Always use IContext for correlation tracking
2. **Authentication**: Implement proper JWT token validation
3. **Logging**: Use structured logging with correlation IDs
4. **Error Handling**: Implement comprehensive error handling
5. **Configuration**: Use strongly typed configuration classes
6. **Security**: Implement proper security measures
7. **Performance**: Use caching and connection pooling
8. **Monitoring**: Implement health checks and metrics
9. **Testing**: Write comprehensive unit and integration tests
10. **Documentation**: Document all public APIs and configurations

## Troubleshooting

### Common Issues

**Authentication Failures**: Check JWT configuration and token validation
**Context Issues**: Verify context factory registration
**Database Issues**: Check connection strings and database availability
**Messaging Issues**: Verify message broker configuration
**Tracing Issues**: Check Jaeger configuration and connectivity

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Related Libraries

- [Mamey.Microservice.Abstractions](microservice-abstractions.md) - Microservice abstractions
- [Mamey.CQRS.Commands](../cqrs/cqrs-commands.md) - Command pattern implementation
- [Mamey.CQRS.Events](../cqrs/cqrs-events.md) - Event handling and dispatching
- [Mamey.MessageBrokers](../messaging/messagebrokers.md) - Message broker abstractions
- [Mamey.Persistence.MongoDB](../persistence/persistence-mongodb.md) - MongoDB persistence
- [Mamey.Tracing.Jaeger](../observability/tracing-jaeger.md) - Jaeger tracing integration

## Additional Resources

- Microservice Architecture Guide
- Authentication Guide
- Distributed Tracing Guide
- Message Brokers Guide
