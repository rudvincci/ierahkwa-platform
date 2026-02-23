# Mamey.Microservice.Abstractions

The Mamey.Microservice.Abstractions library provides the core abstractions and interfaces for building microservices with the Mamey framework. It defines the contracts and data structures that enable a unified microservice development experience across different implementations and infrastructure providers.

## Technical Overview

Mamey.Microservice.Abstractions serves as the foundation for the Mamey microservice system by providing:

- **Core Interfaces**: Essential interfaces for microservice operations
- **Data Models**: Common data structures for microservice communication
- **Authentication Abstractions**: JWT and authentication interfaces
- **Messaging Abstractions**: Message broker and event processing interfaces
- **Exception Handling**: Standardized exception types
- **Cryptography Abstractions**: Cryptographic operations interfaces
- **Service Abstractions**: User data and service interfaces
- **Streaming Abstractions**: Real-time data streaming interfaces
- **API Abstractions**: Web API and controller abstractions
- **Standards Support**: ISO standards and compliance interfaces

## Architecture

The library provides a clean abstraction layer that enables different microservice implementations:

```mermaid
graph TB
    subgraph "Abstraction Layer"
        A[IAuthManager] --> B[IMessageBroker]
        C[IEventProcessor] --> D[IStreamPublisher]
        E[IUserDataService] --> F[IPrivateKeyService]
    end
    
    subgraph "Implementation Layer"
        G[JWT Implementation] --> H[RabbitMQ Implementation]
        I[Event Processing] --> J[Redis Streaming]
        K[User Service] --> L[Crypto Service]
    end
    
    subgraph "Application Layer"
        M[Controllers] --> N[Services]
        O[Handlers] --> P[Middleware]
    end
    
    A --> G
    B --> H
    C --> I
    D --> J
    E --> K
    F --> L
```

## Core Components

### Authentication Abstractions
- **IAuthManager**: JWT token creation and management
- **JsonWebToken**: JWT token data structure
- **Authentication Interfaces**: Standardized authentication contracts

### Messaging Abstractions
- **IMessageBroker**: Message broker interface
- **IEventProcessor**: Event processing interface
- **IEventMapper**: Event mapping interface
- **ContractAttribute**: Message contract attribute

### Cryptography Abstractions
- **IPrivateKeyService**: Private key management interface
- **IRng**: Random number generation interface
- **PrivateKey**: Private key data structure

### Service Abstractions
- **IUserDataService**: User data service interface
- **Service Interfaces**: Common service contracts

### Streaming Abstractions
- **IStreamPublisher**: Stream publishing interface
- **IStreamSubscriber**: Stream subscription interface

### Exception Handling
- **AppException**: Base application exception
- **Exception Types**: Standardized exception hierarchy

### API Abstractions
- **Controller Attributes**: Custom controller attributes
- **API Extensions**: Web API extension methods

### Standards Support
- **ISO Standards**: ISO 9362 and other standards support
- **BusinessIdentifierCode**: BIC code implementation

## Installation

### NuGet Package
```bash
dotnet add package Mamey.Microservice.Abstractions
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)
- Referenced Mamey libraries

## Key Features

### Core Abstraction Features

- **Unified Interfaces**: Common interfaces for all microservice operations
- **Provider Agnostic**: Works with any microservice implementation
- **Type Safety**: Strongly typed interfaces and data structures
- **Extensibility**: Easy to extend with custom implementations
- **Dependency Injection**: Full dependency injection support
- **Async/Await**: Modern async programming patterns
- **Error Handling**: Comprehensive exception handling
- **Standards Compliance**: Built-in support for industry standards

### Authentication Features

- **JWT Support**: JWT token creation and management
- **Role-Based Access**: Role-based authentication support
- **Claims Support**: Custom claims and permissions
- **Token Validation**: Token validation interfaces
- **Multi-Tenant**: Multi-tenant authentication support

### Messaging Features

- **Message Brokers**: Unified message broker interface
- **Event Processing**: Event-driven architecture support
- **Message Contracts**: Message contract definitions
- **Event Mapping**: Event mapping and transformation
- **Streaming**: Real-time data streaming support

### Cryptography Features

- **Key Management**: Private key management
- **Random Generation**: Cryptographically secure random numbers
- **Encryption**: Data encryption and decryption
- **Security**: Built-in security features

### Service Features

- **User Management**: User data service interfaces
- **Service Discovery**: Service discovery abstractions
- **Health Checks**: Health check interfaces
- **Monitoring**: Monitoring and observability interfaces

## Quick Start

### Basic Setup

```csharp
using Mamey.Microservice.Abstractions;

// Register services
builder.Services.AddScoped<IAuthManager, JwtAuthManager>();
builder.Services.AddScoped<IMessageBroker, RabbitMqMessageBroker>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
```

### Basic Usage

```csharp
using Mamey.Microservice.Abstractions;

public class UserService
{
    private readonly IAuthManager _authManager;
    private readonly IMessageBroker _messageBroker;
    private readonly IUserDataService _userDataService;

    public UserService(
        IAuthManager authManager,
        IMessageBroker messageBroker,
        IUserDataService userDataService)
    {
        _authManager = authManager;
        _messageBroker = messageBroker;
        _userDataService = userDataService;
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request)
    {
        // Create user
        var user = await _userDataService.CreateUserAsync(request);

        // Create JWT token
        var token = _authManager.CreateToken(
            userId: user.Id.ToString(),
            role: user.Role,
            claims: new Dictionary<string, IEnumerable<string>>
            {
                ["email"] = new[] { user.Email },
                ["permissions"] = user.Permissions
            });

        // Publish user created event
        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _messageBroker.PublishAsync(userCreatedEvent);

        return token.AccessToken;
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

#### IMessageBroker

Interface for message broker operations.

```csharp
public interface IMessageBroker
{
    Task PublishAsync<T>(T message) where T : class;
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : class;
}
```

**Methods:**
- `PublishAsync<T>(T message)`: Publishes a message
- `SubscribeAsync<T>(Func<T, Task> handler)`: Subscribes to a message type

#### IEventProcessor

Interface for event processing.

```csharp
public interface IEventProcessor
{
    Task ProcessAsync<T>(T @event) where T : class;
    Task ProcessAsync<T>(T @event, string eventName) where T : class;
}
```

**Methods:**
- `ProcessAsync<T>(T @event)`: Processes an event
- `ProcessAsync<T>(T @event, string eventName)`: Processes an event with a specific name

#### IUserDataService

Interface for user data operations.

```csharp
public interface IUserDataService
{
    Task<User> GetUserAsync(Guid userId);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<User> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task DeleteUserAsync(Guid userId);
}
```

**Methods:**
- `GetUserAsync(Guid userId)`: Gets a user by ID
- `CreateUserAsync(CreateUserRequest request)`: Creates a new user
- `UpdateUserAsync(Guid userId, UpdateUserRequest request)`: Updates a user
- `DeleteUserAsync(Guid userId)`: Deletes a user

#### IPrivateKeyService

Interface for private key management.

```csharp
public interface IPrivateKeyService
{
    PrivateKey GetPrivateKey();
    PrivateKey GeneratePrivateKey();
    bool ValidatePrivateKey(PrivateKey privateKey);
}
```

**Methods:**
- `GetPrivateKey()`: Gets the current private key
- `GeneratePrivateKey()`: Generates a new private key
- `ValidatePrivateKey(PrivateKey privateKey)`: Validates a private key

#### IRng

Interface for random number generation.

```csharp
public interface IRng
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    double NextDouble();
    void NextBytes(byte[] buffer);
}
```

**Methods:**
- `Next()`: Generates a random integer
- `Next(int maxValue)`: Generates a random integer less than maxValue
- `Next(int minValue, int maxValue)`: Generates a random integer in the specified range
- `NextDouble()`: Generates a random double
- `NextBytes(byte[] buffer)`: Fills a byte array with random bytes

### Core Classes

#### JsonWebToken

JWT token data structure.

```csharp
public class JsonWebToken
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long Expires { get; set; }
    public string Id { get; set; }
    public string Role { get; set; }
    public IDictionary<string, IEnumerable<string>> Claims { get; set; }
}
```

**Properties:**
- `AccessToken`: JWT access token
- `RefreshToken`: JWT refresh token
- `Expires`: Token expiration timestamp
- `Id`: User ID
- `Role`: User role
- `Claims`: Custom claims

#### PrivateKey

Private key data structure.

```csharp
public class PrivateKey
{
    public string Key { get; set; }
    public string Algorithm { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
```

**Properties:**
- `Key`: The private key value
- `Algorithm`: Encryption algorithm
- `CreatedAt`: Key creation timestamp
- `ExpiresAt`: Key expiration timestamp

#### AppException

Base application exception.

```csharp
public abstract class AppException : Exception
{
    public virtual string Code { get; }
    protected AppException(string message) : base(message);
}
```

**Properties:**
- `Code`: Exception code

**Constructor:**
- `AppException(string message)`: Creates a new application exception

## Usage Examples

### Example 1: Custom Auth Manager Implementation

```csharp
using Mamey.Microservice.Abstractions;

public class CustomAuthManager : IAuthManager
{
    private readonly ILogger<CustomAuthManager> _logger;
    private readonly IConfiguration _configuration;

    public CustomAuthManager(ILogger<CustomAuthManager> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public JsonWebToken CreateToken(string userId, string role = null, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null)
    {
        try
        {
            _logger.LogInformation("Creating token for user {UserId} with role {Role}", userId, role);

            // Implement custom token creation logic
            var token = CreateJwtToken(userId, role, audience, claims);
            
            _logger.LogInformation("Token created successfully for user {UserId}", userId);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create token for user {UserId}", userId);
            throw;
        }
    }

    private JsonWebToken CreateJwtToken(string userId, string role, string audience, 
        IDictionary<string, IEnumerable<string>> claims)
    {
        // Implement JWT token creation
        return new JsonWebToken
        {
            AccessToken = "jwt-token",
            RefreshToken = "refresh-token",
            Expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds(),
            Id = userId,
            Role = role ?? "User",
            Claims = claims ?? new Dictionary<string, IEnumerable<string>>()
        };
    }
}
```

### Example 2: Custom Message Broker Implementation

```csharp
using Mamey.Microservice.Abstractions;

public class CustomMessageBroker : IMessageBroker
{
    private readonly ILogger<CustomMessageBroker> _logger;
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers = new();

    public CustomMessageBroker(ILogger<CustomMessageBroker> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message) where T : class
    {
        try
        {
            _logger.LogInformation("Publishing message of type {MessageType}", typeof(T).Name);

            // Implement custom message publishing logic
            await PublishMessageAsync(message);
            
            _logger.LogInformation("Message published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        try
        {
            _logger.LogInformation("Subscribing to messages of type {MessageType}", typeof(T).Name);

            // Implement custom message subscription logic
            await SubscribeToMessageAsync<T>(handler);
            
            _logger.LogInformation("Subscribed to messages of type {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to messages of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    private async Task PublishMessageAsync<T>(T message) where T : class
    {
        // Implement message publishing
        await Task.Delay(1);
    }

    private async Task SubscribeToMessageAsync<T>(Func<T, Task> handler) where T : class
    {
        // Implement message subscription
        await Task.Delay(1);
    }
}
```

### Example 3: Custom User Data Service Implementation

```csharp
using Mamey.Microservice.Abstractions;

public class CustomUserDataService : IUserDataService
{
    private readonly ILogger<CustomUserDataService> _logger;
    private readonly IUserRepository _userRepository;

    public CustomUserDataService(ILogger<CustomUserDataService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Getting user {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new UserNotFoundException($"User {userId} not found");
            }

            _logger.LogInformation("User {UserId} retrieved successfully", userId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {UserId}", userId);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            _logger.LogInformation("Creating user with email {Email}", request.Email);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            _logger.LogInformation("User {UserId} created successfully", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}", request.Email);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        try
        {
            _logger.LogInformation("Updating user {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException($"User {userId} not found");
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} updated successfully", userId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Deleting user {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException($"User {userId} not found");
            }

            await _userRepository.DeleteAsync(user);

            _logger.LogInformation("User {UserId} deleted successfully", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {UserId}", userId);
            throw;
        }
    }
}
```

### Example 4: Custom Private Key Service Implementation

```csharp
using Mamey.Microservice.Abstractions;

public class CustomPrivateKeyService : IPrivateKeyService
{
    private readonly ILogger<CustomPrivateKeyService> _logger;
    private readonly IConfiguration _configuration;
    private PrivateKey _currentKey;

    public CustomPrivateKeyService(ILogger<CustomPrivateKeyService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public PrivateKey GetPrivateKey()
    {
        try
        {
            if (_currentKey == null || _currentKey.ExpiresAt <= DateTime.UtcNow)
            {
                _currentKey = GeneratePrivateKey();
            }

            return _currentKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get private key");
            throw;
        }
    }

    public PrivateKey GeneratePrivateKey()
    {
        try
        {
            _logger.LogInformation("Generating new private key");

            var key = new PrivateKey
            {
                Key = GenerateRandomKey(),
                Algorithm = "RSA-2048",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _logger.LogInformation("Private key generated successfully");
            return key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate private key");
            throw;
        }
    }

    public bool ValidatePrivateKey(PrivateKey privateKey)
    {
        try
        {
            if (privateKey == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(privateKey.Key))
            {
                return false;
            }

            if (privateKey.ExpiresAt <= DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate private key");
            return false;
        }
    }

    private string GenerateRandomKey()
    {
        // Implement key generation logic
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
```

## Integration Patterns

### Integration with Dependency Injection

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register microservice abstractions
builder.Services.AddScoped<IAuthManager, JwtAuthManager>();
builder.Services.AddScoped<IMessageBroker, RabbitMqMessageBroker>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddScoped<IPrivateKeyService, PrivateKeyService>();

var app = builder.Build();
app.Run();
```

### Integration with Custom Implementations

```csharp
public class CustomMicroserviceProvider
{
    private readonly IAuthManager _authManager;
    private readonly IMessageBroker _messageBroker;
    private readonly IUserDataService _userDataService;

    public CustomMicroserviceProvider(
        IAuthManager authManager,
        IMessageBroker messageBroker,
        IUserDataService userDataService)
    {
        _authManager = authManager;
        _messageBroker = messageBroker;
        _userDataService = userDataService;
    }

    public async Task<string> ProcessUserRequestAsync(UserRequest request)
    {
        // Use abstractions to process user request
        var user = await _userDataService.GetUserAsync(request.UserId);
        var token = _authManager.CreateToken(user.Id.ToString(), user.Role);
        
        await _messageBroker.PublishAsync(new UserProcessedEvent
        {
            UserId = user.Id,
            ProcessedAt = DateTime.UtcNow
        });

        return token.AccessToken;
    }
}
```

## Configuration Reference

### Service Registration

```csharp
// Register all microservice abstractions
builder.Services.AddScoped<IAuthManager, JwtAuthManager>();
builder.Services.AddScoped<IMessageBroker, RabbitMqMessageBroker>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddScoped<IPrivateKeyService, PrivateKeyService>();
builder.Services.AddScoped<IRng, Rng>();
```

### Configuration Options

```json
{
  "auth": {
    "issuerSigningKey": "your-secret-key",
    "issuer": "Mamey.Microservice",
    "expiry": "01:00:00"
  },
  "messaging": {
    "broker": "RabbitMQ",
    "connectionString": "amqp://guest:guest@localhost:5672"
  },
  "crypto": {
    "algorithm": "RSA-2048",
    "keyExpiry": "30.00:00:00"
  }
}
```

## Best Practices

1. **Interface Segregation**: Use specific interfaces for specific purposes
2. **Dependency Injection**: Register all services with dependency injection
3. **Async/Await**: Use async patterns consistently
4. **Error Handling**: Implement comprehensive error handling
5. **Logging**: Add detailed logging for debugging and monitoring
6. **Configuration**: Use strongly typed configuration classes
7. **Validation**: Implement proper input validation
8. **Security**: Implement proper security measures
9. **Testing**: Write comprehensive unit tests
10. **Documentation**: Document all public APIs and configurations

## Troubleshooting

### Common Issues

**Interface Not Found**: Ensure all interfaces are properly registered
**Configuration Issues**: Check configuration binding and validation
**Authentication Issues**: Verify authentication configuration
**Messaging Issues**: Check message broker configuration
**Crypto Issues**: Verify cryptographic key configuration

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Related Libraries

- [Mamey.Microservice.Infrastructure](Microservice.Infrastructure/README.md) - Microservice infrastructure implementation
- [Mamey.Auth](Auth/README.md) - Authentication library
- [Mamey.MessageBrokers](MessageBrokers/README.md) - Message broker library
- [Mamey.Security](Security/README.md) - Security utilities

## Additional Resources

- [Microservice Architecture Guide](../guides/microservice-architecture.md)
- [Authentication Guide](../guides/authentication.md)
- [Message Brokers Guide](../guides/message-brokers.md)
- [Security Guide](../guides/security.md)
