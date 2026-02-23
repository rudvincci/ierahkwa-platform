# Mamey Framework - Master Documentation

**Version**: 2.0.*  
**Organization**: Mamey Technologies (mamey.io)  
**Purpose**: Comprehensive master documentation for the Mamey Framework ecosystem  
**Last Updated**: 2024

## Table of Contents

1. [Framework Overview](#framework-overview)
2. [Architectural Principles](#architectural-principles)
3. [Core Concepts](#core-concepts)
4. [Library Ecosystem](#library-ecosystem)
5. [Complete Examples](#complete-examples)
6. [Integration Patterns](#integration-patterns)
7. [Best Practices](#best-practices)
8. [Architecture Patterns](#architecture-patterns)
9. [Deployment Guide](#deployment-guide)
10. [Reference](#reference)

---

## Framework Overview

### What is Mamey?

Mamey is a comprehensive .NET microservices and distributed systems framework designed for enterprise-grade applications. It consists of **112+ independent helper libraries** that work together to address common infrastructural challenges in building scalable, resilient, and maintainable microservices.

### Key Characteristics

- **Cloud-Agnostic**: Uses CNCF tools that work across cloud providers
- **Modular**: Libraries are independent and can be used separately
- **Type-Safe**: Strongly-typed identifiers prevent primitive obsession
- **Event-Driven**: Built for asynchronous, event-driven architectures
- **Domain-Focused**: Emphasizes Domain-Driven Design patterns
- **Production-Ready**: Designed with operational concerns as first-class citizens

### Design Philosophy

Mamey consists of **helper libraries** that are generally **independent from one another**. It is **not a framework nor a universal solution**. Rather, it includes a **collection of extension methods and additional abstractions** to address common infrastructural challenges like:

- Routing and API management
- Service discovery and load balancing
- Distributed tracing and observability
- Asynchronous messaging
- Persistence and data access
- Authentication and authorization
- Security and compliance

---

## Architectural Principles

### 1. Modularity

Each library is independent and can be used standalone or in combination with others:

```csharp
// Use only what you need
builder.Services
    .AddMamey()
    .AddWebApi()          // Web API only
    .AddMongo()           // MongoDB only
    .AddRabbitMQ();       // RabbitMQ only
```

### 2. Domain-Driven Design (DDD)

Mamey embraces DDD principles with strongly-typed identifiers, value objects, and aggregate roots:

```csharp
// Strongly-typed identifier
public record OrderId(Guid Value) : EntityId<Guid>(Value);

// Aggregate root with domain events
public class Order : AggregateRoot<OrderId>
{
    public decimal TotalAmount { get; private set; }
    
    public void Confirm()
    {
        // Business logic
        Status = OrderStatus.Confirmed;
        AddEvent(new OrderConfirmedEvent(Id));
    }
}
```

### 3. CQRS (Command Query Responsibility Segregation)

Separation of read and write operations for better scalability:

```csharp
// Write side - Commands
public class CreateOrderCommand : ICommand
{
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
}

// Read side - Queries
public class GetOrderQuery : IQuery<OrderDto>
{
    public Guid OrderId { get; set; }
}
```

### 4. Event-Driven Architecture

Asynchronous communication through events:

```csharp
// Domain event
public record OrderCreatedEvent(OrderId OrderId, decimal Amount) : IDomainEvent;

// Event handler
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        // Handle event asynchronously
    }
}
```

### 5. Type Safety

Strongly-typed identifiers prevent primitive obsession:

```csharp
// ✅ Good: Type-safe
public record UserId(Guid Value) : EntityId<Guid>(Value);
public record OrderId(Guid Value) : EntityId<Guid>(Value);

var userId = new UserId(Guid.NewGuid());
var orderId = new OrderId(Guid.NewGuid());

// This won't compile - prevents bugs
// var order = GetOrder(userId); // Error!

// ❌ Bad: Primitive obsession
public Guid UserId { get; set; }
public Guid OrderId { get; set; }
// Easy to mix up: GetOrder(userId) where userId is actually an order ID
```

---

## Core Concepts

### Aggregate Roots

Aggregate roots are consistency boundaries that enforce business invariants:

```csharp
public class Order : AggregateRoot<OrderId>
{
    public string CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    
    // Private constructor for ORM
    private Order() { }
    
    // Public constructor with validation
    public Order(OrderId id, string customerId, decimal totalAmount) : base(id)
    {
        if (totalAmount <= 0)
            throw new ArgumentException("Amount must be positive");
            
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
        
        // Validate aggregate state
        Validate();
        
        // Add domain event
        AddEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount));
    }
    
    // Business operation
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");
            
        Status = OrderStatus.Confirmed;
        AddEvent(new OrderConfirmedEvent(Id));
    }
}
```

### Commands

Commands represent write operations that change system state:

```csharp
public record CreateOrderCommand(string CustomerId, decimal Amount) : ICommand;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly IEventDispatcher _eventDispatcher;
    
    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = new Order(
            new OrderId(Guid.NewGuid()),
            command.CustomerId,
            command.Amount
        );
        
        await _repository.AddAsync(order);
        
        // Publish domain events
        foreach (var domainEvent in order.Events)
        {
            await _eventDispatcher.PublishAsync(domainEvent);
        }
        
        order.ClearEvents();
    }
}
```

### Queries

Queries represent read operations optimized for performance:

```csharp
public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IOrderReadRepository _readRepository;
    
    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _readRepository.GetByIdAsync(query.OrderId);
        if (order == null)
            throw new OrderNotFoundException(query.OrderId);
            
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Amount = order.Amount,
            Status = order.Status
        };
    }
}
```

### Events

Events represent significant business occurrences:

```csharp
public record OrderCreatedEvent(OrderId OrderId, string CustomerId, decimal Amount) : IDomainEvent;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IOrderReadRepository _readRepository;
    private readonly INotificationService _notificationService;
    
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Update read model
        var readModel = new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            Amount = @event.Amount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        
        await _readRepository.AddAsync(readModel);
        
        // Send notification
        await _notificationService.SendOrderConfirmationAsync(@event.CustomerId, @event.OrderId);
    }
}
```

---

## Library Ecosystem

### Core Framework Libraries

#### Mamey (Core)
Foundation library providing:
- `AggregateRoot<T>` base class
- Strongly-typed identifiers (`EntityId<T>`, `AggregateId<T>`)
- Extension methods
- Configuration management
- Domain event infrastructure

```csharp
builder.Services.AddMamey();
```

#### CQRS Libraries
- **Mamey.CQRS.Commands**: Command pattern implementation
- **Mamey.CQRS.Queries**: Query pattern implementation
- **Mamey.CQRS.Events**: Event handling and dispatching

```csharp
builder.Services
    .AddMamey()
    .AddCommands()
    .AddQueries()
    .AddEvents();
```

### Persistence Libraries

#### MongoDB
Document database for flexible schemas:

```csharp
builder.Services
    .AddMamey()
    .AddMongo(options =>
    {
        options.ConnectionString = "mongodb://localhost:27017";
        options.Database = "mydb";
    });
```

#### PostgreSQL
Relational database for ACID compliance:

```csharp
builder.Services
    .AddMamey()
    .AddPostgres<ApplicationDbContext>(options =>
    {
        options.ConnectionString = "Host=localhost;Database=mydb;...";
    });
```

#### Redis
In-memory cache and session storage:

```csharp
builder.Services
    .AddMamey()
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
    });
```

### Messaging Libraries

#### RabbitMQ
Message broker for reliable messaging:

```csharp
builder.Services
    .AddMamey()
    .AddRabbitMQ(options =>
    {
        options.HostNames = new[] { "localhost" };
        options.Port = 5672;
    });
```

#### Outbox Pattern
Reliable message publishing:

```csharp
builder.Services
    .AddMamey()
    .AddRabbitMQ()
    .AddMessageOutbox()
    .AddMongo(); // Outbox storage
```

### Authentication Libraries

#### JWT Authentication
Token-based authentication:

```csharp
builder.Services
    .AddMamey()
    .AddJwt(options =>
    {
        options.IssuerSigningKey = "your-secret-key";
        options.ValidIssuer = "mamey-app";
        options.ValidAudience = "mamey-users";
        options.ExpiryMinutes = 60;
    });
```

#### Azure AD Authentication
Azure Active Directory integration:

```csharp
builder.Services
    .AddMamey()
    .AddAzureAuthentication(options =>
    {
        options.TenantId = "your-tenant-id";
        options.ClientId = "your-client-id";
    });
```

### Web API Libraries

#### WebApi
Minimal API framework:

```csharp
var app = builder.Build();

app.UseWebApi()
    .Post<CreateOrderCommand>("/api/orders", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 201;
    })
    .Get<GetOrderQuery, OrderDto>("/api/orders/{id}", async (query, result, ctx) =>
    {
        await ctx.Response.WriteJsonAsync(result);
    });
```

### Infrastructure Libraries

#### Consul (Service Discovery)
Service registration and discovery:

```csharp
builder.Services
    .AddMamey()
    .AddConsul(options =>
    {
        options.Url = "http://localhost:8500";
        options.Service = "my-service";
    });
```

#### Ntrada (API Gateway)
Configuration-driven API gateway:

```yaml
# ntrada.yml
modules:
- name: orders
  routes:
  - upstream: /api/orders
    method: POST
    use: downstream
    downstream: order-service/api/orders
```

### Observability Libraries

#### Logging
Structured logging with Serilog:

```csharp
builder.Host.UseLogging(options =>
{
    options.Level = "Information";
    options.Console.Enabled = true;
    options.File.Enabled = true;
    options.Seq.Enabled = true;
});
```

#### Tracing (Jaeger)
Distributed tracing:

```csharp
builder.Services
    .AddMamey()
    .AddJaeger(options =>
    {
        options.ServiceName = "my-service";
        options.UdpHost = "localhost";
        options.UdpPort = 6831;
    });
```

#### Metrics (Prometheus)
Metrics collection:

```csharp
builder.Services
    .AddMamey()
    .AddPrometheus();
```

---

## Complete Examples

### Example 1: Complete Microservice Setup

```csharp
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.MessageBrokers.Outbox;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.Redis;
using Mamey.Auth;
using Mamey.Auth.Jwt;
using Mamey.WebApi;
using Mamey.Discovery.Consul;
using Mamey.Logging;
using Mamey.Tracing.Jaeger;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
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
    .AddJwt(options =>
    {
        options.IssuerSigningKey = builder.Configuration["Jwt:SecretKey"];
        options.ValidIssuer = "mamey-app";
        options.ValidAudience = "mamey-users";
        options.ExpiryMinutes = 60;
    })
    .AddWebApi()
    .AddConsul()
    .AddLogging()
    .AddJaeger();

// Configure logging
builder.Host.UseLogging(options =>
{
    options.Level = "Information";
    options.Console.Enabled = true;
    options.File.Enabled = true;
    options.File.Path = "logs/app.log";
});

var app = builder.Build();

// Configure middleware
app.UseMamey()
    .UseWebApi()
    .UseJaeger()
    .UseSharedInfrastructure();

// Configure endpoints
app.UseWebApi()
    .Post<CreateOrderCommand>("/api/orders", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 201;
    })
    .Get<GetOrderQuery, OrderDto>("/api/orders/{id}", async (query, result, ctx) =>
    {
        if (result == null)
        {
            ctx.Response.StatusCode = 404;
            return;
        }
        await ctx.Response.WriteJsonAsync(result);
    });

app.Run();
```

### Example 2: Command Handler with Outbox Pattern

```csharp
using Mamey.CQRS.Commands;
using Mamey.MessageBrokers.Outbox;
using Mamey.Persistence.MongoDB;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IMongoRepository<Order, Guid> _orderRepository;
    private readonly IOutbox _outbox;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IContext _context;

    public CreateOrderCommandHandler(
        IMongoRepository<Order, Guid> orderRepository,
        IOutbox outbox,
        ILogger<CreateOrderCommandHandler> logger,
        IContext context)
    {
        _orderRepository = orderRepository;
        _outbox = outbox;
        _logger = logger;
        _context = context;
    }

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating order for user {UserId} with correlation {CorrelationId}",
            _context.Identity.Id,
            _context.CorrelationId);

        // Create order aggregate
        var orderId = new OrderId(Guid.NewGuid());
        var order = new Order(orderId, command.CustomerId, command.Amount);

        // Save to write database (MongoDB)
        await _orderRepository.AddAsync(order);

        // Store event in outbox for reliable publishing
        var @event = new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            DateTime.UtcNow
        );

        await _outbox.SendAsync(@event);

        _logger.LogInformation("Order created: {OrderId}", order.Id);
    }
}
```

### Example 3: Event Handler with Read Model Update

```csharp
using Mamey.CQRS.Events;
using Mamey.Persistence.MongoDB;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCreatedEvent for order {OrderId}", @event.OrderId);

        // Update read model
        var readModel = new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            Amount = @event.Amount,
            Status = "Pending",
            CreatedAt = @event.OccurredAt
        };

        await _readRepository.AddAsync(readModel);

        _logger.LogInformation("Read model updated for order {OrderId}", @event.OrderId);
    }
}
```

### Example 4: Query Handler with Caching

```csharp
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.Redis;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;
    private readonly ICache _cache;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"order:{query.OrderId}";

        // Try cache first
        var cachedOrder = await _cache.GetAsync<OrderDto>(cacheKey);
        if (cachedOrder != null)
        {
            _logger.LogDebug("Cache hit for order {OrderId}", query.OrderId);
            return cachedOrder;
        }

        // Cache miss - load from database
        var readModel = await _readRepository.GetAsync(query.OrderId, cancellationToken);
        if (readModel == null)
        {
            throw new OrderNotFoundException(query.OrderId);
        }

        var orderDto = new OrderDto
        {
            Id = readModel.Id,
            CustomerId = readModel.CustomerId,
            Amount = readModel.Amount,
            Status = readModel.Status,
            CreatedAt = readModel.CreatedAt
        };

        // Cache for 30 minutes
        await _cache.SetAsync(cacheKey, orderDto, TimeSpan.FromMinutes(30));

        return orderDto;
    }
}
```

### Example 5: Multi-Database Architecture (Write + Read Models)

```csharp
// Write Model (PostgreSQL) - Optimized for writes
public class Order : AggregateRoot<OrderId>
{
    public string CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    
    // Domain logic and business rules
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");
            
        Status = OrderStatus.Confirmed;
        AddEvent(new OrderConfirmedEvent(Id));
    }
}

// Read Model (MongoDB) - Optimized for reads
public class OrderReadModel
{
    [BsonId]
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    
    // Denormalized data for performance
    public string CustomerName { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

// Command Handler - Write to PostgreSQL
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IEFRepository<Order, OrderId> _writeRepository;
    private readonly IOutbox _outbox;

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = new Order(new OrderId(Guid.NewGuid()), command.CustomerId, command.Amount);
        await _writeRepository.AddAsync(order);
        
        // Store event in outbox
        await _outbox.SendAsync(new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount));
    }
}

// Event Handler - Update MongoDB read model
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var readModel = new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            Amount = @event.Amount,
            Status = "Pending",
            CreatedAt = @event.OccurredAt
        };

        await _readRepository.AddAsync(readModel);
    }
}
```

---

## Integration Patterns

### Pattern 1: CQRS with Dual Persistence

**Write Model (PostgreSQL)**:
- ACID transactions
- Domain logic enforcement
- Optimized for writes

**Read Model (MongoDB)**:
- Denormalized data
- Optimized for queries
- Fast reads

**Event Flow**:
1. Command → Write to PostgreSQL
2. Domain Event → Outbox
3. Event Handler → Update MongoDB read model

### Pattern 2: Saga Orchestration

```csharp
public class OrderSaga : ISaga
{
    public Guid Id { get; set; }
    public string State { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid InventoryId { get; set; }

    public async Task Handle(OrderCreatedEvent @event, IMessageHandlerContext context)
    {
        Id = @event.OrderId;
        State = "Processing";
        OrderId = @event.OrderId;

        // Step 1: Reserve inventory
        await context.SendLocal(new ReserveInventoryCommand(@event.OrderId, @event.Items));
    }

    public async Task Handle(InventoryReservedEvent @event, IMessageHandlerContext context)
    {
        State = "InventoryReserved";
        InventoryId = @event.InventoryId;

        // Step 2: Process payment
        await context.SendLocal(new ProcessPaymentCommand(@event.OrderId, @event.Amount));
    }

    public async Task Handle(PaymentProcessedEvent @event, IMessageHandlerContext context)
    {
        State = "PaymentProcessed";
        PaymentId = @event.PaymentId;

        // Step 3: Confirm order
        await context.SendLocal(new ConfirmOrderCommand(@event.OrderId));
        State = "Completed";
    }

    public async Task Handle(InventoryReservationFailedEvent @event, IMessageHandlerContext context)
    {
        State = "Failed";
        await context.SendLocal(new CancelOrderCommand(@event.OrderId));
    }
}
```

### Pattern 3: Outbox Pattern for Reliable Messaging

```csharp
// Command handler stores event in outbox (same transaction)
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IEFRepository<Order, OrderId> _repository;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Save order
            var order = new Order(new OrderId(Guid.NewGuid()), command.CustomerId, command.Amount);
            await _repository.AddAsync(order);

            // Store event in outbox (same transaction)
            await _outbox.SendAsync(new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount));

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}

// Background service publishes outbox messages
public class OutboxPublisher : BackgroundService
{
    private readonly IOutbox _outbox;
    private readonly IBusPublisher _publisher;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await _outbox.GetUnprocessedMessagesAsync();
            
            foreach (var message in messages)
            {
                await _publisher.PublishAsync(message);
                await _outbox.MarkAsProcessedAsync(message.Id);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
```

### Pattern 4: Correlation Tracking Across Services

```csharp
// Request comes in with correlation ID
public class OrderController : ControllerBase
{
    private readonly IContext _context;
    private readonly ICommandDispatcher _commandDispatcher;

    [HttpPost("/api/orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Correlation ID automatically set in context
        var command = new CreateOrderCommand(request.CustomerId, request.Amount);
        await _commandDispatcher.SendAsync(command);

        return Ok(new { OrderId = command.OrderId, CorrelationId = _context.CorrelationId });
    }
}

// Command handler propagates correlation ID
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IContext _context;
    private readonly IBusPublisher _publisher;

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // Correlation ID available in context
        var correlationId = _context.CorrelationId;

        // Publish event with correlation ID
        var @event = new OrderCreatedEvent(command.OrderId, command.CustomerId, command.Amount);
        
        await _publisher.PublishAsync(@event, correlationId: correlationId.ToString());
    }
}

// Event handler receives correlation ID
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IContext _context;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Correlation ID available from context
        _logger.LogInformation(
            "Processing OrderCreatedEvent for order {OrderId} with correlation {CorrelationId}",
            @event.OrderId,
            _context.CorrelationId);

        // Process event...
    }
}
```

### Pattern 5: Complete E-Commerce Microservice Example

This example demonstrates a complete e-commerce order processing microservice using Mamey Framework:

#### Domain Model

```csharp
// Strongly-typed identifiers
public record OrderId(Guid Value) : EntityId<Guid>(Value);
public record CustomerId(Guid Value) : EntityId<Guid>(Value);
public record ProductId(Guid Value) : EntityId<Guid>(Value);

// Domain Events
public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId, decimal TotalAmount, DateTime CreatedAt) : IDomainEvent;
public record OrderConfirmedEvent(OrderId OrderId, DateTime ConfirmedAt) : IDomainEvent;
public record OrderShippedEvent(OrderId OrderId, string TrackingNumber, DateTime ShippedAt) : IDomainEvent;
public record OrderCancelledEvent(OrderId OrderId, string Reason, DateTime CancelledAt) : IDomainEvent;

// Aggregate Root
public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public string? TrackingNumber { get; private set; }
    public string? CancellationReason { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    private Order() { } // For ORM

    public Order(OrderId id, CustomerId customerId, List<OrderItem> items) : base(id)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(items));

        CustomerId = customerId;
        Items = items;
        TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice);
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        Validate();
        AddEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount, CreatedAt));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm order in {Status} status");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        AddEvent(new OrderConfirmedEvent(Id, ConfirmedAt.Value));
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot ship order in {Status} status");

        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number is required", nameof(trackingNumber));

        Status = OrderStatus.Shipped;
        TrackingNumber = trackingNumber;
        ShippedAt = DateTime.UtcNow;
        AddEvent(new OrderShippedEvent(Id, trackingNumber, ShippedAt.Value));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Shipped orders cannot be cancelled");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required", nameof(reason));

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        AddEvent(new OrderCancelledEvent(Id, reason, DateTime.UtcNow));
    }
}

public class OrderItem
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private OrderItem() { } // For ORM

    public OrderItem(ProductId productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
```

#### Commands

```csharp
// Create Order Command
public record CreateOrderCommand(
    CustomerId CustomerId,
    List<CreateOrderItemDto> Items) : ICommand;

public record CreateOrderItemDto(
    ProductId ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

// Confirm Order Command
public record ConfirmOrderCommand(OrderId OrderId) : ICommand;

// Ship Order Command
public record ShipOrderCommand(OrderId OrderId, string TrackingNumber) : ICommand;

// Cancel Order Command
public record CancelOrderCommand(OrderId OrderId, string Reason) : ICommand;

// Command Handler
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IEFRepository<Order, OrderId> _orderRepository;
    private readonly IOutbox _outbox;
    private readonly IContext _context;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IEFRepository<Order, OrderId> orderRepository,
        IOutbox outbox,
        IContext context,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _outbox = outbox;
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating order for customer {CustomerId} with correlation {CorrelationId}",
            command.CustomerId,
            _context.CorrelationId);

        // Create order aggregate
        var orderId = new OrderId(Guid.NewGuid());
        var items = command.Items.Select(i => new OrderItem(
            i.ProductId,
            i.ProductName,
            i.Quantity,
            i.UnitPrice
        )).ToList();

        var order = new Order(orderId, command.CustomerId, items);

        // Save to write database (PostgreSQL)
        await _orderRepository.AddAsync(order);

        // Store domain event in outbox for reliable publishing
        foreach (var domainEvent in order.Events)
        {
            await _outbox.SendAsync(domainEvent);
        }

        order.ClearEvents();

        _logger.LogInformation("Order created: {OrderId} with total {TotalAmount}", order.Id, order.TotalAmount);
    }
}
```

#### Queries

```csharp
// Get Order Query
public record GetOrderQuery(OrderId OrderId) : IQuery<OrderDto>;

// Get Orders Query
public record GetOrdersQuery(
    CustomerId? CustomerId = null,
    OrderStatus? Status = null,
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResult<OrderDto>>, IPagedQuery;

// Order DTO
public class OrderDto
{
    public OrderId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? TrackingNumber { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

// Query Handler
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;
    private readonly ICache _cache;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(
        IMongoRepository<OrderReadModel, Guid> readRepository,
        ICache cache,
        ILogger<GetOrderQueryHandler> logger)
    {
        _readRepository = readRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"order:{query.OrderId}";

        // Try cache first
        var cachedOrder = await _cache.GetAsync<OrderDto>(cacheKey);
        if (cachedOrder != null)
        {
            _logger.LogDebug("Cache hit for order {OrderId}", query.OrderId);
            return cachedOrder;
        }

        // Load from read database (MongoDB)
        var readModel = await _readRepository.GetAsync(query.OrderId, cancellationToken);
        if (readModel == null)
        {
            throw new OrderNotFoundException(query.OrderId);
        }

        // Map to DTO
        var orderDto = new OrderDto
        {
            Id = readModel.Id,
            CustomerId = readModel.CustomerId,
            TotalAmount = readModel.TotalAmount,
            Status = readModel.Status,
            CreatedAt = readModel.CreatedAt,
            ConfirmedAt = readModel.ConfirmedAt,
            ShippedAt = readModel.ShippedAt,
            TrackingNumber = readModel.TrackingNumber,
            Items = readModel.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        // Cache for 30 minutes
        await _cache.SetAsync(cacheKey, orderDto, TimeSpan.FromMinutes(30));

        return orderDto;
    }
}
```

#### Event Handlers

```csharp
// Order Created Event Handler - Updates Read Model
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCreatedEvent for order {OrderId}", @event.OrderId);

        var readModel = new OrderReadModel
        {
            Id = @event.OrderId,
            CustomerId = @event.CustomerId,
            TotalAmount = @event.TotalAmount,
            Status = "Pending",
            CreatedAt = @event.CreatedAt
        };

        await _readRepository.AddAsync(readModel);

        _logger.LogInformation("Read model created for order {OrderId}", @event.OrderId);
    }
}

// Order Confirmed Event Handler - Sends Notification
public class OrderConfirmedEventHandler : IEventHandler<OrderConfirmedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderConfirmedEventHandler> _logger;

    public async Task HandleAsync(OrderConfirmedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderConfirmedEvent for order {OrderId}", @event.OrderId);

        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(@event.OrderId);

        _logger.LogInformation("Confirmation email sent for order {OrderId}", @event.OrderId);
    }
}
```

#### Read Model

```csharp
// MongoDB Read Model
public class OrderReadModel
{
    [BsonId]
    public OrderId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? TrackingNumber { get; set; }
    public List<OrderItemReadModel> Items { get; set; } = new();

    // Denormalized data for performance
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}
```

#### API Endpoints

```csharp
var app = builder.Build();

app.UseWebApi()
    // Create Order
    .Post<CreateOrderCommand>("/api/orders", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 201;
    })

    // Get Order
    .Get<GetOrderQuery, OrderDto>("/api/orders/{id}", async (query, result, ctx) =>
    {
        if (result == null)
        {
            ctx.Response.StatusCode = 404;
            return;
        }
        await ctx.Response.WriteJsonAsync(result);
    })

    // Get Orders (Paged)
    .Get<GetOrdersQuery, PagedResult<OrderDto>>("/api/orders", async (query, result, ctx) =>
    {
        await ctx.Response.WriteJsonAsync(result);
    })

    // Confirm Order
    .Post<ConfirmOrderCommand>("/api/orders/{id}/confirm", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 200;
    })

    // Ship Order
    .Post<ShipOrderCommand>("/api/orders/{id}/ship", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 200;
    })

    // Cancel Order
    .Post<CancelOrderCommand>("/api/orders/{id}/cancel", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 200;
    });
```

#### Complete Service Setup

```csharp
// Program.cs
using Mamey;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.MessageBrokers.Outbox;
using Mamey.Persistence.PostgreSQL;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.Redis;
using Mamey.Auth.Jwt;
using Mamey.WebApi;
using Mamey.Logging;
using Mamey.Tracing.Jaeger;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
builder.Services
    .AddMamey()
    .AddMicroserviceSharedInfrastructure()
    .AddCommands()
    .AddQueries()
    .AddEvents()
    .AddRabbitMQ(options =>
    {
        options.HostNames = new[] { "localhost" };
        options.Port = 5672;
    })
    .AddMessageOutbox()
    .AddPostgres<ApplicationDbContext>(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    })
    .AddMongo(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("MongoDB");
        options.Database = "mamey_orders";
    })
    .AddRedis(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("Redis");
    })
    .AddJwt(options =>
    {
        options.IssuerSigningKey = builder.Configuration["Jwt:SecretKey"];
        options.ValidIssuer = "mamey-app";
        options.ValidAudience = "mamey-users";
        options.ExpiryMinutes = 60;
    })
    .AddWebApi()
    .AddLogging()
    .AddJaeger();

// Configure logging
builder.Host.UseLogging(options =>
{
    options.Level = "Information";
    options.Console.Enabled = true;
    options.File.Enabled = true;
    options.File.Path = "logs/orders.log";
    options.Seq.Enabled = true;
    options.Seq.Url = "http://localhost:5341";
});

var app = builder.Build();

// Configure middleware
app.UseMamey()
    .UseJaeger()
    .UseSharedInfrastructure()
    .UseWebApi();

// Configure endpoints (see above)

app.Run();
```

#### Configuration

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=mamey_orders;Username=mamey;Password=mamey",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "mamey-app",
    "Audience": "mamey-users",
    "ExpiryMinutes": 60
  },
  "RabbitMQ": {
    "HostNames": ["localhost"],
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  },
  "Logger": {
    "Level": "Information",
    "Console": {
      "Enabled": true
    },
    "File": {
      "Enabled": true,
      "Path": "logs/orders.log"
    },
    "Seq": {
      "Enabled": true,
      "Url": "http://localhost:5341"
    }
  },
  "Jaeger": {
    "Enabled": true,
    "ServiceName": "order-service",
    "UdpHost": "localhost",
    "UdpPort": 6831
  }
}
```

### Pattern 6: Multi-Tenant Architecture

```csharp
// Multi-tenant aggregate
public class Order : AggregateRoot<OrderId>
{
    public TenantId TenantId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    // ... other properties

    public Order(OrderId id, TenantId tenantId, CustomerId customerId, ...) : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        // ...
    }
}

// Multi-tenant repository
public class OrderRepository : IEFRepository<Order, OrderId>
{
    private readonly IContext _context;
    private readonly ApplicationDbContext _dbContext;

    public async Task<Order> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        // Filter by tenant from context
        var tenantId = _context.Identity.TenantId;
        
        return await _dbContext.Orders
            .Where(o => o.Id == id && o.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

// Multi-tenant query handler
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IContext _context;
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _context.Identity.TenantId;
        
        // Filter by tenant
        var order = await _readRepository.FindAsync(
            o => o.Id == query.OrderId && o.TenantId == tenantId,
            cancellationToken);

        if (order == null)
            throw new OrderNotFoundException(query.OrderId);

        return MapToDto(order);
    }
}
```

### Pattern 7: Caching Strategy

```csharp
// Multi-level caching
public class OrderService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ICache _redisCache;
    private readonly IMongoRepository<OrderReadModel, Guid> _readRepository;

    public async Task<OrderDto> GetOrderAsync(OrderId orderId)
    {
        var cacheKey = $"order:{orderId}";

        // Level 1: Memory cache
        if (_memoryCache.TryGetValue(cacheKey, out OrderDto cachedOrder))
        {
            return cachedOrder;
        }

        // Level 2: Redis cache
        var redisOrder = await _redisCache.GetAsync<OrderDto>(cacheKey);
        if (redisOrder != null)
        {
            _memoryCache.Set(cacheKey, redisOrder, TimeSpan.FromMinutes(5));
            return redisOrder;
        }

        // Level 3: Database
        var readModel = await _readRepository.GetAsync(orderId);
        if (readModel == null)
            throw new OrderNotFoundException(orderId);

        var orderDto = MapToDto(readModel);

        // Cache in both Redis and memory
        await _redisCache.SetAsync(cacheKey, orderDto, TimeSpan.FromHours(1));
        _memoryCache.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));

        return orderDto;
    }

    public async Task InvalidateCacheAsync(OrderId orderId)
    {
        var cacheKey = $"order:{orderId}";
        _memoryCache.Remove(cacheKey);
        await _redisCache.DeleteAsync<OrderDto>(cacheKey);
    }
}

// Cache invalidation on updates
public class OrderUpdatedEventHandler : IEventHandler<OrderUpdatedEvent>
{
    private readonly OrderService _orderService;

    public async Task HandleAsync(OrderUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Invalidate cache when order is updated
        await _orderService.InvalidateCacheAsync(@event.OrderId);
    }
}
```

### Pattern 8: Retry and Circuit Breaker

```csharp
// HTTP client with retry and circuit breaker
public class PaymentServiceClient : MameyHttpClient
{
    public PaymentServiceClient(
        HttpClient httpClient,
        HttpClientOptions options,
        IHttpClientSerializer serializer,
        ILogger<PaymentServiceClient> logger)
        : base(httpClient, options, serializer, logger)
    {
        // Configure retry
        options.Retries = 3;
        
        // Configure circuit breaker
        options.CircuitBreaker = new CircuitBreakerOptions
        {
            Enabled = true,
            FailureThreshold = 5,
            DurationOfBreak = TimeSpan.FromMinutes(1)
        };
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        var response = await PostAsync<PaymentResult>("/api/payments", request);
        return response;
    }
}

// Command handler with retry
public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly PaymentServiceClient _paymentClient;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public async Task HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _paymentClient.ProcessPaymentAsync(new PaymentRequest
            {
                OrderId = command.OrderId,
                Amount = command.Amount,
                PaymentMethod = command.PaymentMethod
            });

            if (!result.Success)
                throw new PaymentFailedException(result.ErrorMessage);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Payment service unavailable for order {OrderId}", command.OrderId);
            throw;
        }
    }
}
```

### Pattern 9: Health Checks

```csharp
// Health check configuration
builder.Services
    .AddHealthChecks()
    .AddPostgres(builder.Configuration.GetConnectionString("PostgreSQL"))
    .AddMongo(builder.Configuration.GetConnectionString("MongoDB"))
    .AddRedis(builder.Configuration.GetConnectionString("Redis"))
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ"));

// Health check endpoint
app.UseHealthChecks("/health");

// Custom health check
public class OrderServiceHealthCheck : IHealthCheck
{
    private readonly IOrderRepository _orderRepository;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check database connectivity
            var testOrder = await _orderRepository.GetByIdAsync(new OrderId(Guid.NewGuid()), cancellationToken);
            
            return HealthCheckResult.Healthy("Order service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Order service is unhealthy", ex);
        }
    }
}
```

### Pattern 10: API Versioning

```csharp
// Versioned API endpoints
app.UseWebApi()
    // v1 endpoints
    .Post<CreateOrderCommand>("/api/v1/orders", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 201;
    })
    .Get<GetOrderQuery, OrderDto>("/api/v1/orders/{id}", async (query, result, ctx) =>
    {
        await ctx.Response.WriteJsonAsync(result);
    })

    // v2 endpoints with enhanced features
    .Post<CreateOrderCommandV2>("/api/v2/orders", async (command, ctx) =>
    {
        await ctx.SendAsync(command);
        ctx.Response.StatusCode = 201;
    });
```

---

## Best Practices

### 1. Aggregate Design

**✅ Good: Single Responsibility Aggregates**

```csharp
// Order aggregate handles order-specific logic
public class Order : AggregateRoot<OrderId>
{
    public void Confirm() { /* Order confirmation logic */ }
    public void Ship() { /* Shipping logic */ }
    public void Cancel(string reason) { /* Cancellation logic */ }
}

// Customer aggregate handles customer-specific logic
public class Customer : AggregateRoot<CustomerId>
{
    public void UpdateEmail(string email) { /* Email update logic */ }
    public void AddAddress(Address address) { /* Address management */ }
}
```

**❌ Bad: God Aggregates**

```csharp
// Don't put everything in one aggregate
public class Order : AggregateRoot<OrderId>
{
    // Too many responsibilities
    public void ProcessPayment() { }
    public void UpdateInventory() { }
    public void SendEmail() { }
    public void GenerateInvoice() { }
}
```

### 2. Command Design

**✅ Good: Single-Purpose Commands**

```csharp
public record CreateOrderCommand(string CustomerId, decimal Amount) : ICommand;
public record ConfirmOrderCommand(OrderId OrderId) : ICommand;
public record CancelOrderCommand(OrderId OrderId, string Reason) : ICommand;
```

**❌ Bad: Multi-Purpose Commands**

```csharp
// Don't combine multiple operations
public record OrderCommand(OrderId OrderId, string Action, string Data) : ICommand;
```

### 3. Event Design

**✅ Good: Domain Events with Rich Context**

```csharp
public record OrderCreatedEvent(
    OrderId OrderId,
    string CustomerId,
    decimal Amount,
    DateTime OccurredAt) : IDomainEvent;

public record OrderConfirmedEvent(
    OrderId OrderId,
    DateTime ConfirmedAt,
    string ConfirmedBy) : IDomainEvent;
```

**❌ Bad: Generic Events**

```csharp
// Don't use generic events
public record OrderEvent(OrderId OrderId, string EventType, object Data) : IDomainEvent;
```

### 4. Repository Usage

**✅ Good: Repository per Aggregate**

```csharp
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(OrderId id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(CustomerId id);
    Task AddAsync(Customer customer);
}
```

**❌ Bad: Generic Repository for Everything**

```csharp
// Don't use one repository for all aggregates
public interface IGenericRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
    // Loses type safety and domain-specific methods
}
```

### 5. Error Handling

**✅ Good: Domain Exceptions**

```csharp
public class OrderNotFoundException : DomainException
{
    public OrderNotFoundException(OrderId orderId) 
        : base($"Order with ID {orderId} was not found")
    {
        OrderId = orderId;
    }
    
    public OrderId OrderId { get; }
}

// Usage in handler
public async Task<OrderDto> HandleAsync(GetOrderQuery query)
{
    var order = await _repository.GetByIdAsync(query.OrderId);
    if (order == null)
        throw new OrderNotFoundException(query.OrderId);
        
    return MapToDto(order);
}
```

**❌ Bad: Generic Exceptions**

```csharp
// Don't use generic exceptions
throw new Exception("Order not found");
```

### 6. Validation

**✅ Good: Validation at Aggregate Level**

```csharp
public class Order : AggregateRoot<OrderId>
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; private set; }
    
    public Order(OrderId id, decimal amount) : base(id)
    {
        TotalAmount = amount;
        Validate(); // Validate before adding events
        AddEvent(new OrderCreatedEvent(Id, TotalAmount));
    }
}
```

**❌ Bad: Validation Only in Handlers**

```csharp
// Don't validate only in handlers
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    public async Task HandleAsync(CreateOrderCommand command)
    {
        if (command.Amount <= 0) // Validation in handler
            throw new Exception("Invalid amount");
            
        // Order created without validation
        var order = new Order(command.Amount);
    }
}
```

---

## Architecture Patterns

### Pattern 1: Event-Driven Microservices

```
┌─────────────┐
│  API Gateway │
└──────┬──────┘
       │
       ├───► Order Service (Command)
       │         │
       │         ├──► PostgreSQL (Write)
       │         └──► Outbox
       │                   │
       │                   └──► RabbitMQ
       │                              │
       │                              ├──► Inventory Service (Event Handler)
       │                              │         └──► MongoDB (Read Model)
       │                              │
       │                              └──► Notification Service (Event Handler)
       │                                        └──► Send Email
       │
       └───► Order Service (Query)
                 │
                 └──► MongoDB (Read Model)
```

### Pattern 2: Dual Persistence Strategy

**Write Path**:
```
Command → Handler → PostgreSQL (Write Model) → Outbox → Event
```

**Read Path**:
```
Query → Handler → MongoDB (Read Model) → DTO → Response
```

**Event Path**:
```
Event → Handler → MongoDB (Read Model Update) → Cache Invalidation
```

### Pattern 3: Service-to-Service Communication

**HTTP (Synchronous)**:
```csharp
var orderService = serviceProvider.GetRequiredService<IOrderServiceClient>();
var order = await orderService.GetOrderAsync(orderId);
```

**Messaging (Asynchronous)**:
```csharp
await _publisher.PublishAsync(new OrderCreatedEvent(orderId, customerId, amount));
```

**Service Discovery**:
```csharp
var orderServiceUrl = await _consulService.GetServiceUrlAsync("order-service");
var response = await _httpClient.GetAsync($"{orderServiceUrl}/api/orders/{orderId}");
```

---

## Deployment Guide

### Docker Compose Setup

```yaml
version: '3.8'

services:
  consul:
    image: consul:latest
    ports:
      - "8500:8500"
  
  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
  
  mongodb:
    image: mongo:latest
    ports:
      - "27017:27017"
  
  postgresql:
    image: postgres:latest
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: mamey
      POSTGRES_USER: mamey
      POSTGRES_PASSWORD: mamey
  
  redis:
    image: redis:latest
    ports:
      - "6379:6379"
  
  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "16686:16686"
      - "6831:6831/udp"
  
  prometheus:
    image: prom/prometheus:latest
    ports:
      - "9090:9090"
  
  order-service:
    build: ./OrderService
    ports:
      - "5000:80"
    environment:
      - Consul__Url=http://consul:8500
      - RabbitMQ__HostNames=rabbitmq
      - MongoDB__ConnectionString=mongodb://mongodb:27017
      - PostgreSQL__ConnectionString=Host=postgresql;Database=mamey
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: order-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: order-service
  template:
    metadata:
      labels:
        app: order-service
    spec:
      containers:
      - name: order-service
        image: mamey/order-service:latest
        ports:
        - containerPort: 80
        env:
        - name: Consul__Url
          value: "http://consul:8500"
        - name: RabbitMQ__HostNames
          value: "rabbitmq"
```

---

### Pattern 11: Authentication and Authorization

```csharp
// JWT authentication setup
builder.Services
    .AddMamey()
    .AddJwt(options =>
    {
        options.IssuerSigningKey = builder.Configuration["Jwt:SecretKey"];
        options.ValidIssuer = "mamey-app";
        options.ValidAudience = "mamey-users";
        options.ExpiryMinutes = 60;
    });

// Protected endpoints
app.UseWebApi()
    // Public endpoint
    .Get("/api/public/info", async ctx =>
    {
        await ctx.Response.WriteJsonAsync(new { Version = "2.0.0" });
    })

    // Protected endpoint (requires authentication)
    .Get<GetOrderQuery, OrderDto>("/api/orders/{id}", async (query, result, ctx) =>
    {
        await ctx.Response.WriteJsonAsync(result);
    }, auth: true)

    // Admin-only endpoint
    .Delete<OrderId>("/api/orders/{id}", async (id, ctx) =>
    {
        await ctx.SendAsync(new CancelOrderCommand(id, "Admin cancellation"));
        ctx.Response.StatusCode = 204;
    }, auth: true, roles: "Admin");

// Authorization in command handlers
public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IContext _context;
    private readonly IOrderRepository _orderRepository;

    public async Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId);
        
        // Check authorization
        var userId = _context.Identity.Id;
        var isAdmin = _context.Identity.IsAdmin;
        
        if (order.CustomerId != userId && !isAdmin)
        {
            throw new UnauthorizedAccessException("Only the order owner or admin can cancel orders");
        }

        order.Cancel(command.Reason);
        await _orderRepository.UpdateAsync(order);
    }
}
```

### Pattern 12: Distributed Tracing

```csharp
// Automatic tracing with Jaeger
builder.Services
    .AddMamey()
    .AddJaeger(options =>
    {
        options.ServiceName = "order-service";
        options.UdpHost = "localhost";
        options.UdpPort = 6831;
        options.SamplingRate = 0.1; // Sample 10% of requests
    });

// Custom spans in handlers
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly ITracer _tracer;
    private readonly IOrderRepository _orderRepository;

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        using var span = _tracer
            .BuildSpan("create_order")
            .WithTag("customer_id", command.CustomerId.ToString())
            .WithTag("order_amount", command.Amount.ToString())
            .StartActive();

        try
        {
            span.Span.Log("Creating order aggregate");
            var order = new Order(/* ... */);
            
            span.Span.Log("Saving order to database");
            await _orderRepository.AddAsync(order);
            
            span.Span.Log("Order created successfully");
        }
        catch (Exception ex)
        {
            span.Span.SetTag("error", true);
            span.Span.Log(ex.Message);
            throw;
        }
    }
}
```

### Pattern 13: Metrics Collection

```csharp
// Prometheus metrics
builder.Services
    .AddMamey()
    .AddPrometheus();

// Custom metrics
public class OrderMetrics
{
    private readonly Counter _ordersCreated;
    private readonly Histogram _orderProcessingTime;
    private readonly Gauge _activeOrders;

    public OrderMetrics()
    {
        _ordersCreated = Metrics.CreateCounter(
            "orders_created_total",
            "Total orders created",
            new[] { "status", "customer_type" });

        _orderProcessingTime = Metrics.CreateHistogram(
            "order_processing_seconds",
            "Order processing time",
            new[] { "operation" });

        _activeOrders = Metrics.CreateGauge(
            "orders_active",
            "Currently active orders");
    }

    public void RecordOrderCreated(string status, string customerType)
    {
        _ordersCreated.WithLabels(status, customerType).Inc();
    }

    public void RecordProcessingTime(string operation, TimeSpan duration)
    {
        _orderProcessingTime.WithLabels(operation).Observe(duration.TotalSeconds);
    }

    public void UpdateActiveOrders(int count)
    {
        _activeOrders.Set(count);
    }
}

// Usage in handlers
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly OrderMetrics _metrics;
    private readonly IOrderRepository _orderRepository;

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var order = new Order(/* ... */);
            await _orderRepository.AddAsync(order);

            _metrics.RecordOrderCreated("pending", "standard");
            _metrics.RecordProcessingTime("create_order", stopwatch.Elapsed);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
```

### Pattern 14: Error Handling

```csharp
// Global error handler
public class GlobalExceptionHandler : IExceptionToResponseMapper
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public ExceptionResponse Map(Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        return exception switch
        {
            OrderNotFoundException ex => new ExceptionResponse
            {
                StatusCode = 404,
                Error = new ErrorDetails
                {
                    Code = "ORDER_NOT_FOUND",
                    Message = $"Order with ID {ex.OrderId} was not found"
                }
            },

            ValidationException ex => new ExceptionResponse
            {
                StatusCode = 400,
                Error = new ErrorDetails
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Validation failed",
                    Errors = ex.Errors
                }
            },

            UnauthorizedAccessException => new ExceptionResponse
            {
                StatusCode = 401,
                Error = new ErrorDetails
                {
                    Code = "UNAUTHORIZED",
                    Message = "Unauthorized access"
                }
            },

            _ => new ExceptionResponse
            {
                StatusCode = 500,
                Error = new ErrorDetails
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An internal error occurred"
                }
            }
        };
    }
}

// Registration
builder.Services.AddSingleton<IExceptionToResponseMapper, GlobalExceptionHandler>();

// Custom exceptions
public class OrderNotFoundException : DomainException
{
    public OrderId OrderId { get; }

    public OrderNotFoundException(OrderId orderId)
        : base($"Order with ID {orderId} was not found")
    {
        OrderId = orderId;
    }
}

public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}
```

### Pattern 15: Testing

```csharp
// Unit test example
public class OrderTests
{
    [Fact]
    public void CreateOrder_ShouldCreateOrderWithPendingStatus()
    {
        // Arrange
        var orderId = new OrderId(Guid.NewGuid());
        var customerId = new CustomerId(Guid.NewGuid());
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", 2, 10.00m)
        };

        // Act
        var order = new Order(orderId, customerId, items);

        // Assert
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(20.00m, order.TotalAmount);
        Assert.Single(order.Events);
        Assert.IsType<OrderCreatedEvent>(order.Events.First());
    }

    [Fact]
    public void ConfirmOrder_WhenPending_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var order = CreatePendingOrder();

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.NotNull(order.ConfirmedAt);
        Assert.Contains(order.Events, e => e is OrderConfirmedEvent);
    }

    [Fact]
    public void ConfirmOrder_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var order = CreateConfirmedOrder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }
}

// Integration test example
public class OrderServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturn201Created()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 10.00m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(order);
        Assert.Equal(10.00m, order.TotalAmount);
    }
}
```

---

## Reference

### Library Categories

1. **Core Framework** (1 library)
   - Mamey

2. **CQRS** (3 libraries)
   - Commands, Queries, Events

3. **Messaging** (6 libraries)
   - MessageBrokers, RabbitMQ, Outbox (base, EF Core, MongoDB), CQRS integration

4. **Persistence** (8 libraries)
   - MongoDB, PostgreSQL, Redis, SQL, MySQL, MinIO, OpenStack

5. **Authentication** (12 libraries)
   - Auth, Auth.Abstractions, Auth.Jwt, Auth.Jwt.BlazorWasm, Auth.Jwt.Server, Auth.Azure, Auth.Azure.B2B, Auth.Azure.B2C, Auth.Identity, Auth.Decentralized, Auth.DecentralizedIdentifiers, Auth.Distributed

6. **Identity** (9 libraries)
   - Identity.Core, Identity.AspNetCore, Identity.Azure, Identity.Blazor, Identity.Decentralized, Identity.Distributed, Identity.EntityFramework, Identity.Jwt, Identity.Redis

7. **Web/API** (7 libraries)
   - WebApi, WebApi.CQRS, WebApi.Security, WebApi.Swagger, Http, Http.RestEase, Docs.Swagger

8. **Infrastructure** (12 libraries)
   - Discovery.Consul, LoadBalancing.Fabio, Ntrada, Microservice.Infrastructure, Microservice.Abstractions, MicroMonolith, Secrets.Vault, Net, Policies, Micro, Modules

9. **Observability** (7 libraries)
   - Logging, Logging.CQRS, Tracing.Jaeger, Tracing.Jaeger.RabbitMQ, OpenTracingContrib, Metrics.Prometheus, Metrics.AppMetrics

10. **Security** (1 library)
    - Security

11. **Integrations** (11 libraries)
    - Azure.Blobs, Azure.Abstractions, Azure.Identity, Azure.Identity.BlazorWasm, Stripe, Twilio, Visa, Emails, Adobe, Ktt, Mifos, OpenBanking, Web3

12. **Standards** (11 libraries)
    - ISO.Abstractions, ISO13616, ISO20022, ISO22301, ISO27001, ISO3166, ISO4217, ISO639, ISO8583, ISO9362, PCI_DSS

13. **Specialized** (9 libraries)
    - Image, Biometrics, Barcode, Blockchain, Graph, Algorithms, Binimoy, TravelIdentityStandards, AmvvaStandards, Mock

14. **Document Processing** (3 libraries)
    - Excel, Word, Templates

15. **UI** (4 libraries)
    - BlazorWasm, Blazor.Abstractions, Blazor.Identity, Maui

### Common Configuration Pattern

```csharp
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
```

### Common Middleware Order

```csharp
app.UseMamey()
    .UseJaeger()                // Tracing first
    .UseSharedInfrastructure()   // Context and auth
    .UseWebApi()                 // API handling
    .UseErrorHandler();          // Error handling last
```

### Configuration File Template

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=mydb;Username=user;Password=pass",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://guest:guest@localhost:5672/"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "mamey-app",
    "Audience": "mamey-users",
    "ExpiryMinutes": 60
  },
  "RabbitMQ": {
    "HostNames": ["localhost"],
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "mydb"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "Instance": "MyApp",
    "Database": 0
  },
  "Consul": {
    "Enabled": true,
    "Url": "http://localhost:8500",
    "Service": "my-service",
    "Address": "localhost",
    "Port": 5000
  },
  "Logger": {
    "Level": "Information",
    "Console": {
      "Enabled": true
    },
    "File": {
      "Enabled": true,
      "Path": "logs/app.log",
      "RollingInterval": "Day"
    },
    "Seq": {
      "Enabled": true,
      "Url": "http://localhost:5341"
    },
    "Overrides": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "Jaeger": {
    "Enabled": true,
    "ServiceName": "my-service",
    "UdpHost": "localhost",
    "UdpPort": 6831,
    "SamplingRate": 0.1
  },
  "Prometheus": {
    "Enabled": true,
    "Endpoint": "/metrics"
  }
}
```

### Common NuGet Packages

```xml
<ItemGroup>
  <!-- Core Framework -->
  <PackageReference Include="Mamey" Version="2.0.*" />
  <PackageReference Include="Mamey.Microservice.Infrastructure" Version="2.0.*" />
  
  <!-- CQRS -->
  <PackageReference Include="Mamey.CQRS.Commands" Version="2.0.*" />
  <PackageReference Include="Mamey.CQRS.Queries" Version="2.0.*" />
  <PackageReference Include="Mamey.CQRS.Events" Version="2.0.*" />
  
  <!-- Messaging -->
  <PackageReference Include="Mamey.MessageBrokers.RabbitMQ" Version="2.0.*" />
  <PackageReference Include="Mamey.MessageBrokers.Outbox" Version="2.0.*" />
  
  <!-- Persistence -->
  <PackageReference Include="Mamey.Persistence.PostgreSQL" Version="2.0.*" />
  <PackageReference Include="Mamey.Persistence.MongoDB" Version="2.0.*" />
  <PackageReference Include="Mamey.Persistence.Redis" Version="2.0.*" />
  
  <!-- Authentication -->
  <PackageReference Include="Mamey.Auth.Jwt" Version="2.0.*" />
  
  <!-- Web API -->
  <PackageReference Include="Mamey.WebApi" Version="2.0.*" />
  <PackageReference Include="Mamey.WebApi.Swagger" Version="2.0.*" />
  
  <!-- Infrastructure -->
  <PackageReference Include="Mamey.Discovery.Consul" Version="2.0.*" />
  <PackageReference Include="Mamey.Ntrada" Version="2.0.*" />
  
  <!-- Observability -->
  <PackageReference Include="Mamey.Logging" Version="2.0.*" />
  <PackageReference Include="Mamey.Tracing.Jaeger" Version="2.0.*" />
  <PackageReference Include="Mamey.Metrics.Prometheus" Version="2.0.*" />
</ItemGroup>
```

### Quick Start Template

```csharp
// Minimal microservice setup
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

### Decision Guide

#### When to Use PostgreSQL vs MongoDB?

**Use PostgreSQL when:**
- ACID transactions are required
- Complex relational queries needed
- Strong consistency required
- Write model for domain aggregates

**Use MongoDB when:**
- Flexible schema needed
- Horizontal scaling required
- Read model for queries
- Document-based data model

#### When to Use Redis vs Memory Cache?

**Use Redis when:**
- Multiple service instances
- Distributed caching needed
- Cache sharing between services
- Persistence required

**Use Memory Cache when:**
- Single service instance
- Local caching sufficient
- Fastest access needed
- No cache sharing required

#### When to Use Commands vs Queries?

**Use Commands for:**
- Creating new aggregates
- Updating aggregates
- Business operations that change state
- Operations that need validation

**Use Queries for:**
- Reading data
- Reporting
- Data retrieval
- Read-only operations

#### When to Use Events?

**Use Events for:**
- Cross-service communication
- Read model updates
- Side effects (notifications, emails)
- Event sourcing
- Saga orchestration

### Common Patterns Summary

1. **CQRS Pattern**: Separate read and write operations
2. **Outbox Pattern**: Reliable message publishing
3. **Dual Persistence**: PostgreSQL for writes, MongoDB for reads
4. **Event-Driven**: Asynchronous communication via events
5. **Saga Pattern**: Distributed transaction coordination
6. **Correlation Tracking**: Request tracking across services
7. **Multi-Tenant**: Tenant isolation in data and operations
8. **Caching Strategy**: Multi-level caching (Memory + Redis)
9. **Health Checks**: Service health monitoring
10. **Distributed Tracing**: Request flow tracking

### Performance Optimization Tips

1. **Use Read Models**: Denormalize data for fast queries
2. **Implement Caching**: Cache frequently accessed data
3. **Use Async Operations**: All I/O operations should be async
4. **Optimize Queries**: Use appropriate indexes
5. **Batch Operations**: Batch database operations when possible
6. **Connection Pooling**: Configure connection pooling
7. **Pagination**: Always paginate large result sets
8. **Compression**: Enable response compression
9. **CDN**: Use CDN for static assets
10. **Monitoring**: Monitor performance metrics

### Security Best Practices

1. **Use HTTPS**: Always use HTTPS in production
2. **Validate Input**: Validate all input data
3. **Encrypt Sensitive Data**: Encrypt sensitive data at rest
4. **Use Strong Secrets**: Use strong, random secrets
5. **Token Expiration**: Set appropriate token expiration
6. **Rate Limiting**: Implement rate limiting
7. **CORS**: Configure CORS properly
8. **SQL Injection**: Use parameterized queries
9. **XSS Protection**: Sanitize user input
10. **Audit Logging**: Log all security events

---

## Tags

#mamey-framework #master-documentation #microservices #net #ddd #cqrs #event-driven #architecture #documentation #complete-examples
