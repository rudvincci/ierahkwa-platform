# Mamey.CQRS.Queries

The Mamey.CQRS.Queries library provides a complete implementation of the Query pattern for CQRS (Command Query Responsibility Segregation) architecture. It implements read-side operations with type-safe query handling, automatic handler discovery, and support for complex query scenarios including pagination and filtering.

## Introduction to Queries

A **Query** represents a read-only operation that **returns data** without modifying state — like *GetAccount*, *FetchReport*, or *ListOrders*.

Queries:
* Do not mutate the system
* Return a response (unlike Commands)
* Are composable and often cacheable
* Can be optimized for performance independently from write operations

## Technical Overview

Mamey.CQRS.Queries implements several key patterns:

- **Query Pattern**: Encapsulates read operations as objects with strongly-typed return values
- **Dependency Injection**: Seamless integration with .NET DI container for handler resolution
- **Auto-Discovery**: Uses reflection to automatically discover and register query handlers with dependency validation
- **Type Safety**: Generic interfaces ensure compile-time type safety for queries and results
- **Scoped Resolution**: Each query execution creates a new DI scope for proper lifetime management
- **Decorator Pattern**: Built-in support for cross-cutting concerns (logging, caching, pagination)

## Architecture

The library follows a layered architecture:

```
┌─────────────────────────────────────┐
│         Query Dispatcher            │
│    (IQueryDispatcher)               │
├─────────────────────────────────────┤
│         Query Handlers              │
│    (IQueryHandler<TQuery, TResult>) │
├─────────────────────────────────────┤
│         Query Objects               │
│    (IQuery<TResult>)                │
├─────────────────────────────────────┤
│         Decorators                  │
│    (Logging, Caching, Pagination)   │
└─────────────────────────────────────┘
```

## Core Components

### Query Interface
- **IQuery**: Base marker interface for all query objects
- **IQuery<TResult>**: Generic interface for queries with typed return values
- **Immutable**: Queries should be immutable using `record` types
- **Serializable**: Queries can be serialized for caching and API transport

### Query Handlers
- **IQueryHandler<TQuery, TResult>**: Generic interface for query processing
- **Type Safety**: Strongly-typed query and result parameters
- **Async Support**: Full async/await support with cancellation tokens
- **Single Responsibility**: Each handler processes one query type

### Query Dispatcher
- **IQueryDispatcher**: Centralized dispatching mechanism with two overloads
- **Scoped Resolution**: Creates new DI scope for each query execution
- **Reflection Support**: Dynamic handler resolution for complex scenarios
- **Error Handling**: Built-in error handling and logging

## Installation

### NuGet Package
```bash
dotnet add package Mamey.CQRS.Queries
```

### Prerequisites
- .NET 9.0 or later
- Mamey (core framework)

## Key Features

- **Query Pattern**: Clean separation of read operations from business logic
- **Type Safety**: Compile-time type safety for queries and results
- **Async Support**: Full async/await support for query handling
- **Dependency Injection**: Seamless integration with .NET DI container
- **Auto-Discovery**: Automatic registration of query handlers with dependency validation
- **Decorator Pattern**: Support for cross-cutting concerns
- **Cancellation Support**: Built-in cancellation token support
- **Pagination Support**: Built-in pagination decorator for paged queries

## Quick Start

### Basic Setup

```csharp
using Mamey;
using Mamey.CQRS.Queries;

var builder = WebApplication.CreateBuilder(args);

// Create Mamey builder
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Add query handlers and dispatcher
mameyBuilder
    .AddQueryHandlers()
    .AddInMemoryQueryDispatcher();

var app = builder.Build();
app.Run();
```

### Define Queries

```csharp
using Mamey.CQRS.Queries;

// Simple query with result
public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;

// Complex query with filtering
public record GetUsersByRoleQuery(string Role, int Page, int PageSize) : IQuery<PagedResult<UserDto>>;

// Query with multiple parameters
public record SearchProductsQuery(
    string? SearchTerm, 
    decimal? MinPrice, 
    decimal? MaxPrice, 
    string? Category,
    ProductSortOrder SortOrder) : IQuery<IEnumerable<ProductDto>>;

// Query with no parameters
public record GetAllCategoriesQuery() : IQuery<IEnumerable<CategoryDto>>;
```

### Implement Query Handlers

```csharp
using Mamey.CQRS.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user: {UserId}", query.UserId);
        
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(query.UserId);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}

public class GetUsersByRoleQueryHandler : IQueryHandler<GetUsersByRoleQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersByRoleQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersByRoleQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleAsync(
            query.Role, 
            query.Page, 
            query.PageSize, 
            cancellationToken);

        var totalCount = await _userRepository.CountByRoleAsync(query.Role, cancellationToken);

        return new PagedResult<UserDto>
        {
            Items = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            }),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }
}
```

### Dispatch Queries

```csharp
using Mamey.CQRS.Queries;

public class UserController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public UserController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var user = await _queryDispatcher.QueryAsync(query);
        return Ok(user);
    }

    [HttpGet("by-role/{role}")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsersByRole(
        string role, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersByRoleQuery(role, page, pageSize);
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }
}
```

## API Reference

### Core Interfaces

#### IQuery

Base marker interface for all queries.

```csharp
public interface IQuery
{
}
```

#### IQuery<TResult>

Generic interface for queries with typed return values.

```csharp
public interface IQuery<TResult> : IQuery
{
}
```

**Usage:**
```csharp
public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
public record GetAllUsersQuery() : IQuery<IEnumerable<UserDto>>;
public record GetUsersByRoleQuery(string Role) : IQuery<PagedResult<UserDto>>;
```

#### IQueryHandler<TQuery, TResult>

Interface for query handlers with strongly-typed parameters.

```csharp
public interface IQueryHandler<in TQuery, TResult> where TQuery : class, IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

**Generic Parameters:**
- `TQuery`: The type of query to handle (must implement `IQuery<TResult>`)
- `TResult`: The type of result to return

**Methods:**
- `HandleAsync(TQuery query, CancellationToken cancellationToken = default)`: Handles the query asynchronously and returns the result

**Implementation Requirements:**
- **Single Responsibility**: Each handler should process only one query type
- **Async Operations**: Use async/await for all I/O operations
- **Error Handling**: Handle exceptions appropriately and let business exceptions bubble up
- **Performance**: Optimize queries for read performance
- **Caching**: Consider implementing caching for expensive queries

**Lifetime Management:**
- Handlers are registered with `Transient` lifetime by default
- Each query execution gets a new handler instance
- Use `AddScopedQueryHandlers()` for scoped lifetime if needed

#### IQueryDispatcher

Interface for query dispatching with two overloads for different scenarios.

```csharp
public interface IQueryDispatcher
{
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>;
}
```

**Methods:**
- `QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)`: Dispatches a query using reflection (useful for dynamic scenarios)
- `QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)`: Dispatches a strongly-typed query (preferred for performance)

### Core Classes

#### QueryDispatcher

Default implementation of query dispatcher with both reflection-based and strongly-typed dispatching.

```csharp
internal sealed class QueryDispatcher : IQueryDispatcher
{
    public QueryDispatcher(IServiceProvider serviceProvider);
    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : class, IQuery<TResult>;
}
```

**Features:**
- **Scoped Resolution**: Creates a new scope for each query
- **Handler Resolution**: Automatically resolves the appropriate handler
- **Reflection Support**: Dynamic handler resolution for complex scenarios
- **Error Handling**: Built-in error handling and logging

### Extension Methods

#### AddQueryHandlers

Registers query handlers with transient lifetime and dependency validation.

```csharp
public static IMameyBuilder AddQueryHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
```

**Parameters:**
- `builder`: The Mamey builder
- `assemblies`: Optional assemblies to scan (defaults to current domain assemblies)

**Returns:**
- `IMameyBuilder`: The builder for chaining

**Features:**
- **Dependency Validation**: Validates that all handler dependencies are registered
- **Diagnostic Output**: Provides detailed information about handler registration
- **Constructor Analysis**: Analyzes constructor parameters for dependency resolution

#### AddScopedQueryHandlers

Registers query handlers with scoped lifetime.

```csharp
public static IMameyBuilder AddScopedQueryHandlers(this IMameyBuilder builder, IEnumerable<Assembly>? assemblies = null)
```

#### AddInMemoryQueryDispatcher

Registers the in-memory query dispatcher.

```csharp
public static IMameyBuilder AddInMemoryQueryDispatcher(this IMameyBuilder builder)
```

#### AddPagedQueryDecorator

Registers the pagination decorator for paged queries.

```csharp
public static IServiceCollection AddPagedQueryDecorator(this IServiceCollection services)
```

## Usage Examples

### Example 1: Simple Query Handler

```csharp
using Mamey.CQRS.Queries;

public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid ProductId { get; init; }
}

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product == null)
            throw new ProductNotFoundException(query.ProductId);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category.Name
        };
    }
}

// Usage
public class ProductController : ControllerBase
{
    private readonly IQueryDispatcher _dispatcher;

    public ProductController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery { ProductId = id };
        var product = await _dispatcher.QueryAsync(query);
        return Ok(product);
    }
}
```

### Example 2: Complex Query with Filtering and Sorting

```csharp
using Mamey.CQRS.Queries;

public class SearchProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string? SearchTerm { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Category { get; init; }
    public ProductSortOrder SortOrder { get; init; } = ProductSortOrder.Name;
    public int Limit { get; init; } = 50;
}

public enum ProductSortOrder
{
    Name,
    Price,
    CreatedAt,
    Popularity
}

public class SearchProductsQueryHandler : IQueryHandler<SearchProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        IProductRepository repository,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> HandleAsync(SearchProductsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching products with term: {SearchTerm}, category: {Category}", 
            query.SearchTerm, query.Category);

        var products = await _repository.SearchAsync(
            searchTerm: query.SearchTerm,
            minPrice: query.MinPrice,
            maxPrice: query.MaxPrice,
            category: query.Category,
            sortOrder: query.SortOrder,
            limit: query.Limit,
            cancellationToken: cancellationToken);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Category = p.Category.Name,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.Stock > 0
        });
    }
}
```

### Example 3: Paged Query with Decorator

```csharp
using Mamey.CQRS.Queries;

public class GetUsersQuery : IQuery<PagedResult<UserDto>>, IPagedQuery
{
    public string? SearchTerm { get; init; }
    public string? Role { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetUsersAsync(
            searchTerm: query.SearchTerm,
            role: query.Role,
            isActive: query.IsActive,
            page: query.Page,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken);

        var totalCount = await _repository.CountUsersAsync(
            searchTerm: query.SearchTerm,
            role: query.Role,
            isActive: query.IsActive,
            cancellationToken: cancellationToken);

        return new PagedResult<UserDto>
        {
            Items = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }
}

// Register with pagination decorator
mameyBuilder
    .AddQueryHandlers()
    .AddInMemoryQueryDispatcher();

builder.Services.AddPagedQueryDecorator();
```

### Example 4: Query with Caching

```csharp
using Mamey.CQRS.Queries;

public class GetCategoriesQuery : IQuery<IEnumerable<CategoryDto>>
{
}

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<GetCategoriesQueryHandler> _logger;

    public GetCategoriesQueryHandler(
        ICategoryRepository repository,
        ICacheService cache,
        ILogger<GetCategoriesQueryHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        const string cacheKey = "categories:all";
        
        // Try to get from cache first
        var cachedCategories = await _cache.GetAsync<IEnumerable<CategoryDto>>(cacheKey, cancellationToken);
        if (cachedCategories != null)
        {
            _logger.LogDebug("Categories retrieved from cache");
            return cachedCategories;
        }

        _logger.LogDebug("Categories not in cache, fetching from database");
        
        // Fetch from database
        var categories = await _repository.GetAllAsync(cancellationToken);
        var categoryDtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ProductCount = c.Products.Count
        });

        // Cache for 1 hour
        await _cache.SetAsync(cacheKey, categoryDtos, TimeSpan.FromHours(1), cancellationToken);
        
        return categoryDtos;
    }
}
```

### Example 5: Query Decorator

```csharp
using Mamey.CQRS.Queries;
using Mamey.Types;

[Decorator]
public class LoggingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> 
    where TQuery : class, IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> _logger;

    public LoggingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> handler,
        ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var queryName = typeof(TQuery).Name;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("Executing query: {QueryName}", queryName);

        try
        {
            var result = await _handler.HandleAsync(query, cancellationToken);
            stopwatch.Stop();
            
            _logger.LogInformation("Query executed successfully: {QueryName} in {ElapsedMs}ms", 
                queryName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Query failed: {QueryName} after {ElapsedMs}ms", 
                queryName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

## Integration Patterns

### Integration with Other Mamey Libraries

The CQRS Queries library integrates seamlessly with other Mamey libraries:

- **Mamey.CQRS.Commands**: Queries complement commands for read operations
- **Mamey.CQRS.Events**: Queries can be triggered by events for read model updates
- **Mamey.Persistence**: Queries interact with repositories for data access
- **Mamey.Logging**: Built-in logging support for query handlers

### Integration with ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);
mameyBuilder
    .AddQueryHandlers()
    .AddInMemoryQueryDispatcher();

var app = builder.Build();

// Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public ProductsController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? category)
    {
        var query = new SearchProductsQuery
        {
            SearchTerm = searchTerm,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Category = category
        };

        var products = await _queryDispatcher.QueryAsync(query);
        return Ok(products);
    }
}
```

## Configuration Reference

### Service Registration

```csharp
// Register query handlers
mameyBuilder.AddQueryHandlers();

// Register with specific assemblies
mameyBuilder.AddQueryHandlers(new[] { typeof(Program).Assembly });

// Register with scoped lifetime
mameyBuilder.AddScopedQueryHandlers();

// Register query dispatcher
mameyBuilder.AddInMemoryQueryDispatcher();

// Add pagination decorator
builder.Services.AddPagedQueryDecorator();
```

### Custom Query Dispatcher

```csharp
// Register custom dispatcher
builder.Services.AddSingleton<IQueryDispatcher, CustomQueryDispatcher>();

public class CustomQueryDispatcher : IQueryDispatcher
{
    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        // Custom implementation
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : class, IQuery<TResult>
    {
        // Custom implementation
    }
}
```

## Best Practices

1. **Query Naming**: Use descriptive names ending with "Query" (e.g., `GetUserByIdQuery`)
2. **Single Responsibility**: Each query should represent a single read operation
3. **Immutable Queries**: Make queries immutable using `record` types
4. **Performance**: Optimize queries for read performance and consider caching
5. **Error Handling**: Handle errors appropriately in query handlers
6. **Logging**: Add logging for query execution and performance monitoring
7. **Testing**: Write unit tests for query handlers
8. **Async Operations**: Use async/await for all I/O operations
9. **Pagination**: Use the built-in pagination decorator for large result sets
10. **Caching**: Implement caching for expensive queries

## Troubleshooting

### Common Issues

**Handler Not Found**: Ensure the query handler is registered and implements `IQueryHandler<TQuery, TResult>`

**Dependency Missing**: Check the diagnostic output from `AddQueryHandlers()` for missing dependencies

**Type Mismatch**: Ensure the query implements `IQuery<TResult>` and the handler implements `IQueryHandler<TQuery, TResult>`

**Performance Issues**: Consider implementing caching or optimizing database queries

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

The `AddQueryHandlers()` method provides detailed diagnostic output about handler registration and dependency resolution.

## Related Libraries

- [Mamey.CQRS.Commands](cqrs-commands.md) - Command pattern implementation
- [Mamey.CQRS.Events](cqrs-events.md) - Event handling and dispatching
- [Mamey.Persistence.MongoDB](../persistence/persistence-mongodb.md) - MongoDB persistence
- [Mamey.Persistence.SQL](../persistence/persistence-sql.md) - SQL persistence

## Additional Resources

- CQRS Pattern Guide
- Query Optimization
- Caching Strategies
- Testing Queries
