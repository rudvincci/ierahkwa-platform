# Mamey Framework Best Practices

This document outlines the best practices for building microservices applications using the Mamey .NET framework. These practices are based on real-world experience and industry standards.

## Table of Contents

- [Overview](#overview)
- [Architecture Best Practices](#architecture-best-practices)
- [CQRS Best Practices](#cqrs-best-practices)
- [Message Broker Best Practices](#message-broker-best-practices)
- [Authentication Best Practices](#authentication-best-practices)
- [Persistence Best Practices](#persistence-best-practices)
- [Observability Best Practices](#observability-best-practices)
- [Security Best Practices](#security-best-practices)
- [Performance Best Practices](#performance-best-practices)
- [Testing Best Practices](#testing-best-practices)
- [Deployment Best Practices](#deployment-best-practices)
- [Code Quality Best Practices](#code-quality-best-practices)

## Overview

The Mamey framework provides a solid foundation for building microservices applications, but following best practices is crucial for success. This guide covers the most important practices across all aspects of microservices development.

## Architecture Best Practices

### 1. Domain-Driven Design (DDD)

**Do:**
- Organize code around business domains
- Use bounded contexts to separate concerns
- Implement aggregate roots for consistency
- Use value objects for immutable data

```csharp
// Good: Domain-driven organization
public class User : AggregateRoot
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public UserName Name { get; private set; }
    
    public void ChangeEmail(Email newEmail)
    {
        if (Email != newEmail)
        {
            Email = newEmail;
            AddDomainEvent(new UserEmailChangedEvent(Id, newEmail));
        }
    }
}
```

**Don't:**
- Mix business logic with infrastructure concerns
- Create anemic domain models
- Ignore domain boundaries

### 2. Service Boundaries

**Do:**
- Design services around business capabilities
- Keep services loosely coupled
- Use asynchronous communication
- Implement circuit breakers

```csharp
// Good: Clear service boundaries
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventDispatcher _eventDispatcher;
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        var user = new User(command.Name, command.Email);
        await _userRepository.CreateAsync(user);
        await _eventDispatcher.PublishAsync(new UserCreatedEvent(user.Id));
        return user;
    }
}
```

**Don't:**
- Create services that are too large or too small
- Use synchronous communication between services
- Share databases between services

### 3. API Design

**Do:**
- Use RESTful conventions
- Implement proper HTTP status codes
- Use consistent naming conventions
- Version your APIs

```csharp
// Good: RESTful API design
[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _queryDispatcher.QueryAsync(new GetUserQuery(id));
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserCommand command)
    {
        var user = await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
}
```

**Don't:**
- Use inconsistent naming conventions
- Ignore HTTP status codes
- Create APIs that are too granular or too coarse

## CQRS Best Practices

### 1. Command Design

**Do:**
- Make commands immutable
- Use descriptive names
- Include all necessary data
- Validate commands before processing

```csharp
// Good: Well-designed command
public record CreateUserCommand(
    string Name,
    string Email,
    string Password
) : ICommand;

public class CreateUserHandler : ICommandHandler<CreateUserCommand>
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        if (string.IsNullOrEmpty(command.Name))
            throw new ArgumentException("Name is required");
            
        if (string.IsNullOrEmpty(command.Email))
            throw new ArgumentException("Email is required");
            
        // Process command
        var user = new User(command.Name, command.Email);
        await _userRepository.CreateAsync(user);
    }
}
```

**Don't:**
- Include unnecessary data in commands
- Use mutable command objects
- Skip validation

### 2. Query Design

**Do:**
- Keep queries focused and specific
- Use projection for read models
- Implement caching where appropriate
- Use pagination for large datasets

```csharp
// Good: Focused query with projection
public record GetUserListQuery(
    int Page,
    int PageSize,
    string SearchTerm = null
) : IQuery<PagedResult<UserListDto>>;

public class GetUserListHandler : IQueryHandler<GetUserListQuery, PagedResult<UserListDto>>
{
    public async Task<PagedResult<UserListDto>> HandleAsync(GetUserListQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.SearchTerm
        );
        
        return users.MapTo<UserListDto>();
    }
}
```

**Don't:**
- Return entire entities when only specific fields are needed
- Skip pagination for large datasets
- Mix read and write concerns

### 3. Event Design

**Do:**
- Make events immutable
- Use past tense for event names
- Include all necessary data
- Keep events focused

```csharp
// Good: Well-designed event
public record UserCreatedEvent(
    int UserId,
    string Name,
    string Email,
    DateTime CreatedAt
) : IEvent;

public class UserCreatedHandler : IEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(@event.Email, @event.Name);
        
        // Update analytics
        await _analyticsService.TrackUserCreatedAsync(@event.UserId);
    }
}
```

**Don't:**
- Include sensitive data in events
- Use present tense for event names
- Create events that are too large

## Message Broker Best Practices

### 1. Message Design

**Do:**
- Use structured message formats
- Include correlation IDs
- Make messages immutable
- Use versioning for message schemas

```csharp
// Good: Well-structured message
public record OrderCreatedEvent(
    int OrderId,
    int CustomerId,
    decimal TotalAmount,
    DateTime CreatedAt,
    string CorrelationId
) : IEvent;

public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent message, IMessageProperties properties, CancellationToken cancellationToken = default)
    {
        // Process order creation
        await _inventoryService.ReserveItemsAsync(message.OrderId);
        await _paymentService.ProcessPaymentAsync(message.OrderId, message.TotalAmount);
    }
}
```

**Don't:**
- Include sensitive data in messages
- Use mutable message objects
- Ignore message versioning

### 2. Error Handling

**Do:**
- Implement dead letter queues
- Use exponential backoff for retries
- Log errors appropriately
- Implement circuit breakers

```csharp
// Good: Error handling with retries
public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly IRetryPolicy _retryPolicy;
    
    public async Task HandleAsync(OrderCreatedEvent message, IMessageProperties properties, CancellationToken cancellationToken = default)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _inventoryService.ReserveItemsAsync(message.OrderId);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process OrderCreatedEvent for Order {OrderId}", message.OrderId);
            throw; // Let the message broker handle retries
        }
    }
}
```

**Don't:**
- Ignore errors
- Use infinite retries
- Skip logging

### 3. Performance

**Do:**
- Use batch processing where appropriate
- Implement message compression
- Use appropriate message sizes
- Monitor message throughput

```csharp
// Good: Batch processing
public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
{
    private readonly List<OrderCreatedEvent> _pendingEvents = new();
    private readonly Timer _batchTimer;
    
    public async Task HandleAsync(OrderCreatedEvent message, IMessageProperties properties, CancellationToken cancellationToken = default)
    {
        _pendingEvents.Add(message);
        
        if (_pendingEvents.Count >= 100)
        {
            await ProcessBatchAsync();
        }
    }
    
    private async Task ProcessBatchAsync()
    {
        var events = _pendingEvents.ToList();
        _pendingEvents.Clear();
        
        await _inventoryService.ReserveItemsBatchAsync(events.Select(e => e.OrderId));
    }
}
```

**Don't:**
- Process messages one by one when batch processing is more efficient
- Use messages that are too large
- Ignore performance monitoring

## Authentication Best Practices

### 1. Token Management

**Do:**
- Use short-lived access tokens
- Implement refresh token rotation
- Store tokens securely
- Implement token revocation

```csharp
// Good: Token management
public class AuthService : IAuthenticationService
{
    public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return AuthenticationResult.Failed("Invalid credentials");
        }
        
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
        
        await _tokenRepository.StoreRefreshTokenAsync(user.Id, refreshToken);
        
        return AuthenticationResult.Success(accessToken, refreshToken);
    }
}
```

**Don't:**
- Use long-lived access tokens
- Store tokens in local storage
- Ignore token revocation

### 2. Password Security

**Do:**
- Use strong password hashing
- Implement password policies
- Use salt for password hashing
- Implement password reset securely

```csharp
// Good: Password security
public class PasswordService
{
    public string HashPassword(string password)
    {
        var salt = _rng.GenerateSalt();
        var hash = _hasher.Hash(password + salt);
        return $"{salt}:{hash}";
    }
    
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2) return false;
        
        var salt = parts[0];
        var hash = parts[1];
        
        return _hasher.Verify(password + salt, hash);
    }
}
```

**Don't:**
- Store passwords in plain text
- Use weak hashing algorithms
- Skip password validation

### 3. Authorization

**Do:**
- Implement role-based access control
- Use claims-based authorization
- Implement resource-based authorization
- Use principle of least privilege

```csharp
// Good: Authorization
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }
}

[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var currentUserId = User.GetUserId();
        if (id != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }
        
        var user = await _userRepository.GetByIdAsync(id);
        return Ok(user);
    }
}
```

**Don't:**
- Implement authorization in the UI only
- Use overly permissive roles
- Skip resource-based authorization

## Persistence Best Practices

### 1. Repository Pattern

**Do:**
- Use generic repositories
- Implement unit of work pattern
- Use async/await consistently
- Implement proper error handling

```csharp
// Good: Repository implementation
public class UserRepository : IUserRepository
{
    private readonly IMongoRepository<User, int> _mongoRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<User> GetByIdAsync(int id)
    {
        try
        {
            return await _mongoRepository.GetAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user with ID {UserId}", id);
            throw;
        }
    }
    
    public async Task<User> CreateAsync(User user)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _mongoRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
```

**Don't:**
- Mix data access logic with business logic
- Use synchronous methods when async is available
- Skip transaction management

### 2. Caching

**Do:**
- Use appropriate cache keys
- Implement cache invalidation
- Use distributed caching for microservices
- Monitor cache performance

```csharp
// Good: Caching implementation
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user:{id}";
        
        // Try memory cache first
        if (_cache.TryGetValue(cacheKey, out User user))
        {
            return user;
        }
        
        // Try distributed cache
        var cachedUser = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedUser != null)
        {
            user = JsonSerializer.Deserialize<User>(cachedUser);
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
            return user;
        }
        
        // Get from database
        user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }
        
        return user;
    }
}
```

**Don't:**
- Cache sensitive data
- Use inconsistent cache keys
- Ignore cache invalidation

### 3. Data Migration

**Do:**
- Use versioned migrations
- Test migrations thoroughly
- Implement rollback strategies
- Use database transactions for migrations

```csharp
// Good: Data migration
public class AddUserEmailIndexMigration : IMigration
{
    public int Version => 1;
    
    public async Task UpAsync(IDatabase database)
    {
        await database.ExecuteAsync("CREATE INDEX IX_Users_Email ON Users (Email)");
    }
    
    public async Task DownAsync(IDatabase database)
    {
        await database.ExecuteAsync("DROP INDEX IX_Users_Email ON Users");
    }
}
```

**Don't:**
- Skip migration testing
- Ignore rollback strategies
- Use non-versioned migrations

## Observability Best Practices

### 1. Logging

**Do:**
- Use structured logging
- Include correlation IDs
- Log at appropriate levels
- Use consistent log formats

```csharp
// Good: Structured logging
public class UserService
{
    private readonly ILogger<UserService> _logger;
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = command.CorrelationId,
            ["UserId"] = command.UserId
        });
        
        _logger.LogInformation("Creating user with email {Email}", command.Email);
        
        try
        {
            var user = new User(command.Name, command.Email);
            await _userRepository.CreateAsync(user);
            
            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}", command.Email);
            throw;
        }
    }
}
```

**Don't:**
- Log sensitive data
- Use inconsistent log formats
- Log at inappropriate levels

### 2. Tracing

**Do:**
- Use distributed tracing
- Include span tags
- Implement trace sampling
- Use consistent span names

```csharp
// Good: Distributed tracing
public class UserService
{
    private readonly ITracer _tracer;
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        using var span = _tracer.BuildSpan("user.create")
            .WithTag("user.email", command.Email)
            .WithTag("user.name", command.Name)
            .StartActive();
            
        try
        {
            var user = new User(command.Name, command.Email);
            await _userRepository.CreateAsync(user);
            
            span.Span.SetTag("user.id", user.Id.ToString());
            span.Span.SetTag("success", true);
            
            return user;
        }
        catch (Exception ex)
        {
            span.Span.SetTag("error", true);
            span.Span.SetTag("error.message", ex.Message);
            throw;
        }
    }
}
```

**Don't:**
- Skip tracing for critical operations
- Use inconsistent span names
- Ignore trace sampling

### 3. Metrics

**Do:**
- Use appropriate metric types
- Include business metrics
- Monitor key performance indicators
- Use consistent metric names

```csharp
// Good: Metrics implementation
public class UserService
{
    private readonly IMetrics _metrics;
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        using var timer = _metrics.Measure.Timer.Time("user.create.duration");
        
        try
        {
            var user = new User(command.Name, command.Email);
            await _userRepository.CreateAsync(user);
            
            _metrics.Measure.Counter.Increment("user.created.total");
            _metrics.Measure.Gauge.SetValue("user.count", await _userRepository.GetCountAsync());
            
            return user;
        }
        catch (Exception ex)
        {
            _metrics.Measure.Counter.Increment("user.create.errors.total");
            throw;
        }
    }
}
```

**Don't:**
- Use metrics for debugging
- Ignore business metrics
- Use inconsistent metric names

## Security Best Practices

### 1. Data Encryption

**Do:**
- Encrypt sensitive data at rest
- Use strong encryption algorithms
- Implement key rotation
- Use secure key storage

```csharp
// Good: Data encryption
public class UserService
{
    private readonly IEncryptor _encryptor;
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        var user = new User
        {
            Name = command.Name,
            Email = _encryptor.Encrypt(command.Email), // Encrypt sensitive data
            Phone = _encryptor.Encrypt(command.Phone),
            CreatedAt = DateTime.UtcNow
        };
        
        await _userRepository.CreateAsync(user);
        return user;
    }
    
    public async Task<User> GetUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            user.Email = _encryptor.Decrypt(user.Email); // Decrypt when needed
            user.Phone = _encryptor.Decrypt(user.Phone);
        }
        return user;
    }
}
```

**Don't:**
- Store sensitive data in plain text
- Use weak encryption algorithms
- Ignore key rotation

### 2. Input Validation

**Do:**
- Validate all input
- Use whitelist validation
- Implement rate limiting
- Sanitize input data

```csharp
// Good: Input validation
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100)
            .Matches("^[a-zA-Z\\s]+$");
            
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]");
    }
}
```

**Don't:**
- Skip input validation
- Use blacklist validation
- Ignore rate limiting

### 3. API Security

**Do:**
- Use HTTPS everywhere
- Implement proper authentication
- Use rate limiting
- Implement CORS properly

```csharp
// Good: API security
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddMamey()
            .AddAuth()
            .AddJwt();
            
        var app = builder.Build();
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiting();
        app.UseCors("AllowSpecificOrigin");
        
        app.Run();
    }
}
```

**Don't:**
- Use HTTP in production
- Skip authentication
- Ignore rate limiting

## Performance Best Practices

### 1. Async/Await

**Do:**
- Use async/await consistently
- Avoid blocking calls
- Use ConfigureAwait(false) in libraries
- Implement proper cancellation

```csharp
// Good: Async/await usage
public class UserService
{
    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return null;
        }
        
        // Load related data asynchronously
        var orders = await _orderRepository.GetByUserIdAsync(id, cancellationToken);
        user.Orders = orders;
        
        return user;
    }
}
```

**Don't:**
- Mix async and synchronous code
- Use blocking calls in async methods
- Ignore cancellation tokens

### 2. Caching

**Do:**
- Cache frequently accessed data
- Use appropriate cache expiration
- Implement cache invalidation
- Monitor cache performance

```csharp
// Good: Caching implementation
public class UserService
{
    private readonly IMemoryCache _cache;
    private readonly IUserRepository _userRepository;
    
    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user:{id}";
        
        if (_cache.TryGetValue(cacheKey, out User user))
        {
            return user;
        }
        
        user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        }
        
        return user;
    }
}
```

**Don't:**
- Cache data that changes frequently
- Use infinite cache expiration
- Ignore cache invalidation

### 3. Database Optimization

**Do:**
- Use appropriate indexes
- Implement query optimization
- Use connection pooling
- Monitor database performance

```csharp
// Good: Database optimization
public class UserRepository
{
    public async Task<User> GetUserByEmailAsync(string email)
    {
        // Use indexed field for query
        return await _context.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
    {
        // Use pagination
        return await _context.Users
            .OrderBy(u => u.CreatedAt)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
```

**Don't:**
- Use SELECT * queries
- Skip pagination for large datasets
- Ignore database indexing

## Testing Best Practices

### 1. Unit Testing

**Do:**
- Test business logic thoroughly
- Use mocks for dependencies
- Test edge cases
- Maintain high test coverage

```csharp
// Good: Unit testing
[Test]
public async Task CreateUser_WithValidData_ShouldCreateUser()
{
    // Arrange
    var command = new CreateUserCommand("John Doe", "john@example.com");
    var mockRepository = new Mock<IUserRepository>();
    var mockEventDispatcher = new Mock<IEventDispatcher>();
    var handler = new CreateUserHandler(mockRepository.Object, mockEventDispatcher.Object);
    
    // Act
    await handler.HandleAsync(command);
    
    // Assert
    mockRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    mockEventDispatcher.Verify(e => e.PublishAsync(It.IsAny<UserCreatedEvent>()), Times.Once);
}
```

**Don't:**
- Test implementation details
- Use real dependencies in unit tests
- Skip edge case testing

### 2. Integration Testing

**Do:**
- Test complete workflows
- Use test containers
- Test with real dependencies
- Clean up test data

```csharp
// Good: Integration testing
[Test]
public async Task CreateUser_EndToEnd_ShouldWork()
{
    // Arrange
    var client = _factory.CreateClient();
    var command = new CreateUserCommand("John Doe", "john@example.com");
    
    // Act
    var response = await client.PostAsJsonAsync("/api/users", command);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var user = await response.Content.ReadFromJsonAsync<UserDto>();
    user.Name.Should().Be("John Doe");
    user.Email.Should().Be("john@example.com");
}
```

**Don't:**
- Skip integration testing
- Use production data in tests
- Ignore test cleanup

### 3. End-to-End Testing

**Do:**
- Test complete user workflows
- Use realistic test data
- Test error scenarios
- Automate test execution

```csharp
// Good: End-to-end testing
[Test]
public async Task UserRegistration_CompleteWorkflow_ShouldWork()
{
    // Arrange
    var client = _factory.CreateClient();
    var registrationData = new UserRegistrationDto
    {
        Name = "John Doe",
        Email = "john@example.com",
        Password = "SecurePassword123!"
    };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/register", registrationData);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    
    // Verify user can login
    var loginData = new LoginDto
    {
        Email = "john@example.com",
        Password = "SecurePassword123!"
    };
    
    var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginData);
    loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

**Don't:**
- Skip end-to-end testing
- Use unrealistic test data
- Ignore error scenario testing

## Deployment Best Practices

### 1. Containerization

**Do:**
- Use multi-stage builds
- Minimize image size
- Use specific base images
- Implement health checks

```dockerfile
# Good: Multi-stage Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyService/MyService.csproj", "MyService/"]
RUN dotnet restore "MyService/MyService.csproj"
COPY . .
WORKDIR "/src/MyService"
RUN dotnet build "MyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyService.dll"]
```

**Don't:**
- Use latest tags
- Include unnecessary files
- Skip health checks

### 2. Configuration Management

**Do:**
- Use environment-specific configurations
- Use secrets management
- Implement configuration validation
- Use configuration providers

```csharp
// Good: Configuration management
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .AddAzureKeyVault();
            
        var app = builder.Build();
        app.Run();
    }
}
```

**Don't:**
- Hardcode configuration values
- Store secrets in configuration files
- Skip configuration validation

### 3. Monitoring and Alerting

**Do:**
- Implement comprehensive monitoring
- Set up appropriate alerts
- Monitor key performance indicators
- Use centralized logging

```csharp
// Good: Monitoring setup
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddMamey()
            .AddLogging()
            .AddTracing()
            .AddMetrics()
            .AddHealthChecks();
            
        var app = builder.Build();
        
        app.UseHealthChecks("/health");
        app.UseMamey();
        
        app.Run();
    }
}
```

**Don't:**
- Skip monitoring setup
- Ignore alerting
- Monitor only technical metrics

## Code Quality Best Practices

### 1. Code Organization

**Do:**
- Use consistent naming conventions
- Organize code by feature
- Use meaningful names
- Keep methods small and focused

```csharp
// Good: Code organization
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<UserService> _logger;
    
    public UserService(
        IUserRepository userRepository,
        IEventDispatcher eventDispatcher,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }
    
    public async Task<User> CreateUserAsync(CreateUserCommand command)
    {
        _logger.LogInformation("Creating user with email {Email}", command.Email);
        
        var user = new User(command.Name, command.Email);
        await _userRepository.CreateAsync(user);
        
        await _eventDispatcher.PublishAsync(new UserCreatedEvent(user.Id, user.Name, user.Email));
        
        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        return user;
    }
}
```

**Don't:**
- Use inconsistent naming
- Create large classes
- Use unclear names

### 2. Error Handling

**Do:**
- Use specific exception types
- Implement proper error logging
- Return meaningful error messages
- Use global exception handling

```csharp
// Good: Error handling
public class UserService
{
    public async Task<User> GetUserAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException($"User with ID {id} not found");
            }
            return user;
        }
        catch (UserNotFoundException)
        {
            throw; // Re-throw known exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user with ID {UserId}", id);
            throw new UserServiceException("An error occurred while retrieving the user", ex);
        }
    }
}
```

**Don't:**
- Catch generic exceptions
- Ignore errors
- Return unclear error messages

### 3. Documentation

**Do:**
- Document public APIs
- Use XML documentation
- Keep documentation up to date
- Include examples

```csharp
// Good: Documentation
/// <summary>
/// Creates a new user in the system.
/// </summary>
/// <param name="command">The command containing user creation data.</param>
/// <returns>The created user.</returns>
/// <exception cref="ArgumentException">Thrown when the command data is invalid.</exception>
/// <exception cref="UserServiceException">Thrown when an error occurs during user creation.</exception>
/// <example>
/// <code>
/// var command = new CreateUserCommand("John Doe", "john@example.com");
/// var user = await userService.CreateUserAsync(command);
/// </code>
/// </example>
public async Task<User> CreateUserAsync(CreateUserCommand command)
{
    // Implementation
}
```

**Don't:**
- Skip documentation
- Use outdated documentation
- Document implementation details

## Conclusion

Following these best practices will help you build robust, maintainable, and scalable microservices applications using the Mamey framework. Remember that best practices are guidelines, not rigid rules, and should be adapted to your specific context and requirements.

Key takeaways:

1. **Architecture**: Use DDD principles and maintain clear service boundaries
2. **CQRS**: Design commands, queries, and events carefully
3. **Message Brokers**: Implement proper error handling and performance optimization
4. **Authentication**: Use secure token management and proper authorization
5. **Persistence**: Implement proper repository patterns and caching
6. **Observability**: Use structured logging, tracing, and metrics
7. **Security**: Encrypt sensitive data and validate all input
8. **Performance**: Use async/await and implement proper caching
9. **Testing**: Write comprehensive unit, integration, and end-to-end tests
10. **Deployment**: Use containerization and proper configuration management
11. **Code Quality**: Maintain clean, well-documented, and organized code

By following these practices, you'll be well on your way to building production-ready microservices applications with the Mamey framework.
