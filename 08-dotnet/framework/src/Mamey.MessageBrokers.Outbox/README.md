# Mamey.MessageBrokers.Outbox

The Mamey.MessageBrokers.Outbox library provides a complete implementation of the Outbox pattern for reliable message publishing in distributed systems. It ensures message delivery guarantees by storing messages in a local database before publishing them to message brokers, preventing message loss during system failures.

## Technical Overview

Mamey.MessageBrokers.Outbox implements several key patterns:

- **Outbox Pattern**: Ensures reliable message delivery by storing messages locally before publishing
- **Transactional Guarantees**: Messages are stored in the same transaction as business data
- **Idempotency**: Prevents duplicate message processing through inbox tracking
- **Background Processing**: Hosted service for processing outbox messages
- **Multiple Storage Backends**: Support for in-memory, Entity Framework, and MongoDB storage
- **Configurable Processing**: Sequential or parallel message processing modes

## Architecture

The library follows a layered architecture:

```
┌─────────────────────────────────────┐
│         Business Logic              │
│    (Commands, Events, Handlers)     │
├─────────────────────────────────────┤
│         Outbox Processor            │
│    (Background Service)             │
├─────────────────────────────────────┤
│         Message Outbox              │
│    (IMessageOutbox)                 │
├─────────────────────────────────────┤
│         Storage Backend             │
│    (InMemory, EF, MongoDB)          │
├─────────────────────────────────────┤
│         Message Broker              │
│    (RabbitMQ, Kafka, etc.)          │
└─────────────────────────────────────┘
```

## Core Components

### Message Outbox
- **IMessageOutbox**: Interface for storing and retrieving outbox messages
- **Message Storage**: Persistent storage for messages before publishing
- **Idempotency**: Inbox tracking to prevent duplicate processing
- **Transaction Support**: Integration with database transactions

### Outbox Processor
- **Background Service**: Hosted service for processing outbox messages
- **Configurable Processing**: Sequential or parallel message processing
- **Retry Logic**: Built-in retry mechanisms for failed messages
- **Expiry Management**: Automatic cleanup of processed messages

### Storage Backends
- **InMemoryMessageOutbox**: In-memory storage for development and testing
- **EntityFrameworkMessageOutbox**: Entity Framework Core integration
- **MongoMessageOutbox**: MongoDB integration for document storage

## Installation

### NuGet Package
```bash
dotnet add package Mamey.MessageBrokers.Outbox
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)
- Mamey.MessageBrokers (abstractions)
- Storage backend (Entity Framework, MongoDB, or in-memory)

## Key Features

- **Reliable Messaging**: Guarantees message delivery through local storage
- **Transactional Consistency**: Messages stored in same transaction as business data
- **Idempotency**: Prevents duplicate message processing
- **Background Processing**: Automatic processing of outbox messages
- **Multiple Storage Backends**: Support for different storage technologies
- **Configurable Processing**: Sequential or parallel message processing
- **Message Expiry**: Automatic cleanup of processed messages
- **Error Handling**: Built-in retry mechanisms and error recovery
- **Performance**: Optimized for high-throughput message processing

## Quick Start

### Basic Setup

```csharp
using Mamey;
using Mamey.MessageBrokers.Outbox;

var builder = WebApplication.CreateBuilder(args);

// Create Mamey builder
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Add message outbox
mameyBuilder.AddMessageOutbox();

var app = builder.Build();
app.Run();
```

### Configuration

```json
{
  "outbox": {
    "enabled": true,
    "expiry": 3600,
    "intervalMilliseconds": 1000,
    "inboxCollection": "inbox_messages",
    "outboxCollection": "outbox_messages",
    "type": "Sequential",
    "disableTransactions": false
  }
}
```

### Using Outbox in Command Handlers

```csharp
using Mamey.MessageBrokers.Outbox;
using Mamey.CQRS.Commands;

public class CreateUserCommand : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageOutbox _outbox;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMessageOutbox outbox,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Create user in database
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Store message in outbox (will be published by background service)
        var message = new UserCreatedEvent
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        await _outbox.SendAsync(message);
        _logger.LogInformation("User created and message stored in outbox: {UserId}", user.Id);
    }
}
```

### Using Outbox in Event Handlers

```csharp
using Mamey.MessageBrokers.Outbox;
using Mamey.CQRS.Events;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMessageOutbox _outbox;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IMessageOutbox outbox,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing order created event: {OrderId}", @event.OrderId);

        // Process the event
        await ProcessOrderCreatedAsync(@event);

        // Send notification message through outbox
        var notificationMessage = new OrderNotificationMessage
        {
            OrderId = @event.OrderId,
            CustomerId = @event.CustomerId,
            TotalAmount = @event.TotalAmount,
            CreatedAt = @event.CreatedAt
        };

        await _outbox.SendAsync(
            notificationMessage,
            originatedMessageId: @event.MessageId,
            correlationId: @event.CorrelationId,
            headers: new Dictionary<string, object>
            {
                ["source"] = "order-service",
                ["event_type"] = "order_created"
            });

        _logger.LogInformation("Order notification message stored in outbox: {OrderId}", @event.OrderId);
    }

    private async Task ProcessOrderCreatedAsync(OrderCreatedEvent @event)
    {
        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(@event.CustomerId, @event.OrderId);
        
        // Update inventory
        await _inventoryService.ReserveItemsAsync(@event.OrderId);
    }
}
```

## API Reference

### Core Interfaces

#### IMessageOutbox

Interface for storing and retrieving outbox messages.

```csharp
public interface IMessageOutbox
{
    bool Enabled { get; }
    
    Task HandleAsync(string messageId, Func<Task> handler);
    
    Task SendAsync<T>(T message, string originatedMessageId = null, string messageId = null,
        string correlationId = null, string spanContext = null, object messageContext = null,
        IDictionary<string, object> headers = null) where T : class;
}
```

**Properties:**
- `Enabled`: Whether the outbox is enabled

**Methods:**
- `HandleAsync(string messageId, Func<Task> handler)`: Handles incoming messages with idempotency
- `SendAsync<T>()`: Stores a message in the outbox for later publishing

#### IMessageOutboxAccessor

Interface for accessing outbox messages (internal use).

```csharp
public interface IMessageOutboxAccessor
{
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
    Task ProcessAsync(IEnumerable<OutboxMessage> outboxMessages);
    Task ProcessAsync(OutboxMessage message);
}
```

**Methods:**
- `GetUnsentAsync()`: Retrieves unsent messages from the outbox
- `ProcessAsync(IEnumerable<OutboxMessage> outboxMessages)`: Marks multiple messages as processed
- `ProcessAsync(OutboxMessage message)`: Marks a single message as processed

### Core Classes

#### OutboxMessage

Represents a message stored in the outbox.

```csharp
public sealed class OutboxMessage : IIdentifiable<string>
{
    public string Id { get; set; }
    public string OriginatedMessageId { get; set; }
    public string CorrelationId { get; set; }
    public string SpanContext { get; set; }
    public Dictionary<string, object> Headers { get; set; }
    public string MessageType { get; set; }
    public string MessageContextType { get; set; }
    public object Message { get; set; }
    public object MessageContext { get; set; }
    public string SerializedMessage { get; set; }
    public string SerializedMessageContext { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
```

**Properties:**
- `Id`: Unique identifier for the outbox message
- `OriginatedMessageId`: ID of the original message that triggered this outbox message
- `CorrelationId`: Correlation ID for distributed tracing
- `SpanContext`: Span context for distributed tracing
- `Headers`: Custom headers for the message
- `MessageType`: Type of the message
- `MessageContextType`: Type of the message context
- `Message`: The actual message object
- `MessageContext`: Context object for the message
- `SerializedMessage`: Serialized message for storage
- `SerializedMessageContext`: Serialized message context for storage
- `SentAt`: When the message was stored in the outbox
- `ProcessedAt`: When the message was processed (null if not yet processed)

#### OutboxOptions

Configuration options for the outbox.

```csharp
public class OutboxOptions
{
    public bool Enabled { get; set; }
    public int Expiry { get; set; }
    public double IntervalMilliseconds { get; set; }
    public string InboxCollection { get; set; }
    public string OutboxCollection { get; set; }
    public string Type { get; set; }
    public bool DisableTransactions { get; set; }
}
```

**Properties:**
- `Enabled`: Whether the outbox is enabled
- `Expiry`: Message expiry time in seconds
- `IntervalMilliseconds`: Processing interval in milliseconds
- `InboxCollection`: Collection name for inbox messages
- `OutboxCollection`: Collection name for outbox messages
- `Type`: Processing type (Sequential or Parallel)
- `DisableTransactions`: Whether to disable transactions

### Extension Methods

#### AddMessageOutbox

Registers the message outbox with configuration.

```csharp
public static IMameyBuilder AddMessageOutbox(this IMameyBuilder builder,
    Action<IMessageOutboxConfigurator> configure = null, string sectionName = "outbox")
```

**Parameters:**
- `builder`: The Mamey builder
- `configure`: Optional configuration action
- `sectionName`: Configuration section name (default: "outbox")

**Returns:**
- `IMameyBuilder`: The builder for chaining

#### AddInMemory

Registers in-memory outbox storage.

```csharp
public static IMessageOutboxConfigurator AddInMemory(this IMessageOutboxConfigurator configurator,
    string mongoSectionName = null)
```

## Usage Examples

### Example 1: Basic Outbox Usage

```csharp
using Mamey.MessageBrokers.Outbox;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageOutbox _outbox;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IMessageOutbox outbox,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        // Create order in database
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            Items = request.Items,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order);

        // Store message in outbox
        var message = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        };

        await _outbox.SendAsync(message);
        _logger.LogInformation("Order created and message stored in outbox: {OrderId}", order.Id);
    }
}
```

### Example 2: Outbox with Correlation Context

```csharp
using Mamey.MessageBrokers.Outbox;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageOutbox _outbox;
    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    public UserService(
        IUserRepository userRepository,
        IMessageOutbox outbox,
        ICorrelationContextAccessor correlationContextAccessor)
    {
        _userRepository = userRepository;
        _outbox = outbox;
        _correlationContextAccessor = correlationContextAccessor;
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        // Get correlation context
        var correlationId = _correlationContextAccessor.CorrelationContext?.ToString() 
            ?? Guid.NewGuid().ToString();

        // Create user in database
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Store message in outbox with correlation
        var message = new UserCreatedEvent
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        await _outbox.SendAsync(
            message,
            correlationId: correlationId,
            headers: new Dictionary<string, object>
            {
                ["source"] = "user-service",
                ["version"] = "1.0.0",
                ["correlation-context"] = _correlationContextAccessor.CorrelationContext
            });
    }
}
```

### Example 3: Outbox with Message Context

```csharp
using Mamey.MessageBrokers.Outbox;

public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMessageOutbox _outbox;

    public PaymentService(IPaymentRepository paymentRepository, IMessageOutbox outbox)
    {
        _paymentRepository = paymentRepository;
        _outbox = outbox;
    }

    public async Task ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        // Process payment
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = PaymentStatus.Processing,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment);

        // Create message context
        var messageContext = new PaymentContext
        {
            PaymentId = payment.Id,
            OrderId = request.OrderId,
            Amount = request.Amount,
            ProcessingStartedAt = DateTime.UtcNow
        };

        // Store message in outbox with context
        var message = new PaymentProcessedEvent
        {
            PaymentId = payment.Id,
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = payment.Status,
            ProcessedAt = DateTime.UtcNow
        };

        await _outbox.SendAsync(
            message,
            messageContext: messageContext,
            headers: new Dictionary<string, object>
            {
                ["payment_method"] = request.PaymentMethod,
                ["currency"] = request.Currency,
                ["merchant_id"] = request.MerchantId
            });
    }
}
```

### Example 4: Outbox with Idempotency

```csharp
using Mamey.MessageBrokers.Outbox;

public class NotificationService
{
    private readonly IMessageOutbox _outbox;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IMessageOutbox outbox, ILogger<NotificationService> logger)
    {
        _outbox = outbox;
        _logger = logger;
    }

    public async Task SendNotificationAsync(SendNotificationRequest request)
    {
        // Handle message with idempotency
        await _outbox.HandleAsync(request.MessageId, async () =>
        {
            _logger.LogInformation("Processing notification: {MessageId}", request.MessageId);

            // Send notification
            await _notificationProvider.SendAsync(
                request.Recipient,
                request.Subject,
                request.Body,
                request.Template);

            _logger.LogInformation("Notification sent successfully: {MessageId}", request.MessageId);
        });
    }
}
```

### Example 5: Outbox Configuration

```csharp
using Mamey;
using Mamey.MessageBrokers.Outbox;

var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Configure outbox
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddInMemory(); // or AddEntityFramework() or AddMongo()
}, "outbox");

var app = builder.Build();
app.Run();
```

### Example 6: Outbox with Entity Framework

```csharp
using Mamey;
using Mamey.MessageBrokers.Outbox;

var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Configure outbox with Entity Framework
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddEntityFramework<ApplicationDbContext>();
});

var app = builder.Build();
app.Run();
```

### Example 7: Outbox with MongoDB

```csharp
using Mamey;
using Mamey.MessageBrokers.Outbox;

var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Configure outbox with MongoDB
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddMongo();
});

var app = builder.Build();
app.Run();
```

## Integration Patterns

### Integration with Other Mamey Libraries

The Outbox library integrates seamlessly with other Mamey libraries:

- **Mamey.CQRS.Commands**: Commands can use outbox for reliable message publishing
- **Mamey.CQRS.Events**: Events can use outbox for reliable message publishing
- **Mamey.MessageBrokers**: Outbox works with any message broker implementation
- **Mamey.Persistence**: Integration with Entity Framework and MongoDB

### Integration with ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);
mameyBuilder.AddMessageOutbox();

var app = builder.Build();
app.Run();
```

### Integration with Database Transactions

```csharp
using Microsoft.EntityFrameworkCore.Storage;

public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMessageOutbox _outbox;

    public OrderService(ApplicationDbContext context, IMessageOutbox outbox)
    {
        _context = context;
        _outbox = outbox;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Create order
            var order = new Order { /* ... */ };
            _context.Orders.Add(order);
            
            // Store message in outbox (same transaction)
            var message = new OrderCreatedEvent { /* ... */ };
            await _outbox.SendAsync(message);
            
            // Commit transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

## Configuration Reference

### Service Registration

```csharp
// Basic registration
mameyBuilder.AddMessageOutbox();

// With custom configuration
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddInMemory();
}, "outbox");

// With Entity Framework
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddEntityFramework<ApplicationDbContext>();
});

// With MongoDB
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddMongo();
});
```

### Configuration Options

```json
{
  "outbox": {
    "enabled": true,
    "expiry": 3600,
    "intervalMilliseconds": 1000,
    "inboxCollection": "inbox_messages",
    "outboxCollection": "outbox_messages",
    "type": "Sequential",
    "disableTransactions": false
  }
}
```

### Environment Variables

```bash
# Outbox Configuration
OUTBOX__ENABLED=true
OUTBOX__EXPIRY=3600
OUTBOX__INTERVALMILLISECONDS=1000
OUTBOX__INBOXCOLLECTION=inbox_messages
OUTBOX__OUTBOXCOLLECTION=outbox_messages
OUTBOX__TYPE=Sequential
OUTBOX__DISABLETRANSACTIONS=false
```

## Best Practices

1. **Transactional Consistency**: Always store messages in the same transaction as business data
2. **Idempotency**: Use message IDs to prevent duplicate processing
3. **Error Handling**: Implement proper error handling and retry mechanisms
4. **Message Expiry**: Configure appropriate message expiry times
5. **Processing Mode**: Choose sequential or parallel processing based on requirements
6. **Storage Backend**: Choose appropriate storage backend for your needs
7. **Monitoring**: Monitor outbox processing and message counts
8. **Cleanup**: Ensure expired messages are cleaned up regularly
9. **Correlation**: Use correlation IDs for distributed tracing
10. **Headers**: Use headers for metadata and routing information

## Troubleshooting

### Common Issues

**Messages Not Published**: Check if outbox is enabled and processor is running

**Duplicate Messages**: Ensure proper idempotency handling with message IDs

**Transaction Issues**: Verify database transaction configuration

**Storage Issues**: Check storage backend configuration and connectivity

**Processing Delays**: Adjust processing interval and check for bottlenecks

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Performance Tuning

```csharp
// Optimize for high throughput
mameyBuilder.AddMessageOutbox(configurator =>
{
    configurator.AddInMemory();
}, options =>
{
    options.IntervalMilliseconds = 100; // Process every 100ms
    options.Type = "Parallel"; // Use parallel processing
    options.Expiry = 1800; // 30 minutes
});
```

## Related Libraries

- [Mamey.MessageBrokers](MessageBrokers/README.md) - Message broker abstractions
- [Mamey.MessageBrokers.RabbitMQ](MessageBrokers.RabbitMQ/README.md) - RabbitMQ implementation
- [Mamey.CQRS.Commands](CQRS.Commands/README.md) - Command pattern implementation
- [Mamey.CQRS.Events](CQRS.Events/README.md) - Event handling and dispatching

## Additional Resources

- [Outbox Pattern Guide](../guides/outbox-pattern.md)
- [Reliable Messaging](../guides/reliable-messaging.md)
- [Event-Driven Architecture](../guides/event-driven-architecture.md)
- [Distributed Transactions](../guides/distributed-transactions.md)
