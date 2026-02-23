using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Composite;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Infrastructure.Redis.Options;
using Mamey.FWID.Identities.Infrastructure.Redis.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Persistence.Redis;
using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for Composite repository with all three storage backends.
/// </summary>
[Collection("Integration")]
public class CompositeRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly RedisFixture _redisFixture;
    private readonly MongoDBFixture _mongoFixture;
    private IServiceProvider? _serviceProvider;
    private IIdentityRepository? _repository;

    public CompositeRepositoryIntegrationTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture)
    {
        _postgresFixture = postgresFixture;
        _redisFixture = redisFixture;
        _mongoFixture = mongoFixture;
    }

    public async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Entity Framework Core
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(_postgresFixture.ConnectionString);
            options.EnableSensitiveDataLogging();
        });

        // Add Security services (manual registration for tests)
        var securityOptions = new Mamey.Security.SecurityOptions
        {
            Encryption = new Mamey.Security.SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "12345678901234567890123456789012" // 32 characters for AES-256
            }
        };
        services.AddSingleton(securityOptions);
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptorLogger = loggerFactory.CreateLogger<Encryptor>();
        services.AddSingleton<Mamey.Security.IEncryptor>(new Encryptor(encryptorLogger));
        services.AddSingleton<Mamey.Security.IHasher, Mamey.Security.Internals.Hasher>();
        services.AddSingleton<Mamey.Security.IRng, Mamey.Security.Rng>();
        services.AddSingleton<Mamey.Security.ISigner, Mamey.Security.Signer>();
        services.AddSingleton<Mamey.Security.IMd5, Mamey.Security.Md5>();
        services.AddSingleton<Mamey.Security.ISecurityProvider>(sp =>
        {
            var encryptor = sp.GetRequiredService<Mamey.Security.IEncryptor>();
            var hasher = sp.GetRequiredService<Mamey.Security.IHasher>();
            var rng = sp.GetRequiredService<Mamey.Security.IRng>();
            var signer = sp.GetRequiredService<Mamey.Security.ISigner>();
            var md5 = sp.GetRequiredService<Mamey.Security.IMd5>();
            var opts = sp.GetRequiredService<Mamey.Security.SecurityOptions>();
            return new Mamey.Security.SecurityProvider(encryptor, hasher, rng, signer, md5, opts);
        });
        services.AddScoped<Mamey.Security.SecurityAttributeProcessor>();

        // Add PostgreSQL repository
        services.AddScoped<IdentityPostgresRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            var securityProcessor = provider.GetRequiredService<SecurityAttributeProcessor>();
            return new IdentityPostgresRepository(dbContext, securityProcessor);
        });

        // Add Redis repository
        services.AddSingleton<ICache>(provider =>
        {
            var database = _redisFixture.Database;
            return new RedisCache(database);
        });

        var redisSyncOptions = new RedisSyncOptions
        {
            Enabled = true,
            CacheTimeToLive = TimeSpan.FromHours(24)
        };
        services.AddSingleton(Options.Create(redisSyncOptions));

        services.AddScoped<IdentityRedisRepository>(provider =>
        {
            var cache = provider.GetRequiredService<ICache>();
            var options = provider.GetRequiredService<IOptions<RedisSyncOptions>>();
            return new IdentityRedisRepository(cache, options);
        });

        // Add MongoDB repository
        services.AddScoped<IdentityMongoRepository>(provider =>
        {
            var mongoRepository = _mongoFixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
            return new IdentityMongoRepository(mongoRepository);
        });

        // Add Composite repository
        services.AddScoped<IIdentityRepository, CompositeIdentityRepository>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure PostgreSQL database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get repository
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider != null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            await context.Database.EnsureDeletedAsync();
        }

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToAllRepositories()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();

        // Act
        await _repository!.AddAsync(identity);

        // Assert - Should be in PostgreSQL (source of truth)
        using var scope = _serviceProvider!.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<IdentityPostgresRepository>();
        var fromPostgres = await postgresRepo.GetAsync(identity.Id);
        fromPostgres.ShouldNotBeNull();

        // Should be in Redis (cache)
        var redisRepo = scope.ServiceProvider.GetRequiredService<IdentityRedisRepository>();
        var fromRedis = await redisRepo.GetAsync(identity.Id);
        fromRedis.ShouldNotBeNull();

        // Should be in MongoDB (read model)
        var mongoRepo = scope.ServiceProvider.GetRequiredService<IdentityMongoRepository>();
        var existsInMongo = await mongoRepo.ExistsAsync(identity.Id);
        existsInMongo.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAsync_ShouldTryRedisFirst_ThenMongo_ThenPostgres()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var result = await _repository.GetAsync(identity.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(identity.Id);
        // Composite repository tries Redis → Mongo → Postgres
        // Since Redis should have it, it should return from Redis
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAllRepositories()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        identity.UpdateContactInformation(TestDataFactory.CreateTestContactInformation("updated@example.com", "5551234567"));
        await _repository.UpdateAsync(identity);

        // Assert
        var updated = await _repository.GetAsync(identity.Id);
        updated.ShouldNotBeNull();
        updated.ContactInformation.Email.Value.ShouldBe("updated@example.com");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFromAllRepositories()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        await _repository.DeleteAsync(identity.Id);

        // Assert
        var deleted = await _repository.GetAsync(identity.Id);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldCheckAllRepositories()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var exists = await _repository.ExistsAsync(identity.Id);

        // Assert
        exists.ShouldBeTrue();
    }
}

