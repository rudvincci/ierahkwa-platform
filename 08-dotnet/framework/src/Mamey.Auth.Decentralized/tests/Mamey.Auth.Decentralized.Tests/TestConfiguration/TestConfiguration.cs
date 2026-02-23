using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Auth.Decentralized.Caching;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Options;
using Mamey.Auth.Decentralized.Persistence.Read;
using Mamey.Auth.Decentralized.Persistence.Read.Repositories;
using Mamey.Auth.Decentralized.Persistence.Write;
using Mamey.Auth.Decentralized.Validation;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Microsoft.EntityFrameworkCore.Storage;

namespace Mamey.Auth.Decentralized.Tests.TestConfiguration;

/// <summary>
/// Provides test configuration and setup utilities for the Mamey.Auth.Decentralized library tests.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Gets the test configuration.
    /// </summary>
    /// <returns>The test configuration.</returns>
    public static IConfiguration GetTestConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// Configures test services for unit testing.
    /// </summary>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection ConfigureTestServices()
    {
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add configuration
        services.AddSingleton(configuration);

        // Add decentralized options
        services.Configure<DecentralizedOptions>(configuration.GetSection("Decentralized"));

        // Add in-memory caching for unit tests
        services.AddMemoryCache();
        services.AddSingleton<IDidDocumentCache, InMemoryDidDocumentCache>();

        // Add in-memory database for unit tests
        services.AddDbContext<DidDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        // Add mock repositories for unit tests
        services.AddScoped<IDidUnitOfWork, MockDidUnitOfWork>();
        services.AddScoped<IDidDocumentReadRepository, MockDidDocumentReadRepository>();
        services.AddScoped<IVerificationMethodReadRepository, MockVerificationMethodReadRepository>();
        services.AddScoped<IServiceEndpointReadRepository, MockServiceEndpointReadRepository>();

        // Add validators
        services.AddScoped<IDidDocumentValidator, DidDocumentValidator>();
        services.AddScoped<IW3cComplianceValidator, W3cComplianceValidator>();

        // Add handlers
        services.AddScoped<IDecentralizedHandler, DecentralizedHandler>();

        return services;
    }

    /// <summary>
    /// Configures test services for integration testing with real databases.
    /// </summary>
    /// <param name="postgresContainer">The PostgreSQL test container.</param>
    /// <param name="mongoContainer">The MongoDB test container.</param>
    /// <param name="redisContainer">The Redis test container.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection ConfigureIntegrationTestServices(
        PostgreSqlContainer postgresContainer,
        MongoDbContainer mongoContainer,
        RedisContainer redisContainer)
    {
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add configuration
        services.AddSingleton(configuration);

        // Add decentralized options
        services.Configure<DecentralizedOptions>(options =>
        {
            options.EnableDidAuthentication = true;
            options.EnableDidValidation = true;
            options.EnableCaching = true;
            options.UseRedisCache = true;
            options.RedisCacheConnection = redisContainer.GetConnectionString();
            options.UsePostgreSqlStore = true;
            options.PostgreSqlConnectionString = postgresContainer.GetConnectionString();
            options.UseMongoStore = true;
            options.MongoConnectionString = mongoContainer.GetConnectionString();
            options.MongoDatabaseName = "test_db";
            options.SupportedDidMethods = new List<string> { "web", "key" };
        });

        // Add Redis caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisContainer.GetConnectionString();
        });
        services.AddSingleton<IDidDocumentCache, RedisDidDocumentCache>();

        // Add PostgreSQL
        services.AddDbContext<DidDbContext>(options =>
        {
            options.UseNpgsql(postgresContainer.GetConnectionString());
        });
        services.AddScoped<IDidUnitOfWork, DidUnitOfWork>();

        // Add MongoDB
        services.Configure<MongoDidOptions>(options =>
        {
            options.ConnectionString = mongoContainer.GetConnectionString();
            options.DatabaseName = "test_db";
        });
        services.AddSingleton<MongoDidDbContext>();
        services.AddScoped<IDidDocumentReadRepository, MongoDidDocumentReadRepository>();
        services.AddScoped<IVerificationMethodReadRepository, MongoVerificationMethodReadRepository>();
        services.AddScoped<IServiceEndpointReadRepository, MongoServiceEndpointReadRepository>();

        // Add validators
        services.AddScoped<IDidDocumentValidator, DidDocumentValidator>();
        services.AddScoped<IW3cComplianceValidator, W3cComplianceValidator>();

        // Add handlers
        services.AddScoped<IDecentralizedHandler, DecentralizedHandler>();

        return services;
    }

    /// <summary>
    /// Creates and configures a PostgreSQL test container.
    /// </summary>
    /// <returns>The configured PostgreSQL test container.</returns>
    public static PostgreSqlContainer CreatePostgresContainer()
    {
        return new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, 5432)
            .Build();
    }

    /// <summary>
    /// Creates and configures a MongoDB test container.
    /// </summary>
    /// <returns>The configured MongoDB test container.</returns>
    public static MongoDbContainer CreateMongoContainer()
    {
        return new MongoDbBuilder()
            .WithPortBinding(27017, 27017)
            .Build();
    }

    /// <summary>
    /// Creates and configures a Redis test container.
    /// </summary>
    /// <returns>The configured Redis test container.</returns>
    public static RedisContainer CreateRedisContainer()
    {
        return new RedisBuilder()
            .WithPortBinding(6379, 6379)
            .Build();
    }

    /// <summary>
    /// Initializes the test database with test data.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the initialization.</returns>
    public static async Task InitializeTestDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DidDbContext>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Add test data
        await AddTestDataAsync(scope.ServiceProvider);
    }

    /// <summary>
    /// Cleans up the test database.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A task representing the cleanup.</returns>
    public static async Task CleanupTestDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DidDbContext>();
        
        // Clean up test data
        await CleanupTestDataAsync(scope.ServiceProvider);
        
        // Drop database
        await context.Database.EnsureDeletedAsync();
    }

    private static async Task AddTestDataAsync(IServiceProvider serviceProvider)
    {
        // This method would add test data to the database
        // Implementation depends on specific test requirements
        await Task.CompletedTask;
    }

    private static async Task CleanupTestDataAsync(IServiceProvider serviceProvider)
    {
        // This method would clean up test data from the database
        // Implementation depends on specific test requirements
        await Task.CompletedTask;
    }
}

/// <summary>
/// Mock implementation of IDidUnitOfWork for unit testing.
/// </summary>
public class MockDidUnitOfWork : IDidUnitOfWork
{
    public IRepository<Mamey.Auth.Decentralized.Persistence.Write.Entities.DidDocumentEntity> DidDocuments => throw new NotImplementedException();
    public IRepository<Mamey.Auth.Decentralized.Persistence.Write.Entities.VerificationMethodEntity> VerificationMethods => throw new NotImplementedException();
    public IRepository<Mamey.Auth.Decentralized.Persistence.Write.Entities.ServiceEndpointEntity> ServiceEndpoints => throw new NotImplementedException();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Mock implementation
    }
}

/// <summary>
/// Mock implementation of IDidDocumentReadRepository for unit testing.
/// </summary>
public class MockDidDocumentReadRepository : IDidDocumentReadRepository
{
    public Task<Mamey.Auth.Decentralized.Core.DidDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Mamey.Auth.Decentralized.Core.DidDocument?>(null);
    }

    public Task<DidDocumentReadModel?> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<DidDocumentReadModel?>(null);
    }

    public Task<IReadOnlyList<DidDocumentReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<DidDocumentReadModel>>(new List<DidDocumentReadModel>());
    }

    public Task<IReadOnlyList<DidDocumentReadModel>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<DidDocumentReadModel>>(new List<DidDocumentReadModel>());
    }
}

/// <summary>
/// Mock implementation of IVerificationMethodReadRepository for unit testing.
/// </summary>
public class MockVerificationMethodReadRepository : IVerificationMethodReadRepository
{
    public Task<Mamey.Auth.Decentralized.Core.VerificationMethod?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Mamey.Auth.Decentralized.Core.VerificationMethod?>(null);
    }

    public Task<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>>(new List<Mamey.Auth.Decentralized.Core.VerificationMethod>());
    }

    public Task<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>>(new List<Mamey.Auth.Decentralized.Core.VerificationMethod>());
    }

    public Task<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Mamey.Auth.Decentralized.Core.VerificationMethod>>(new List<Mamey.Auth.Decentralized.Core.VerificationMethod>());
    }
}

/// <summary>
/// Mock implementation of IServiceEndpointReadRepository for unit testing.
/// </summary>
public class MockServiceEndpointReadRepository : IServiceEndpointReadRepository
{
    public Task<ServiceEndpoint?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ServiceEndpoint?>(null);
    }

    public Task<IEnumerable<ServiceEndpoint>> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ServiceEndpoint>>(new List<ServiceEndpoint>());
    }

    public Task<IEnumerable<ServiceEndpoint>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ServiceEndpoint>>(new List<ServiceEndpoint>());
    }
}
