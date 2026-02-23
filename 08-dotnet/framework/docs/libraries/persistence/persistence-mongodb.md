# Mamey.Persistence.MongoDB

A comprehensive MongoDB persistence library for the Mamey framework, providing seamless integration with MongoDB databases. This library offers a clean, type-safe approach to data persistence with built-in support for repositories, pagination, and advanced querying capabilities.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Components](#core-components)
- [API Reference](#api-reference)
- [Usage Examples](#usage-examples)
- [Configuration](#configuration)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Overview

Mamey.Persistence.MongoDB is a specialized persistence library that provides MongoDB integration for the Mamey framework. It offers a repository pattern implementation with advanced querying capabilities, pagination support, and seamless integration with CQRS patterns.

### Technical Overview

The library provides:

- **Repository Pattern**: Clean abstraction over MongoDB operations
- **Type Safety**: Full compile-time type safety for all operations
- **Pagination Support**: Built-in pagination for large datasets
- **Query Building**: Expression-based query building
- **Convention Registration**: Automatic MongoDB convention registration
- **Seeder Support**: Built-in data seeding capabilities
- **Session Management**: Advanced session and transaction management

## Key Features

### Core Features

- **Repository Pattern**: Generic repository implementation for MongoDB collections
- **Type Safety**: Full compile-time type safety for all database operations
- **Pagination**: Built-in pagination support with `PagedResult<T>`
- **Query Building**: Expression-based query building for complex queries
- **Convention Registration**: Automatic MongoDB convention registration
- **Seeder Support**: Built-in data seeding capabilities
- **Session Management**: Advanced session and transaction management

### Advanced Features

- **Custom Conventions**: Support for custom MongoDB conventions
- **Random Database Suffix**: Support for integration testing with random database names
- **Async Operations**: Full async/await support for all operations
- **Cancellation Support**: Built-in cancellation token support
- **Error Handling**: Comprehensive error handling and logging
- **Performance Optimization**: Optimized queries and connection management

## Architecture

```mermaid
graph TB
    subgraph "Mamey.Persistence.MongoDB Architecture"
        A[Application] --> B[MongoRepository]
        B --> C[MongoDB Driver]
        C --> D[MongoDB Database]
        
        E[MongoDbOptions] --> F[Connection Configuration]
        F --> C
        
        G[Conventions] --> H[Serialization Rules]
        H --> C
        
        I[Seeder] --> J[Data Initialization]
        J --> D
        
        K[Session Factory] --> L[Transaction Management]
        L --> C
        
        M[Pagination] --> N[PagedResult]
        N --> B
    end
    
    B --> O[Query Building]
    B --> P[CRUD Operations]
    B --> Q[Aggregation]
```

## Installation

### Package Manager
```bash
Install-Package Mamey.Persistence.MongoDB
```

### .NET CLI
```bash
dotnet add package Mamey.Persistence.MongoDB
```

### PackageReference
```xml
<PackageReference Include="Mamey.Persistence.MongoDB" Version="2.0.*" />
```

## Quick Start

### Basic Setup

```csharp
using Mamey.Persistence.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
builder.Services.AddMamey()
    .AddMongo();

var app = builder.Build();

app.Run();
```

### With Custom Configuration

```csharp
builder.Services.AddMamey()
    .AddMongo(options =>
    {
        options.ConnectionString = "mongodb://localhost:27017";
        options.Database = "MyDatabase";
        options.Seed = true;
    });
```

### With Repository Registration

```csharp
builder.Services.AddMamey()
    .AddMongo()
    .AddMongoRepository<User, Guid>("users");
```

## Core Components

### Repository System

#### IMongoRepository Interface
```csharp
public interface IMongoRepository<TEntity, in TIdentifiable> 
    where TEntity : IIdentifiable<TIdentifiable>
{
    IMongoCollection<TEntity> Collection { get; }
    Task<TEntity> GetAsync(TIdentifiable id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) 
        where TQuery : IPagedQuery;
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(TIdentifiable id);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}
```

#### MongoRepository Implementation
```csharp
internal class MongoRepository<TEntity, TIdentifiable> : IMongoRepository<TEntity, TIdentifiable>
    where TEntity : IIdentifiable<TIdentifiable>
{
    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<TEntity>(collectionName);
    }

    public IMongoCollection<TEntity> Collection { get; }
    // Implementation details...
}
```

### Configuration System

#### MongoDbOptions
```csharp
public class MongoDbOptions
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public bool Seed { get; set; }
    public bool SetRandomDatabaseSuffix { get; set; }
}
```

#### MongoDbOptionsBuilder
```csharp
public interface IMongoDbOptionsBuilder
{
    IMongoDbOptionsBuilder WithConnectionString(string connectionString);
    IMongoDbOptionsBuilder WithDatabase(string database);
    IMongoDbOptionsBuilder WithSeed(bool seed);
    MongoDbOptions Build();
}
```

### Seeder System

#### IMongoDbSeeder
```csharp
public interface IMongoDbSeeder
{
    Task SeedAsync();
}
```

#### MongoDbSeeder
```csharp
internal class MongoDbSeeder : IMongoDbSeeder
{
    public async Task SeedAsync()
    {
        // Seeding implementation
    }
}
```

## API Reference

### Extension Methods

#### IMameyBuilder Extensions

```csharp
public static class MongoExtensions
{
    public static IMameyBuilder AddMongo(this IMameyBuilder builder, 
        string sectionName = "mongo", Type seederType = null, bool registerConventions = true);
    
    public static IMameyBuilder AddMongo(this IMameyBuilder builder, 
        Func<IMongoDbOptionsBuilder, IMongoDbOptionsBuilder> buildOptions, 
        Type seederType = null, bool registerConventions = true);
    
    public static IMameyBuilder AddMongo(this IMameyBuilder builder, 
        MongoDbOptions mongoOptions, Type seederType = null, bool registerConventions = true);
    
    public static IMameyBuilder AddMongoRepository<TEntity, TIdentifiable>(this IMameyBuilder builder, 
        string collectionName) where TEntity : IIdentifiable<TIdentifiable>;
}
```

### Repository Methods

```csharp
public interface IMongoRepository<TEntity, in TIdentifiable>
{
    // Get operations
    Task<TEntity> GetAsync(TIdentifiable id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    
    // Find operations
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) 
        where TQuery : IPagedQuery;
    
    // CRUD operations
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(TIdentifiable id);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    
    // Utility operations
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}
```

## Usage Examples

### Basic Repository Usage

```csharp
public class UserService
{
    private readonly IMongoRepository<User, Guid> _userRepository;

    public UserService(IMongoRepository<User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        return await _userRepository.GetAsync(id);
    }

    public async Task<IReadOnlyList<User>> GetActiveUsersAsync()
    {
        return await _userRepository.FindAsync(u => u.IsActive);
    }

    public async Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize)
    {
        var query = new GetUsersQuery { Page = page, PageSize = pageSize };
        return await _userRepository.BrowseAsync(u => true, query);
    }
}
```

### Custom Seeder Implementation

```csharp
public class CustomMongoDbSeeder : IMongoDbSeeder
{
    private readonly IMongoRepository<User, Guid> _userRepository;

    public CustomMongoDbSeeder(IMongoRepository<User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SeedAsync()
    {
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" },
            new User { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com" }
        };

        foreach (var user in users)
        {
            await _userRepository.AddAsync(user);
        }
    }
}

// Register custom seeder
builder.Services.AddMamey()
    .AddMongo(seederType: typeof(CustomMongoDbSeeder));
```

### Advanced Querying

```csharp
public class UserRepository
{
    private readonly IMongoRepository<User, Guid> _userRepository;

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role)
    {
        return await _userRepository.FindAsync(u => u.Roles.Contains(role));
    }

    public async Task<IReadOnlyList<User>> GetUsersCreatedAfterAsync(DateTime date)
    {
        return await _userRepository.FindAsync(u => u.CreatedAt > date);
    }

    public async Task<PagedResult<User>> SearchUsersAsync(string searchTerm, int page, int pageSize)
    {
        var query = new SearchUsersQuery 
        { 
            SearchTerm = searchTerm, 
            Page = page, 
            PageSize = pageSize 
        };
        
        return await _userRepository.BrowseAsync(
            u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm), 
            query);
    }
}
```

### Custom Conventions

```csharp
public class CustomMongoConventions
{
    public static void RegisterCustomConventions()
    {
        ConventionRegistry.Register("CustomConventions", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreIfNullConvention(true)
        }, _ => true);
    }
}

// Register custom conventions
builder.Services.AddMamey()
    .AddMongo(registerConventions: false);

// Then register custom conventions
CustomMongoConventions.RegisterCustomConventions();
```

## Configuration

### Basic Configuration

```csharp
builder.Services.AddMamey()
    .AddMongo();
```

### With Custom Options

```csharp
builder.Services.AddMamey()
    .AddMongo(options =>
    {
        options.ConnectionString = "mongodb://localhost:27017";
        options.Database = "MyDatabase";
        options.Seed = true;
        options.SetRandomDatabaseSuffix = false;
    });
```

### With Configuration Section

```csharp
// appsettings.json
{
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "MyDatabase",
    "Seed": true,
    "SetRandomDatabaseSuffix": false
  }
}

// Program.cs
builder.Services.AddMamey()
    .AddMongo("Mongo");
```

### Repository Registration

```csharp
builder.Services.AddMamey()
    .AddMongo()
    .AddMongoRepository<User, Guid>("users")
    .AddMongoRepository<Product, Guid>("products")
    .AddMongoRepository<Order, Guid>("orders");
```

## Best Practices

### Repository Design

1. **Single Responsibility**: Each repository should handle one entity type
2. **Async Operations**: Use async/await for all I/O operations
3. **Error Handling**: Implement proper error handling and logging
4. **Query Optimization**: Use appropriate indexes and query patterns

```csharp
public class UserRepository
{
    private readonly IMongoRepository<User, Guid> _userRepository;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IMongoRepository<User, Guid> userRepository, ILogger<UserRepository> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        try
        {
            return await _userRepository.GetAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<PagedResult<User>> GetUsersPagedAsync(GetUsersQuery query)
    {
        try
        {
            return await _userRepository.BrowseAsync(u => true, query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged users");
            throw;
        }
    }
}
```

### Query Optimization

1. **Use Indexes**: Create appropriate indexes for frequently queried fields
2. **Limit Results**: Use pagination for large datasets
3. **Project Fields**: Only select necessary fields when possible
4. **Use Aggregation**: Use aggregation pipelines for complex queries

```csharp
public class OptimizedUserRepository
{
    private readonly IMongoRepository<User, Guid> _userRepository;

    public async Task<PagedResult<User>> GetUsersOptimizedAsync(GetUsersQuery query)
    {
        // Use pagination to limit results
        return await _userRepository.BrowseAsync(u => u.IsActive, query);
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role)
    {
        // Use indexed field for better performance
        return await _userRepository.FindAsync(u => u.Roles.Contains(role));
    }
}
```

### Error Handling

1. **Logging**: Log all errors with appropriate context
2. **Exception Handling**: Handle specific MongoDB exceptions
3. **Retry Logic**: Implement retry logic for transient failures
4. **Validation**: Validate data before database operations

```csharp
public class UserService
{
    private readonly IMongoRepository<User, Guid> _userRepository;
    private readonly ILogger<UserService> _logger;

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return user;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Code == 11000)
        {
            _logger.LogWarning("User with email {Email} already exists", request.Email);
            throw new DuplicateEmailException(request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", request.Email);
            throw;
        }
    }
}
```

## Troubleshooting

### Common Issues

#### 1. Connection String Issues

**Problem**: MongoDB connection fails.

**Solution**: Ensure proper connection string format and MongoDB server is running.

```csharp
// Correct connection string format
var connectionString = "mongodb://localhost:27017";
// Or with authentication
var connectionString = "mongodb://username:password@localhost:27017/database";
```

#### 2. Collection Not Found

**Problem**: Collection doesn't exist when querying.

**Solution**: Ensure collection is created or use automatic collection creation.

```csharp
// MongoDB automatically creates collections on first write
await _userRepository.AddAsync(user);
```

#### 3. Serialization Issues

**Problem**: Object serialization/deserialization fails.

**Solution**: Ensure proper BSON serialization conventions are registered.

```csharp
// Register conventions
builder.Services.AddMamey()
    .AddMongo(registerConventions: true);
```

#### 4. Performance Issues

**Problem**: Queries are slow.

**Solution**: Create appropriate indexes and optimize queries.

```csharp
// Create indexes for frequently queried fields
await collection.Indexes.CreateOneAsync(
    new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email)));
```

### Performance Considerations

1. **Connection Pooling**: Use connection pooling for better performance
2. **Indexing**: Create appropriate indexes for query patterns
3. **Pagination**: Use pagination for large datasets
4. **Caching**: Implement caching for frequently accessed data

```csharp
public class CachedUserRepository
{
    private readonly IMongoRepository<User, Guid> _userRepository;
    private readonly IMemoryCache _cache;

    public async Task<User> GetUserAsync(Guid id)
    {
        var cacheKey = $"user_{id}";
        
        if (_cache.TryGetValue(cacheKey, out User cachedUser))
        {
            return cachedUser;
        }

        var user = await _userRepository.GetAsync(id);
        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        
        return user;
    }
}
```

### Debugging Tips

1. **Enable Logging**: Use structured logging for better debugging
2. **Query Analysis**: Use MongoDB query analysis tools
3. **Connection Monitoring**: Monitor connection pool usage
4. **Performance Profiling**: Use performance profiling tools

```csharp
public class UserRepository
{
    private readonly IMongoRepository<User, Guid> _userRepository;
    private readonly ILogger<UserRepository> _logger;

    public async Task<User> GetUserAsync(Guid id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);
        
        var stopwatch = Stopwatch.StartNew();
        var user = await _userRepository.GetAsync(id);
        stopwatch.Stop();
        
        _logger.LogInformation("User retrieved in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        
        return user;
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Please read Contributing Guide for details on our code of conduct and the process for submitting pull requests.

## Support

For support and questions, please open an issue in the [GitHub repository](https://github.com/mamey-io/mamey-persistence-mongodb/issues).
