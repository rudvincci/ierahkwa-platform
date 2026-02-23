# Quick Start Tutorial

This tutorial will guide you through creating your first microservice using the Mamey Framework.

## What You'll Build

A simple user management microservice that demonstrates:
- CQRS pattern with commands and queries
- Event handling
- Message brokering
- Authentication
- API endpoints

## Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Docker (for RabbitMQ and MongoDB)

## Step 1: Create a New Project

```bash
# Create new Web API project
dotnet new webapi -n UserService
cd UserService

# Add Mamey packages
dotnet add package Mamey
dotnet add package Mamey.CQRS.Commands
dotnet add package Mamey.CQRS.Events
dotnet add package Mamey.CQRS.Queries
dotnet add package Mamey.Microservice.Infrastructure
dotnet add package Mamey.MessageBrokers.RabbitMQ
dotnet add package Mamey.Persistence.MongoDB
```

## Step 2: Define Domain Models

Create `Models/User.cs`:

```csharp
using Mamey.Types;

namespace UserService.Models;

public class User : AggregateRoot<UserId>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { } // For EF Core

    public User(UserId id, string name, string email) : base(id)
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new UserCreatedEvent(Id, Name, Email, CreatedAt));
    }

    public void UpdateName(string newName)
    {
        Name = newName;
        AddEvent(new UserNameUpdatedEvent(Id, newName));
    }
}

public record UserId(Guid Value) : EntityId<Guid>(Value);
```

## Step 3: Create Commands and Queries

Create `Commands/CreateUserCommand.cs`:

```csharp
using Mamey.CQRS.Commands;

namespace UserService.Commands;

public record CreateUserCommand(string Name, string Email) : ICommand;
```

Create `Commands/UpdateUserNameCommand.cs`:

```csharp
using Mamey.CQRS.Commands;

namespace UserService.Commands;

public record UpdateUserNameCommand(UserId UserId, string NewName) : ICommand;
```

Create `Queries/GetUserQuery.cs`:

```csharp
using Mamey.CQRS.Queries;

namespace UserService.Queries;

public record GetUserQuery(UserId UserId) : IQuery<User>;
```

Create `Queries/GetAllUsersQuery.cs`:

```csharp
using Mamey.CQRS.Queries;

namespace UserService.Queries;

public record GetAllUsersQuery() : IQuery<IEnumerable<User>>;
```

## Step 4: Create Events

Create `Events/UserCreatedEvent.cs`:

```csharp
using Mamey.CQRS.Events;

namespace UserService.Events;

public record UserCreatedEvent(UserId UserId, string Name, string Email, DateTime CreatedAt) : IEvent;
```

Create `Events/UserNameUpdatedEvent.cs`:

```csharp
using Mamey.CQRS.Events;

namespace UserService.Events;

public record UserNameUpdatedEvent(UserId UserId, string NewName) : IEvent;
```

## Step 5: Implement Command Handlers

Create `Handlers/CreateUserCommandHandler.cs`:

```csharp
using Mamey.CQRS.Commands;
using UserService.Commands;
using UserService.Models;

namespace UserService.Handlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(Guid.NewGuid());
        var user = new User(userId, command.Name, command.Email);
        
        await _repository.AddAsync(user);
    }
}
```

Create `Handlers/UpdateUserNameCommandHandler.cs`:

```csharp
using Mamey.CQRS.Commands;
using UserService.Commands;
using UserService.Models;

namespace UserService.Handlers;

public class UpdateUserNameCommandHandler : ICommandHandler<UpdateUserNameCommand>
{
    private readonly IUserRepository _repository;

    public UpdateUserNameCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateUserNameCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(command.UserId);
        if (user == null)
            throw new UserNotFoundException(command.UserId);

        user.UpdateName(command.NewName);
        await _repository.UpdateAsync(user);
    }
}
```

## Step 6: Implement Query Handlers

Create `Handlers/GetUserQueryHandler.cs`:

```csharp
using Mamey.CQRS.Queries;
using UserService.Queries;
using UserService.Models;

namespace UserService.Handlers;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(query.UserId);
        if (user == null)
            throw new UserNotFoundException(query.UserId);

        return user;
    }
}
```

## Step 7: Implement Event Handlers

Create `Handlers/UserCreatedEventHandler.cs`:

```csharp
using Mamey.CQRS.Events;
using UserService.Events;

namespace UserService.Handlers;

public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User created: {UserId} - {Name} ({Email})", 
            @event.UserId, @event.Name, @event.Email);
        
        // Send welcome email, update analytics, etc.
        await Task.CompletedTask;
    }
}
```

## Step 8: Create Repository Interface

Create `Repositories/IUserRepository.cs`:

```csharp
using UserService.Models;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId userId);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId userId);
}
```

## Step 9: Configure the Application

Update `Program.cs`:

```csharp
using Mamey;
using Mamey.Microservice.Infrastructure;
using UserService.Handlers;
using UserService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

mameyBuilder
    .AddMicroserviceSharedInfrastructure()
    .AddCommandHandlers()
    .AddEventHandlers()
    .AddQueryHandlers()
    .AddMongoDB()
    .AddRabbitMq();

// Register custom services
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.UseSharedInfrastructure();

// Add API endpoints
app.MapPost("/users", async (CreateUserCommand command, ICommandDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(command);
    return Results.Created($"/users/{command.Name}", command);
});

app.MapGet("/users/{id:guid}", async (Guid id, IQueryDispatcher dispatcher) =>
{
    var query = new GetUserQuery(new UserId(id));
    var user = await dispatcher.QueryAsync(query);
    return Results.Ok(user);
});

app.MapPut("/users/{id:guid}/name", async (Guid id, string name, ICommandDispatcher dispatcher) =>
{
    var command = new UpdateUserNameCommand(new UserId(id), name);
    await dispatcher.SendAsync(command);
    return Results.NoContent();
});

app.Run();
```

## Step 10: Add Configuration

Update `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Mamey": {
    "MessageBroker": {
      "Type": "RabbitMQ",
      "ConnectionString": "amqp://localhost:5672"
    },
    "Persistence": {
      "MongoDB": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "userservice"
      }
    }
  }
}
```

## Step 11: Run the Application

```bash
# Start dependencies (RabbitMQ and MongoDB)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
docker run -d --name mongodb -p 27017:27017 mongo:latest

# Run the application
dotnet run
```

## Step 12: Test the API

```bash
# Create a user
curl -X POST "https://localhost:7000/users" \
  -H "Content-Type: application/json" \
  -d '{"name": "John Doe", "email": "john@example.com"}'

# Get a user
curl "https://localhost:7000/users/{user-id}"

# Update user name
curl -X PUT "https://localhost:7000/users/{user-id}/name" \
  -H "Content-Type: application/json" \
  -d '"Jane Doe"'
```

## What's Next?

- [Architecture Overview](../guides/architecture.md) - Learn about the framework architecture
- [Core Libraries](../libraries/core/mamey.md) - Explore the core framework
- [CQRS Pattern](../libraries/cqrs/) - Deep dive into CQRS implementation
- [Message Brokers](../libraries/messaging/messagebrokers.md) - Learn about messaging patterns
