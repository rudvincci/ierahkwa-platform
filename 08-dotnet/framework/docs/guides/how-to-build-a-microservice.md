# Complete Guide: Building a Production-Ready Microservice with Mamey Framework

This comprehensive guide will walk you through building a complete, production-ready microservice using the Mamey .NET framework. We'll build a Products microservice for an e-commerce platform, covering everything from initial setup to deployment.

## Table of Contents

1. [Introduction & Prerequisites](#introduction--prerequisites)
2. [Project Setup & Structure](#project-setup--structure)
3. [Domain Layer](#domain-layer)
4. [Application Layer - CQRS Implementation](#application-layer---cqrs-implementation)
5. [Infrastructure Layer](#infrastructure-layer)
6. [API Layer](#api-layer)
7. [Authentication & Authorization](#authentication--authorization)
8. [Observability](#observability)
9. [Message Brokers & Event-Driven Architecture](#message-brokers--event-driven-architecture)
10. [Persistence Patterns](#persistence-patterns)
11. [Service Discovery & Load Balancing](#service-discovery--load-balancing)
12. [API Gateway Integration](#api-gateway-integration)
13. [Testing](#testing)
14. [Docker & Containerization](#docker--containerization)
15. [Deployment](#deployment)
16. [Production Considerations](#production-considerations)
17. [Complete Working Example](#complete-working-example)
18. [Best Practices & Common Patterns](#best-practices--common-patterns)
19. [Troubleshooting Guide](#troubleshooting-guide)
20. [Next Steps & Resources](#next-steps--resources)

---

## Introduction & Prerequisites

### What is Mamey Framework?

Mamey is a comprehensive .NET 9.0 microservices framework designed to simplify the development of scalable, maintainable microservices applications. It provides:

- **Modular Architecture**: Pluggable components for maximum flexibility
- **CQRS Support**: Built-in Command Query Responsibility Segregation
- **Message Brokers**: Support for RabbitMQ, Kafka, Azure Service Bus
- **Authentication**: JWT, Azure AD (B2B/B2C), and custom authentication
- **Observability**: Comprehensive logging, tracing, and metrics
- **Persistence**: MongoDB, Redis, SQL Server, PostgreSQL, MySQL support
- **Infrastructure**: Service discovery, load balancing, API gateway

### Prerequisites

Before starting, ensure you have:

- **.NET 9.0 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **Docker Desktop** (for containerized services)
- **Git** (for version control)
- **Postman** or **REST Client** (for API testing)

### Knowledge Requirements

This guide assumes you have:
- Intermediate knowledge of C# and .NET
- Understanding of microservices architecture concepts
- Familiarity with REST APIs and HTTP
- Basic knowledge of databases and messaging

### Our Example: Products Microservice

We'll build a Products microservice for an e-commerce platform that handles:
- Product catalog management (CRUD operations)
- Product search and filtering
- Inventory tracking
- Price management
- Category management
- Integration with other services (Inventory, Pricing, Notifications)

---

## Project Setup & Structure

### Creating the Project

First, let's create our microservice using the Mamey template:

```bash
# Create the microservice project
dotnet new mamey-srv -n Mamey.Products -c Mamey -ap ECommerce -s Products -e Product -p 5001

# Navigate to the project directory
cd Mamey.Products
```

This creates a complete project structure with all necessary layers:

```
Mamey.Products/
├── src/
│   ├── Mamey.Products.Api/           # Web API layer
│   ├── Mamey.Products.Application/   # Application layer (CQRS)
│   ├── Mamey.Products.Domain/        # Domain layer
│   ├── Mamey.Products.Infrastructure/ # Infrastructure layer
│   ├── Mamey.Products.Contracts/     # Shared contracts
│   ├── Mamey.Products.BlazorWasm/    # Blazor WebAssembly client
│   ├── Mamey.Products.Maui/          # MAUI mobile client
│   └── Mamey.Products.Net/           # .NET client library
├── tests/
│   ├── Mamey.Products.Tests.Unit/
│   ├── Mamey.Products.Tests.Integration/
│   ├── Mamey.Products.Tests.EndToEnd/
│   └── Mamey.Products.Tests.Performance/
├── scripts/
│   ├── build.sh
│   ├── start.sh
│   ├── test.sh
│   └── watch.sh
├── docker-compose.yml
├── docker-compose.development.yml
├── Dockerfile
└── README.md
```

### Understanding the Layered Architecture

Our microservice follows Clean Architecture principles:

```mermaid
graph TB
    subgraph "API Layer"
        A[Controllers] --> B[DTOs]
        A --> C[Middleware]
    end
    
    subgraph "Application Layer"
        D[Commands] --> E[Command Handlers]
        F[Queries] --> G[Query Handlers]
        H[Events] --> I[Event Handlers]
    end
    
    subgraph "Domain Layer"
        J[Entities] --> K[Value Objects]
        L[Domain Events] --> M[Domain Services]
        N[Repository Interfaces]
    end
    
    subgraph "Infrastructure Layer"
        O[Repository Implementations] --> P[Database]
        Q[Message Brokers] --> R[External Services]
        S[HTTP Clients] --> T[Third-party APIs]
    end
    
    A --> D
    A --> F
    E --> J
    G --> J
    I --> L
    E --> Q
    O --> N
```

### Solution Structure

The solution file (`Mamey.Products.sln`) organizes our projects:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="src\Mamey.Products.Api\Mamey.Products.Api.csproj" />
    <ProjectReference Include="src\Mamey.Products.Application\Mamey.Products.Application.csproj" />
    <ProjectReference Include="src\Mamey.Products.Domain\Mamey.Products.Domain.csproj" />
    <ProjectReference Include="src\Mamey.Products.Infrastructure\Mamey.Products.Infrastructure.csproj" />
    <ProjectReference Include="src\Mamey.Products.Contracts\Mamey.Products.Contracts.csproj" />
  </ItemGroup>
</Project>
```

### Configuration Management

Let's set up our configuration files:

**appsettings.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Mamey": "Debug"
    }
  },
  "AllowedHosts": "*",
  "mamey": {
    "name": "Mamey.Products",
    "version": "1.0.0"
  },
  "mongo": {
    "connectionString": "mongodb://localhost:27017",
    "database": "ecommerce_products"
  },
  "redis": {
    "connectionString": "localhost:6379"
  },
  "rabbitmq": {
    "connectionString": "amqp://guest:guest@localhost:5672",
    "exchange": "ecommerce.products",
    "queue": "ecommerce.products.queue"
  },
  "jwt": {
    "secretKey": "your-secret-key-here-must-be-at-least-32-characters-long",
    "issuer": "Mamey.Products",
    "audience": "Mamey.Products",
    "expiryMinutes": 60
  },
  "consul": {
    "enabled": true,
    "url": "http://localhost:8500"
  },
  "jaeger": {
    "enabled": true,
    "agentHost": "localhost",
    "agentPort": 6831
  }
}
```

**appsettings.Development.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "mongo": {
    "connectionString": "mongodb://localhost:27017",
    "database": "ecommerce_products_dev"
  }
}
```

---

## Domain Layer

The domain layer contains the core business logic and entities. Let's start by creating our domain models.

### Domain Entities

**Product.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Entities;

public class Product : AggregateRoot
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public SKU Sku { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public ProductStatus Status { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    private Product() { } // For EF Core

    public Product(
        ProductId id,
        string name,
        string description,
        Money price,
        SKU sku,
        CategoryId categoryId,
        string createdBy)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        CategoryId = categoryId;
        Status = ProductStatus.Draft;
        StockQuantity = 0;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price.Amount, Sku.Value));
    }

    public void UpdateDetails(string name, string description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Description));
    }

    public void UpdatePrice(Money newPrice, string updatedBy)
    {
        if (newPrice.Amount <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice.Amount, newPrice.Amount));
    }

    public void UpdateStock(int quantity, string updatedBy)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        var oldQuantity = StockQuantity;
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductStockUpdatedEvent(Id, oldQuantity, quantity));
    }

    public void Activate(string updatedBy)
    {
        if (Status == ProductStatus.Active)
            return;

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductActivatedEvent(Id, Name));
    }

    public void Deactivate(string updatedBy)
    {
        if (Status == ProductStatus.Inactive)
            return;

        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductDeactivatedEvent(Id, Name));
    }

    public void Delete(string deletedBy)
    {
        if (Status == ProductStatus.Deleted)
            return;

        Status = ProductStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;

        AddDomainEvent(new ProductDeletedEvent(Id, Name));
    }
}
```

**Category.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Entities;

public class Category : AggregateRoot
{
    public CategoryId Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CategoryId? ParentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Category() { } // For EF Core

    public Category(CategoryId id, string name, string description, CategoryId? parentId = null)
    {
        Id = id;
        Name = name;
        Description = description;
        ParentId = parentId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryCreatedEvent(Id, Name, ParentId));
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryUpdatedEvent(Id, Name, Description));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryDeactivatedEvent(Id, Name));
    }
}
```

### Value Objects

**ProductId.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.ValueObjects;

public record ProductId(Guid Value) : IIdentifiable<Guid>
{
    public static ProductId New() => new(Guid.NewGuid());
    public static implicit operator Guid(ProductId id) => id.Value;
    public static implicit operator ProductId(Guid value) => new(value);
}
```

**CategoryId.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.ValueObjects;

public record CategoryId(Guid Value) : IIdentifiable<Guid>
{
    public static CategoryId New() => new(Guid.NewGuid());
    public static implicit operator Guid(CategoryId id) => id.Value;
    public static implicit operator CategoryId(Guid value) => new(value);
}
```

**Money.cs**:
```csharp
namespace Mamey.Products.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "USD")
{
    public static Money Zero => new(0);
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Cannot divide by zero");
        
        return new Money(money.Amount / divisor, money.Currency);
    }
}
```

**SKU.cs**:
```csharp
namespace Mamey.Products.Domain.ValueObjects;

public record SKU(string Value)
{
    public static implicit operator string(SKU sku) => sku.Value;
    public static implicit operator SKU(string value) => new(value);

    public static SKU Generate(string prefix = "PRD")
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        return new SKU($"{prefix}-{timestamp}-{random}");
    }
}
```

### Enums

**ProductStatus.cs**:
```csharp
namespace Mamey.Products.Domain.Enums;

public enum ProductStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    Discontinued = 3,
    Deleted = 4
}
```

### Domain Events

**ProductCreatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductCreatedEvent(
    ProductId ProductId,
    string Name,
    decimal Price,
    string Sku) : IDomainEvent;
```

**ProductUpdatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductUpdatedEvent(
    ProductId ProductId,
    string Name,
    string Description) : IDomainEvent;
```

**ProductPriceChangedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductPriceChangedEvent(
    ProductId ProductId,
    decimal OldPrice,
    decimal NewPrice) : IDomainEvent;
```

**ProductStockUpdatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductStockUpdatedEvent(
    ProductId ProductId,
    int OldQuantity,
    int NewQuantity) : IDomainEvent;
```

**ProductActivatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductActivatedEvent(
    ProductId ProductId,
    string Name) : IDomainEvent;
```

**ProductDeactivatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductDeactivatedEvent(
    ProductId ProductId,
    string Name) : IDomainEvent;
```

**ProductDeletedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record ProductDeletedEvent(
    ProductId ProductId,
    string Name) : IDomainEvent;
```

**CategoryCreatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record CategoryCreatedEvent(
    CategoryId CategoryId,
    string Name,
    CategoryId? ParentId) : IDomainEvent;
```

**CategoryUpdatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record CategoryUpdatedEvent(
    CategoryId CategoryId,
    string Name,
    string Description) : IDomainEvent;
```

**CategoryDeactivatedEvent.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Events;

public record CategoryDeactivatedEvent(
    CategoryId CategoryId,
    string Name) : IDomainEvent;
```

### Domain Exceptions

**ProductNotFoundException.cs**:
```csharp
namespace Mamey.Products.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductId ProductId { get; }

    public ProductNotFoundException(ProductId productId)
        : base($"Product with ID {productId} was not found.")
    {
        ProductId = productId;
    }
}
```

**CategoryNotFoundException.cs**:
```csharp
namespace Mamey.Products.Domain.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryId CategoryId { get; }

    public CategoryNotFoundException(CategoryId categoryId)
        : base($"Category with ID {categoryId} was not found.")
    {
        CategoryId = categoryId;
    }
}
```

**InvalidProductOperationException.cs**:
```csharp
namespace Mamey.Products.Domain.Exceptions;

public class InvalidProductOperationException : Exception
{
    public ProductId ProductId { get; }
    public string Operation { get; }

    public InvalidProductOperationException(ProductId productId, string operation, string message)
        : base(message)
    {
        ProductId = productId;
        Operation = operation;
    }
}
```

### Repository Interfaces

**IProductRepository.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Repositories;

public interface IProductRepository : IRepository<Product, ProductId>
{
    Task<Product?> GetBySkuAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(SKU sku, ProductId excludeId, CancellationToken cancellationToken = default);
}
```

**ICategoryRepository.cs**:
```csharp
using Mamey.Core.Domain;

namespace Mamey.Products.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByParentIdAsync(CategoryId? parentId, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
}
```

---

## Application Layer - CQRS Implementation

Now let's implement the CQRS pattern with commands, queries, and event handlers.

### Commands

**CreateProductCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string Sku,
    Guid CategoryId,
    string CreatedBy) : ICommand;
```

**UpdateProductCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Description,
    string UpdatedBy) : ICommand;
```

**UpdateProductPriceCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record UpdateProductPriceCommand(
    Guid ProductId,
    decimal Price,
    string Currency,
    string UpdatedBy) : ICommand;
```

**UpdateProductStockCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record UpdateProductStockCommand(
    Guid ProductId,
    int Quantity,
    string UpdatedBy) : ICommand;
```

**ActivateProductCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record ActivateProductCommand(
    Guid ProductId,
    string UpdatedBy) : ICommand;
```

**DeactivateProductCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record DeactivateProductCommand(
    Guid ProductId,
    string UpdatedBy) : ICommand;
```

**DeleteProductCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record DeleteProductCommand(
    Guid ProductId,
    string DeletedBy) : ICommand;
```

**CreateCategoryCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record CreateCategoryCommand(
    string Name,
    string Description,
    Guid? ParentId) : ICommand;
```

**UpdateCategoryCommand.cs**:
```csharp
using Mamey.CQRS.Commands;

namespace Mamey.Products.Application.Commands;

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Description) : ICommand;
```

### Command Handlers

**CreateProductHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Entities;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class CreateProductHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILogger<CreateProductHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating product {ProductName} with SKU {Sku}", command.Name, command.Sku);

        // Validate category exists
        var categoryId = new CategoryId(command.CategoryId);
        var category = await _categoryRepository.GetAsync(categoryId, cancellationToken);
        if (category == null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        // Check if SKU already exists
        var sku = new SKU(command.Sku);
        if (await _productRepository.ExistsBySkuAsync(sku, cancellationToken))
        {
            throw new InvalidOperationException($"Product with SKU {command.Sku} already exists");
        }

        // Create product
        var product = new Product(
            ProductId.New(),
            command.Name,
            command.Description,
            new Money(command.Price, command.Currency),
            sku,
            categoryId,
            command.CreatedBy);

        await _productRepository.AddAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} created successfully", product.Id);
    }
}
```

**UpdateProductHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class UpdateProductHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating product {ProductId}", command.ProductId);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.UpdateDetails(command.Name, command.Description, command.UpdatedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} updated successfully", product.Id);
    }
}
```

**UpdateProductPriceHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class UpdateProductPriceHandler : ICommandHandler<UpdateProductPriceCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductPriceHandler> _logger;

    public UpdateProductPriceHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductPriceHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateProductPriceCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating price for product {ProductId}", command.ProductId);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        var newPrice = new Money(command.Price, command.Currency);
        product.UpdatePrice(newPrice, command.UpdatedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Price updated for product {ProductId} to {Price}", product.Id, newPrice.Amount);
    }
}
```

**UpdateProductStockHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class UpdateProductStockHandler : ICommandHandler<UpdateProductStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductStockHandler> _logger;

    public UpdateProductStockHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductStockHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateProductStockCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating stock for product {ProductId} to {Quantity}", command.ProductId, command.Quantity);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.UpdateStock(command.Quantity, command.UpdatedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Stock updated for product {ProductId} to {Quantity}", product.Id, command.Quantity);
    }
}
```

**ActivateProductHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class ActivateProductHandler : ICommandHandler<ActivateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ActivateProductHandler> _logger;

    public ActivateProductHandler(
        IProductRepository productRepository,
        ILogger<ActivateProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ActivateProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating product {ProductId}", command.ProductId);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Activate(command.UpdatedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} activated successfully", product.Id);
    }
}
```

**DeactivateProductHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class DeactivateProductHandler : ICommandHandler<DeactivateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeactivateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ILogger<DeactivateProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeactivateProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating product {ProductId}", command.ProductId);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Deactivate(command.UpdatedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} deactivated successfully", product.Id);
    }
}
```

**DeleteProductHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class DeleteProductHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IProductRepository productRepository,
        ILogger<DeleteProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting product {ProductId}", command.ProductId);

        var productId = new ProductId(command.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Delete(command.DeletedBy);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Product {ProductId} deleted successfully", product.Id);
    }
}
```

**CreateCategoryHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Entities;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Commands;

public class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(
        ICategoryRepository categoryRepository,
        ILogger<CreateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category {CategoryName}", command.Name);

        var category = new Category(
            CategoryId.New(),
            command.Name,
            command.Description,
            command.ParentId.HasValue ? new CategoryId(command.ParentId.Value) : null);

        await _categoryRepository.AddAsync(category, cancellationToken);

        _logger.LogInformation("Category {CategoryId} created successfully", category.Id);
    }
}
```

**UpdateCategoryHandler.cs**:
```csharp
using Mamey.CQRS.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Exceptions;

namespace Mamey.Products.Application.Commands;

public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository,
        ILogger<UpdateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category {CategoryId}", command.CategoryId);

        var categoryId = new CategoryId(command.CategoryId);
        var category = await _categoryRepository.GetAsync(categoryId, cancellationToken);
        if (category == null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        category.UpdateDetails(command.Name, command.Description);

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        _logger.LogInformation("Category {CategoryId} updated successfully", category.Id);
    }
}
```

### Queries

**GetProductQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record GetProductQuery(Guid ProductId) : IQuery<ProductDto?>;
```

**GetProductsQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    string? Status = null,
    string? SortBy = "Name",
    string? SortDirection = "asc") : IQuery<PagedResult<ProductDto>>;
```

**SearchProductsQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record SearchProductsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 20,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null) : IQuery<PagedResult<ProductDto>>;
```

**GetProductsByCategoryQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record GetProductsByCategoryQuery(
    Guid CategoryId,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ProductDto>>;
```

**GetCategoriesQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record GetCategoriesQuery(
    bool ActiveOnly = true,
    Guid? ParentId = null) : IQuery<IEnumerable<CategoryDto>>;
```

**GetCategoryQuery.cs**:
```csharp
using Mamey.CQRS.Queries;

namespace Mamey.Products.Application.Queries;

public record GetCategoryQuery(Guid CategoryId) : IQuery<CategoryDto?>;
```

### Query Handlers

**GetProductHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Queries;

public class GetProductHandler : IQueryHandler<GetProductQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductHandler> _logger;

    public GetProductHandler(
        IProductRepository productRepository,
        ILogger<GetProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting product {ProductId}", query.ProductId);

        var productId = new ProductId(query.ProductId);
        var product = await _productRepository.GetAsync(productId, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", query.ProductId);
            return null;
        }

        return new ProductDto
        {
            Id = product.Id.Value,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Sku = product.Sku.Value,
            CategoryId = product.CategoryId.Value,
            Status = product.Status.ToString(),
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            CreatedBy = product.CreatedBy,
            UpdatedBy = product.UpdatedBy
        };
    }
}
```

**GetProductsHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using Mamey.Products.Domain.Enums;

namespace Mamey.Products.Application.Queries;

public class GetProductsHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsHandler> _logger;

    public GetProductsHandler(
        IProductRepository productRepository,
        ILogger<GetProductsHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting products with filters: Page={Page}, PageSize={PageSize}, SearchTerm={SearchTerm}, CategoryId={CategoryId}, Status={Status}",
            query.Page, query.PageSize, query.SearchTerm, query.CategoryId, query.Status);

        var products = await _productRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            products = products.Where(p => 
                p.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Value.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (query.CategoryId.HasValue)
        {
            var categoryId = new CategoryId(query.CategoryId.Value);
            products = products.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<ProductStatus>(query.Status, out var status))
        {
            products = products.Where(p => p.Status == status);
        }

        // Apply sorting
        products = query.SortBy.ToLower() switch
        {
            "name" => query.SortDirection.ToLower() == "desc" 
                ? products.OrderByDescending(p => p.Name)
                : products.OrderBy(p => p.Name),
            "price" => query.SortDirection.ToLower() == "desc"
                ? products.OrderByDescending(p => p.Price.Amount)
                : products.OrderBy(p => p.Price.Amount),
            "createdat" => query.SortDirection.ToLower() == "desc"
                ? products.OrderByDescending(p => p.CreatedAt)
                : products.OrderBy(p => p.CreatedAt),
            _ => products.OrderBy(p => p.Name)
        };

        var totalCount = products.Count();
        var pagedProducts = products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                Sku = p.Sku.Value,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy
            })
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = pagedProducts,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }
}
```

**SearchProductsHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Queries;

public class SearchProductsHandler : IQueryHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SearchProductsHandler> _logger;

    public SearchProductsHandler(
        IProductRepository productRepository,
        ILogger<SearchProductsHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> HandleAsync(SearchProductsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching products with term: {SearchTerm}", query.SearchTerm);

        var products = await _productRepository.SearchAsync(query.SearchTerm, cancellationToken);

        // Apply additional filters
        if (query.CategoryId.HasValue)
        {
            var categoryId = new CategoryId(query.CategoryId.Value);
            products = products.Where(p => p.CategoryId == categoryId);
        }

        if (query.MinPrice.HasValue)
        {
            products = products.Where(p => p.Price.Amount >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            products = products.Where(p => p.Price.Amount <= query.MaxPrice.Value);
        }

        var totalCount = products.Count();
        var pagedProducts = products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                Sku = p.Sku.Value,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy
            })
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = pagedProducts,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }
}
```

**GetProductsByCategoryHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Queries;

public class GetProductsByCategoryHandler : IQueryHandler<GetProductsByCategoryQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsByCategoryHandler> _logger;

    public GetProductsByCategoryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsByCategoryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsByCategoryQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting products for category {CategoryId}", query.CategoryId);

        var categoryId = new CategoryId(query.CategoryId);
        var products = await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);

        var totalCount = products.Count();
        var pagedProducts = products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                Sku = p.Sku.Value,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy
            })
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = pagedProducts,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }
}
```

**GetCategoriesHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Queries;

public class GetCategoriesHandler : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoriesHandler> _logger;

    public GetCategoriesHandler(
        ICategoryRepository categoryRepository,
        ILogger<GetCategoriesHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting categories with filters: ActiveOnly={ActiveOnly}, ParentId={ParentId}",
            query.ActiveOnly, query.ParentId);

        var categories = query.ActiveOnly 
            ? await _categoryRepository.GetActiveCategoriesAsync(cancellationToken)
            : await _categoryRepository.GetAllAsync(cancellationToken);

        if (query.ParentId.HasValue)
        {
            var parentId = new CategoryId(query.ParentId.Value);
            categories = categories.Where(c => c.ParentId == parentId);
        }

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id.Value,
            Name = c.Name,
            Description = c.Description,
            ParentId = c.ParentId?.Value,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }
}
```

**GetCategoryHandler.cs**:
```csharp
using Mamey.CQRS.Queries;
using Mamey.Products.Application.DTO;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Queries;

public class GetCategoryHandler : IQueryHandler<GetCategoryQuery, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoryHandler> _logger;

    public GetCategoryHandler(
        ICategoryRepository categoryRepository,
        ILogger<GetCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<CategoryDto?> HandleAsync(GetCategoryQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting category {CategoryId}", query.CategoryId);

        var categoryId = new CategoryId(query.CategoryId);
        var category = await _categoryRepository.GetAsync(categoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category {CategoryId} not found", query.CategoryId);
            return null;
        }

        return new CategoryDto
        {
            Id = category.Id.Value,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId?.Value,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
```

### DTOs

**ProductDto.cs**:
```csharp
namespace Mamey.Products.Application.DTO;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public string Sku { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
```

**CategoryDto.cs**:
```csharp
namespace Mamey.Products.Application.DTO;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**PagedResult.cs**:
```csharp
namespace Mamey.Products.Application.DTO;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
```

### Event Handlers

**ProductCreatedEventHandler.cs**:
```csharp
using Mamey.CQRS.Events;
using Mamey.Products.Domain.Events;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Events;

public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductCreatedEvent for product {ProductId}", @event.ProductId);

        // Publish integration event to other services
        var integrationEvent = new ProductCreatedIntegrationEvent(
            @event.ProductId.Value,
            @event.Name,
            @event.Price,
            @event.Sku);

        await _messageBroker.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation("ProductCreatedIntegrationEvent published for product {ProductId}", @event.ProductId);
    }
}
```

**ProductPriceChangedEventHandler.cs**:
```csharp
using Mamey.CQRS.Events;
using Mamey.Products.Domain.Events;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Events;

public class ProductPriceChangedEventHandler : IEventHandler<ProductPriceChangedEvent>
{
    private readonly ILogger<ProductPriceChangedEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;

    public ProductPriceChangedEventHandler(
        ILogger<ProductPriceChangedEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ProductPriceChangedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductPriceChangedEvent for product {ProductId}: {OldPrice} -> {NewPrice}",
            @event.ProductId, @event.OldPrice, @event.NewPrice);

        // Publish integration event to other services
        var integrationEvent = new ProductPriceChangedIntegrationEvent(
            @event.ProductId.Value,
            @event.OldPrice,
            @event.NewPrice);

        await _messageBroker.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation("ProductPriceChangedIntegrationEvent published for product {ProductId}", @event.ProductId);
    }
}
```

**ProductStockUpdatedEventHandler.cs**:
```csharp
using Mamey.CQRS.Events;
using Mamey.Products.Domain.Events;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Application.Events;

public class ProductStockUpdatedEventHandler : IEventHandler<ProductStockUpdatedEvent>
{
    private readonly ILogger<ProductStockUpdatedEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;

    public ProductStockUpdatedEventHandler(
        ILogger<ProductStockUpdatedEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ProductStockUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductStockUpdatedEvent for product {ProductId}: {OldQuantity} -> {NewQuantity}",
            @event.ProductId, @event.OldQuantity, @event.NewQuantity);

        // Publish integration event to other services
        var integrationEvent = new ProductStockUpdatedIntegrationEvent(
            @event.ProductId.Value,
            @event.OldQuantity,
            @event.NewQuantity);

        await _messageBroker.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation("ProductStockUpdatedIntegrationEvent published for product {ProductId}", @event.ProductId);
    }
}
```

### Integration Events

**ProductCreatedIntegrationEvent.cs**:
```csharp
using Mamey.CQRS.Events;

namespace Mamey.Products.Application.Events;

public record ProductCreatedIntegrationEvent(
    Guid ProductId,
    string Name,
    decimal Price,
    string Sku) : IEvent;
```

**ProductPriceChangedIntegrationEvent.cs**:
```csharp
using Mamey.CQRS.Events;

namespace Mamey.Products.Application.Events;

public record ProductPriceChangedIntegrationEvent(
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice) : IEvent;
```

**ProductStockUpdatedIntegrationEvent.cs**:
```csharp
using Mamey.CQRS.Events;

namespace Mamey.Products.Application.Events;

public record ProductStockUpdatedIntegrationEvent(
    Guid ProductId,
    int OldQuantity,
    int NewQuantity) : IEvent;
```

---

## Infrastructure Layer

The infrastructure layer handles external concerns like database access, message brokers, and external service integrations.

### MongoDB Integration

**ProductRepository.cs**:
```csharp
using Mamey.Persistence.MongoDB;
using Mamey.Products.Domain.Entities;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using MongoDB.Driver;

namespace Mamey.Products.Infrastructure.Mongo.Repositories;

public class ProductRepository : MongoRepository<Product, ProductId>, IProductRepository
{
    public ProductRepository(IMongoSessionFactory sessionFactory) : base(sessionFactory)
    {
    }

    public async Task<Product?> GetBySkuAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Sku, sku);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.CategoryId, categoryId);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.Regex(x => x.Sku.Value, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Status, Domain.Enums.ProductStatus.Active);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Sku, sku);
        var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<bool> ExistsBySkuAsync(SKU sku, ProductId excludeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(x => x.Sku, sku),
            Builders<Product>.Filter.Ne(x => x.Id, excludeId)
        );
        var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}
```

**CategoryRepository.cs**:
```csharp
using Mamey.Persistence.MongoDB;
using Mamey.Products.Domain.Entities;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;
using MongoDB.Driver;

namespace Mamey.Products.Infrastructure.Mongo.Repositories;

public class CategoryRepository : MongoRepository<Category, CategoryId>, ICategoryRepository
{
    public CategoryRepository(IMongoSessionFactory sessionFactory) : base(sessionFactory)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Category>.Filter.Eq(x => x.IsActive, true);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetByParentIdAsync(CategoryId? parentId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Category>.Filter.Eq(x => x.ParentId, parentId);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        var productCollection = Database.GetCollection<Product>("products");
        var filter = Builders<Product>.Filter.Eq(x => x.CategoryId, categoryId);
        var count = await productCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}
```

### Redis Integration

**ProductCacheService.cs**:
```csharp
using Mamey.Products.Application.DTO;
using System.Text.Json;

namespace Mamey.Products.Infrastructure.Services;

public interface IProductCacheService
{
    Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task SetProductAsync(ProductDto product, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>?> GetProductsAsync(string cacheKey, CancellationToken cancellationToken = default);
    Task SetProductsAsync(string cacheKey, PagedResult<ProductDto> products, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveProductsAsync(string pattern, CancellationToken cancellationToken = default);
}

public class ProductCacheService : IProductCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductCacheService(IDistributedCache cache, ILogger<ProductCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product:{productId}";
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (cachedValue == null)
        {
            _logger.LogDebug("Product {ProductId} not found in cache", productId);
            return null;
        }

        _logger.LogDebug("Product {ProductId} found in cache", productId);
        return JsonSerializer.Deserialize<ProductDto>(cachedValue, _jsonOptions);
    }

    public async Task SetProductAsync(ProductDto product, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product:{product.Id}";
        var serializedProduct = JsonSerializer.Serialize(product, _jsonOptions);
        
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }
        else
        {
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
        }

        await _cache.SetStringAsync(cacheKey, serializedProduct, options, cancellationToken);
        _logger.LogDebug("Product {ProductId} cached with expiry {Expiry}", product.Id, expiry ?? TimeSpan.FromMinutes(15));
    }

    public async Task RemoveProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product:{productId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        _logger.LogDebug("Product {ProductId} removed from cache", productId);
    }

    public async Task<PagedResult<ProductDto>?> GetProductsAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (cachedValue == null)
        {
            _logger.LogDebug("Products not found in cache for key {CacheKey}", cacheKey);
            return null;
        }

        _logger.LogDebug("Products found in cache for key {CacheKey}", cacheKey);
        return JsonSerializer.Deserialize<PagedResult<ProductDto>>(cachedValue, _jsonOptions);
    }

    public async Task SetProductsAsync(string cacheKey, PagedResult<ProductDto> products, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serializedProducts = JsonSerializer.Serialize(products, _jsonOptions);
        
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }
        else
        {
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }

        await _cache.SetStringAsync(cacheKey, serializedProducts, options, cancellationToken);
        _logger.LogDebug("Products cached with key {CacheKey} and expiry {Expiry}", cacheKey, expiry ?? TimeSpan.FromMinutes(5));
    }

    public async Task RemoveProductsAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: This is a simplified implementation. In production, you might want to use Redis SCAN
        // or maintain a list of cache keys for more efficient pattern-based removal
        _logger.LogDebug("Removing products from cache with pattern {Pattern}", pattern);
        // Implementation would depend on your Redis setup and requirements
    }
}
```

### Message Broker Integration

**MessageBusSubscriber.cs**:
```csharp
using Mamey.MessageBrokers;
using Mamey.Products.Application.Events;

namespace Mamey.Products.Infrastructure;

public class MessageBusSubscriber : IMessageBusSubscriber
{
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<MessageBusSubscriber> _logger;

    public MessageBusSubscriber(IMessageBroker messageBroker, ILogger<MessageBusSubscriber> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting message bus subscriber");

        // Subscribe to external events
        await _messageBroker.SubscribeAsync<InventoryUpdatedEvent>(HandleInventoryUpdatedAsync, cancellationToken);
        await _messageBroker.SubscribeAsync<PricingUpdatedEvent>(HandlePricingUpdatedAsync, cancellationToken);

        _logger.LogInformation("Message bus subscriber started successfully");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping message bus subscriber");
        // Cleanup logic here
        await Task.CompletedTask;
    }

    private async Task HandleInventoryUpdatedAsync(InventoryUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling InventoryUpdatedEvent for product {ProductId}", @event.ProductId);
        
        // Update product stock in our database
        // This would typically involve a command handler
        
        await Task.CompletedTask;
    }

    private async Task HandlePricingUpdatedAsync(PricingUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling PricingUpdatedEvent for product {ProductId}", @event.ProductId);
        
        // Update product price in our database
        // This would typically involve a command handler
        
        await Task.CompletedTask;
    }
}

// External event definitions
public record InventoryUpdatedEvent(Guid ProductId, int NewQuantity);
public record PricingUpdatedEvent(Guid ProductId, decimal NewPrice);
```

### External Service Clients

**InventoryServiceClient.cs**:
```csharp
using Mamey.Net;

namespace Mamey.Products.Infrastructure.Clients;

public interface IInventoryServiceClient
{
    Task<bool> ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<bool> ReleaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<int> GetAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default);
}

public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly IHttpClient _httpClient;
    private readonly ILogger<InventoryServiceClient> _logger;

    public InventoryServiceClient(IHttpClient httpClient, ILogger<InventoryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reserving {Quantity} units of product {ProductId}", quantity, productId);

        try
        {
            var request = new ReserveStockRequest { ProductId = productId, Quantity = quantity };
            var response = await _httpClient.PostAsJsonAsync<ReserveStockRequest, ReserveStockResponse>(
                "api/inventory/reserve", request, cancellationToken);

            _logger.LogInformation("Stock reservation {Status} for product {ProductId}", 
                response.Success ? "succeeded" : "failed", productId);

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve stock for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<bool> ReleaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Releasing {Quantity} units of product {ProductId}", quantity, productId);

        try
        {
            var request = new ReleaseStockRequest { ProductId = productId, Quantity = quantity };
            var response = await _httpClient.PostAsJsonAsync<ReleaseStockRequest, ReleaseStockResponse>(
                "api/inventory/release", request, cancellationToken);

            _logger.LogInformation("Stock release {Status} for product {ProductId}", 
                response.Success ? "succeeded" : "failed", productId);

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release stock for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<int> GetAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting available stock for product {ProductId}", productId);

        try
        {
            var response = await _httpClient.GetAsync<StockResponse>(
                $"api/inventory/{productId}/stock", cancellationToken);

            _logger.LogDebug("Available stock for product {ProductId}: {Quantity}", productId, response.Quantity);
            return response.Quantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available stock for product {ProductId}", productId);
            return 0;
        }
    }
}

// Request/Response DTOs
public record ReserveStockRequest(Guid ProductId, int Quantity);
public record ReserveStockResponse(bool Success, string? Message);
public record ReleaseStockRequest(Guid ProductId, int Quantity);
public record ReleaseStockResponse(bool Success, string? Message);
public record StockResponse(int Quantity);
```

---

## API Layer

Now let's create the Web API layer with controllers and endpoints.

### Program.cs Setup

```csharp
using Mamey;
using Mamey.WebApi;
using Mamey.Products.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;

namespace Mamey.Products;

public class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args)
            .Build()
            .RunAsync();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webhostBuilder =>
            {
                webhostBuilder.ConfigureServices(services =>
                    services
                        .AddMamey()
                        .AddWebApi()
                        .AddCQRS()
                        .AddMessageBrokers()
                        .AddRabbitMQ()
                        .AddAuth()
                        .AddJwt()
                        .AddMongo()
                        .AddRedis()
                        .AddLogging()
                        .AddTracing()
                        .AddMetrics()
                        .AddInfrastructure()
                        .Build()
                )
                .Configure(app =>
                    app.UseInfrastructure()
                       .UseMamey()
                       .AddProductRoutes()
                );
            })
            .UseLogging()
            .UseVault();
}
```

### Product Controller

**ProductsController.cs**:
```csharp
using Microsoft.AspNetCore.Mvc;
using Mamey.Products.Application.Commands;
using Mamey.Products.Application.Queries;
using Mamey.Products.Application.DTO;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;

namespace Mamey.Products.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<ProductsController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);

        var query = new GetProductQuery(id);
        var product = await _queryDispatcher.QueryAsync(query);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Get products with filtering and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="categoryId">Category ID filter</param>
    /// <param name="status">Status filter</param>
    /// <param name="sortBy">Sort field (default: Name)</param>
    /// <param name="sortDirection">Sort direction (default: asc)</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? status = null,
        [FromQuery] string sortBy = "Name",
        [FromQuery] string sortDirection = "asc")
    {
        _logger.LogInformation("Getting products with filters: Page={Page}, PageSize={PageSize}, SearchTerm={SearchTerm}",
            page, pageSize, searchTerm);

        var query = new GetProductsQuery(page, pageSize, searchTerm, categoryId, status, sortBy, sortDirection);
        var result = await _queryDispatcher.QueryAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Search products
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="categoryId">Category ID filter</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <returns>Paginated search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ProductDto>>> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest("Search term is required");
        }

        _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);

        var query = new SearchProductsQuery(searchTerm, page, pageSize, categoryId, minPrice, maxPrice);
        var result = await _queryDispatcher.QueryAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated list of products in category</returns>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProductsByCategory(
        Guid categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting products for category {CategoryId}", categoryId);

        var query = new GetProductsByCategoryQuery(categoryId, page, pageSize);
        var result = await _queryDispatcher.QueryAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation("Creating product {ProductName}", request.Name);

        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.Currency,
            request.Sku,
            request.CategoryId,
            GetCurrentUserId());

        await _commandDispatcher.SendAsync(command);

        // Get the created product
        var productQuery = new GetProductQuery(command.ProductId);
        var product = await _queryDispatcher.QueryAsync(productQuery);

        _logger.LogInformation("Product {ProductId} created successfully", product?.Id);

        return CreatedAtAction(nameof(GetProduct), new { id = product?.Id }, product);
    }

    /// <summary>
    /// Update product details
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update request</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product {ProductId}", id);

        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            GetCurrentUserId());

        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Product {ProductId} updated successfully", id);

        return NoContent();
    }

    /// <summary>
    /// Update product price
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Price update request</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}/price")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProductPrice(Guid id, [FromBody] UpdateProductPriceRequest request)
    {
        _logger.LogInformation("Updating price for product {ProductId}", id);

        var command = new UpdateProductPriceCommand(
            id,
            request.Price,
            request.Currency,
            GetCurrentUserId());

        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Price updated for product {ProductId}", id);

        return NoContent();
    }

    /// <summary>
    /// Update product stock
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Stock update request</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}/stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] UpdateProductStockRequest request)
    {
        _logger.LogInformation("Updating stock for product {ProductId}", id);

        var command = new UpdateProductStockCommand(
            id,
            request.Quantity,
            GetCurrentUserId());

        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Stock updated for product {ProductId}", id);

        return NoContent();
    }

    /// <summary>
    /// Activate product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateProduct(Guid id)
    {
        _logger.LogInformation("Activating product {ProductId}", id);

        var command = new ActivateProductCommand(id, GetCurrentUserId());
        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Product {ProductId} activated successfully", id);

        return NoContent();
    }

    /// <summary>
    /// Deactivate product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        _logger.LogInformation("Deactivating product {ProductId}", id);

        var command = new DeactivateProductCommand(id, GetCurrentUserId());
        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Product {ProductId} deactivated successfully", id);

        return NoContent();
    }

    /// <summary>
    /// Delete product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        var command = new DeleteProductCommand(id, GetCurrentUserId());
        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Product {ProductId} deleted successfully", id);

        return NoContent();
    }

    private string GetCurrentUserId()
    {
        return User?.Identity?.Name ?? "system";
    }
}
```

### Request/Response Models

**CreateProductRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public string Sku { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}
```

**UpdateProductRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

**UpdateProductPriceRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class UpdateProductPriceRequest
{
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
}
```

**UpdateProductStockRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class UpdateProductStockRequest
{
    public int Quantity { get; set; }
}
```

### Category Controller

**CategoriesController.cs**:
```csharp
using Microsoft.AspNetCore.Mvc;
using Mamey.Products.Application.Commands;
using Mamey.Products.Application.Queries;
using Mamey.Products.Application.DTO;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;

namespace Mamey.Products.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<CategoriesController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <param name="activeOnly">Show only active categories</param>
    /// <param name="parentId">Filter by parent category ID</param>
    /// <returns>List of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(
        [FromQuery] bool activeOnly = true,
        [FromQuery] Guid? parentId = null)
    {
        _logger.LogInformation("Getting categories with filters: ActiveOnly={ActiveOnly}, ParentId={ParentId}",
            activeOnly, parentId);

        var query = new GetCategoriesQuery(activeOnly, parentId);
        var categories = await _queryDispatcher.QueryAsync(query);

        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
    {
        _logger.LogInformation("Getting category {CategoryId}", id);

        var query = new GetCategoryQuery(id);
        var category = await _queryDispatcher.QueryAsync(query);

        if (category == null)
        {
            _logger.LogWarning("Category {CategoryId} not found", id);
            return NotFound($"Category with ID {id} not found");
        }

        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="request">Category creation request</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        _logger.LogInformation("Creating category {CategoryName}", request.Name);

        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ParentId);

        await _commandDispatcher.SendAsync(command);

        // Get the created category
        var categoryQuery = new GetCategoryQuery(command.CategoryId);
        var category = await _queryDispatcher.QueryAsync(categoryQuery);

        _logger.LogInformation("Category {CategoryId} created successfully", category?.Id);

        return CreatedAtAction(nameof(GetCategory), new { id = category?.Id }, category);
    }

    /// <summary>
    /// Update category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Category update request</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        _logger.LogInformation("Updating category {CategoryId}", id);

        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description);

        await _commandDispatcher.SendAsync(command);

        _logger.LogInformation("Category {CategoryId} updated successfully", id);

        return NoContent();
    }
}
```

**CreateCategoryRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}
```

**UpdateCategoryRequest.cs**:
```csharp
namespace Mamey.Products.Api.Models;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

---

## Authentication & Authorization

### JWT Configuration

**JwtOptions.cs**:
```csharp
namespace Mamey.Products.Infrastructure.Auth;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
    public string Algorithm { get; set; } = "HS256";
}
```

### Authentication Service

**AuthenticationService.cs**:
```csharp
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mamey.Products.Infrastructure.Auth;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IOptions<JwtOptions> jwtOptions, ILogger<AuthenticationService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
    {
        _logger.LogInformation("Authenticating user {Username}", username);

        // In a real application, you would validate credentials against a user store
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return AuthenticationResult.Failed("Invalid credentials");
        }

        // Mock authentication - replace with real user validation
        var user = await ValidateUserCredentialsAsync(username, password);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed for user {Username}", username);
            return AuthenticationResult.Failed("Invalid credentials");
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        _logger.LogInformation("User {Username} authenticated successfully", username);

        return AuthenticationResult.Success(token, refreshToken);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        _logger.LogInformation("Refreshing token");

        // In a real application, you would validate the refresh token
        if (string.IsNullOrEmpty(refreshToken))
        {
            return AuthenticationResult.Failed("Invalid refresh token");
        }

        // Mock refresh token validation - replace with real validation
        var user = await GetUserFromRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            _logger.LogWarning("Invalid refresh token");
            return AuthenticationResult.Failed("Invalid refresh token");
        }

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        _logger.LogInformation("Token refreshed successfully for user {Username}", user.Username);

        return AuthenticationResult.Success(newToken, newRefreshToken);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<User?> ValidateUserCredentialsAsync(string username, string password)
    {
        // Mock user validation - replace with real implementation
        await Task.Delay(100); // Simulate async operation

        return username.ToLower() switch
        {
            "admin" => new User { Id = Guid.NewGuid(), Username = "admin", Email = "admin@example.com", Role = "Admin" },
            "manager" => new User { Id = Guid.NewGuid(), Username = "manager", Email = "manager@example.com", Role = "Manager" },
            "user" => new User { Id = Guid.NewGuid(), Username = "user", Email = "user@example.com", Role = "User" },
            _ => null
        };
    }

    private async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken)
    {
        // Mock refresh token validation - replace with real implementation
        await Task.Delay(100); // Simulate async operation
        return new User { Id = Guid.NewGuid(), Username = "user", Email = "user@example.com", Role = "User" };
    }
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthenticationResult Success(string accessToken, string refreshToken)
    {
        return new AuthenticationResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static AuthenticationResult Failed(string errorMessage)
    {
        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
```

### Auth Controller

**AuthController.cs**:
```csharp
using Microsoft.AspNetCore.Mvc;
using Mamey.Products.Infrastructure.Auth;

namespace Mamey.Products.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>Authentication result with JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user {Username}", request.Username);

        var result = await _authService.AuthenticateAsync(request.Username, request.Password);

        if (!result.Success)
        {
            _logger.LogWarning("Login failed for user {Username}: {Error}", request.Username, result.ErrorMessage);
            return Unauthorized(new { message = result.ErrorMessage });
        }

        _logger.LogInformation("User {Username} logged in successfully", request.Username);

        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken!,
            RefreshToken = result.RefreshToken!,
            ExpiresIn = 3600 // 1 hour
        });
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication result with JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Token refresh attempt");

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Success)
        {
            _logger.LogWarning("Token refresh failed: {Error}", result.ErrorMessage);
            return Unauthorized(new { message = result.ErrorMessage });
        }

        _logger.LogInformation("Token refreshed successfully");

        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken!,
            RefreshToken = result.RefreshToken!,
            ExpiresIn = 3600 // 1 hour
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
```

---

## Observability

Observability is crucial for production microservices. Let's implement comprehensive logging, tracing, and metrics.

### Structured Logging

**Serilog Configuration**:
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Elasticsearch"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Mamey": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/products-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://localhost:9200",
          "indexFormat": "products-logs-{0:yyyy.MM.dd}",
          "autoRegisterTemplate": true
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId", "WithProcessId"]
  }
}
```

### Distributed Tracing

**Jaeger Configuration**:
```csharp
// Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMongoDBInstrumentation()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
            });
    });
```

### Metrics

**AppMetrics Configuration**:
```csharp
// Program.cs
builder.Services.AddMetrics()
    .AddPrometheusMetrics()
    .AddHealthChecks()
    .AddMongoDb(connectionString)
    .AddRabbitMQ(rabbitMqConnectionString);
```

---

## Message Brokers & Event-Driven Architecture

### RabbitMQ Setup

**docker-compose.yml**:
```yaml
version: '3.8'
services:
  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq

volumes:
  rabbitmq_data:
```

### Event Publishing

**EventPublisher.cs**:
```csharp
using Mamey.MessageBrokers;

namespace Mamey.Products.Infrastructure;

public class EventPublisher : IEventPublisher
{
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IMessageBroker messageBroker, ILogger<EventPublisher> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogInformation("Publishing event {EventType} with ID {EventId}", 
                typeof(T).Name, @event.GetHashCode());

            await _messageBroker.PublishAsync(@event, cancellationToken);

            _logger.LogInformation("Event {EventType} published successfully", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType}", typeof(T).Name);
            throw;
        }
    }
}
```

---

## Testing

### Unit Tests

**CreateProductHandlerTests.cs**:
```csharp
using Xunit;
using Moq;
using Mamey.Products.Application.Commands;
using Mamey.Products.Domain.Repositories;
using Mamey.Products.Domain.ValueObjects;

namespace Mamey.Products.Tests.Unit;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger<CreateProductHandler>>();
        _handler = new CreateProductHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Test Product",
            "Test Description",
            99.99m,
            "USD",
            "TEST-SKU-001",
            Guid.NewGuid(),
            "test-user");

        var categoryId = new CategoryId(command.CategoryId);
        var category = new Category(categoryId, "Test Category", "Test Category Description");

        _categoryRepositoryMock
            .Setup(x => x.GetAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _productRepositoryMock
            .Setup(x => x.ExistsBySkuAsync(It.IsAny<SKU>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _productRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCategory_ShouldThrowException()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Test Product",
            "Test Description",
            99.99m,
            "USD",
            "TEST-SKU-001",
            Guid.NewGuid(),
            "test-user");

        var categoryId = new CategoryId(command.CategoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => _handler.HandleAsync(command));
    }
}
```

### Integration Tests

**ProductsControllerIntegrationTests.cs**:
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Mamey.Products.Tests.Integration;

public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Currency = "USD",
            Sku = "TEST-SKU-001",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
```

---

## Docker & Containerization

### Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Mamey.Products.Api/Mamey.Products.Api.csproj", "src/Mamey.Products.Api/"]
COPY ["src/Mamey.Products.Application/Mamey.Products.Application.csproj", "src/Mamey.Products.Application/"]
COPY ["src/Mamey.Products.Domain/Mamey.Products.Domain.csproj", "src/Mamey.Products.Domain/"]
COPY ["src/Mamey.Products.Infrastructure/Mamey.Products.Infrastructure.csproj", "src/Mamey.Products.Infrastructure/"]
COPY ["src/Mamey.Products.Contracts/Mamey.Products.Contracts.csproj", "src/Mamey.Products.Contracts/"]

# Restore dependencies
RUN dotnet restore "src/Mamey.Products.Api/Mamey.Products.Api.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/src/Mamey.Products.Api"
RUN dotnet build "Mamey.Products.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Mamey.Products.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose port
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Start application
ENTRYPOINT ["dotnet", "Mamey.Products.Api.dll"]
```

### Docker Compose

**docker-compose.yml**:
```yaml
version: '3.8'

services:
  products-api:
    build: .
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MONGO__CONNECTIONSTRING=mongodb://mongo:27017
      - REDIS__CONNECTIONSTRING=redis:6379
      - RABBITMQ__CONNECTIONSTRING=amqp://guest:guest@rabbitmq:5672
    depends_on:
      - mongo
      - redis
      - rabbitmq
    networks:
      - products-network

  mongo:
    image: mongo:7.0
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db
    networks:
      - products-network

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - products-network

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - products-network

  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "16686:16686"
      - "14268:14268"
    environment:
      - COLLECTOR_OTLP_ENABLED=true
    networks:
      - products-network

volumes:
  mongo_data:
  redis_data:
  rabbitmq_data:

networks:
  products-network:
    driver: bridge
```

---

## Deployment

### Kubernetes Manifests

**deployment.yaml**:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: products-api
  labels:
    app: products-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: products-api
  template:
    metadata:
      labels:
        app: products-api
    spec:
      containers:
      - name: products-api
        image: mamey/products-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: MONGO__CONNECTIONSTRING
          valueFrom:
            secretKeyRef:
              name: products-secrets
              key: mongo-connection
        - name: REDIS__CONNECTIONSTRING
          valueFrom:
            secretKeyRef:
              name: products-secrets
              key: redis-connection
        - name: RABBITMQ__CONNECTIONSTRING
          valueFrom:
            secretKeyRef:
              name: products-secrets
              key: rabbitmq-connection
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
```

**service.yaml**:
```yaml
apiVersion: v1
kind: Service
metadata:
  name: products-api-service
spec:
  selector:
    app: products-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: ClusterIP
```

**ingress.yaml**:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: products-api-ingress
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
spec:
  tls:
  - hosts:
    - products-api.example.com
    secretName: products-tls
  rules:
  - host: products-api.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: products-api-service
            port:
              number: 80
```

---

## Production Considerations

### Configuration Management

**Azure Key Vault Integration**:
```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri("https://your-keyvault.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Monitoring and Alerting

**Health Checks**:
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddMongoDb(connectionString, name: "mongodb")
    .AddRedis(redisConnectionString, name: "redis")
    .AddRabbitMQ(rabbitMqConnectionString, name: "rabbitmq")
    .AddCheck<CustomHealthCheck>("custom-check");
```

### Security Hardening

**Security Headers Middleware**:
```csharp
// Program.cs
app.UseSecurityHeaders(policies =>
    policies
        .AddFrameOptionsDeny()
        .AddXssProtectionBlock()
        .AddContentTypeOptionsNoSniff()
        .AddReferrerPolicyStrictOriginWhenCrossOrigin()
        .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
        .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
        .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
        .RemoveServerHeader());
```

---

## Complete Working Example

### Running the Service

1. **Start Dependencies**:
```bash
docker-compose up -d mongo redis rabbitmq jaeger
```

2. **Run the Service**:
```bash
cd src/Mamey.Products.Api
dotnet run
```

3. **Test the API**:
```bash
# Get products
curl -X GET "http://localhost:5001/api/v1/products"

# Create a product
curl -X POST "http://localhost:5001/api/v1/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "description": "A test product",
    "price": 99.99,
    "currency": "USD",
    "sku": "TEST-001",
    "categoryId": "00000000-0000-0000-0000-000000000000"
  }'
```

### API Documentation

The service includes Swagger documentation available at:
- Swagger UI: `http://localhost:5001/swagger`
- OpenAPI JSON: `http://localhost:5001/swagger/v1/swagger.json`

---

## Best Practices & Common Patterns

### Error Handling

1. **Use specific exception types**
2. **Implement global exception handling**
3. **Return meaningful error messages**
4. **Log errors with context**

### Validation

1. **Validate input at API boundaries**
2. **Use FluentValidation for complex validation**
3. **Validate business rules in domain layer**

### Caching

1. **Cache frequently accessed data**
2. **Use appropriate cache expiration**
3. **Implement cache invalidation strategies**

### Performance

1. **Use async/await consistently**
2. **Implement connection pooling**
3. **Use pagination for large datasets**
4. **Monitor performance metrics**

---

## Troubleshooting Guide

### Common Issues

1. **Service won't start**: Check configuration and dependencies
2. **Database connection issues**: Verify connection strings and network
3. **Message broker issues**: Check RabbitMQ status and configuration
4. **Authentication failures**: Verify JWT configuration

### Debugging Techniques

1. **Enable detailed logging**
2. **Use distributed tracing**
3. **Monitor health checks**
4. **Check application metrics**

---

## Next Steps & Resources

### Extending the Microservice

1. **Add more business features**
2. **Implement additional integrations**
3. **Add more comprehensive testing**
4. **Implement advanced monitoring**

### Additional Learning Resources

- [Mamey Framework Documentation](../README.md)
- [Microservices Patterns](https://microservices.io/patterns/)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

### Community and Support

- [GitHub Issues](https://github.com/mamey-io/mamey/issues)
- [Discord Community](https://discord.gg/mamey)
- [Documentation](https://docs.mamey.io)

---

## Conclusion

This comprehensive guide has walked you through building a complete, production-ready microservice using the Mamey framework. You've learned:

- **Domain-Driven Design** principles and implementation
- **CQRS** pattern with commands, queries, and events
- **Infrastructure** concerns including databases, caching, and messaging
- **API** design and implementation
- **Authentication and authorization**
- **Observability** with logging, tracing, and metrics
- **Testing** strategies and implementation
- **Containerization** and deployment
- **Production** considerations and best practices

The Products microservice we built serves as a reference implementation that you can use as a template for your own microservices. Remember to adapt the patterns and practices to your specific domain and requirements.

Happy coding with Mamey! 🚀
