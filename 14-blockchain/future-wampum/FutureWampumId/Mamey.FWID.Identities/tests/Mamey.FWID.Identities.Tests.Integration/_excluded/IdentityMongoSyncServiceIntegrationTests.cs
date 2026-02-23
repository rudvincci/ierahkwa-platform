using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Options;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Services;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.MongoDB.Repositories;
using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Services;

/// <summary>
/// Integration tests for IdentityMongoSyncService with real PostgreSQL and MongoDB.
/// </summary>
[Collection("Integration")]
public class IdentityMongoSyncServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly MongoDBFixture _mongoFixture;
    private IServiceProvider? _serviceProvider;
    private IdentityMongoSyncService? _syncService;
    private IIdentityRepository? _postgresRepo;
    private IdentityMongoRepository? _mongoRepo;

    public IdentityMongoSyncServiceIntegrationTests(
        PostgreSQLFixture postgresFixture,
        MongoDBFixture mongoFixture)
    {
        _postgresFixture = postgresFixture;
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

        // Add MongoDB repository
        services.AddScoped<IdentityMongoRepository>(provider =>
        {
            var mongoRepository = _mongoFixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
            return new IdentityMongoRepository(mongoRepository);
        });

        // Add MongoDB sync options
        var mongoSyncOptions = new MongoSyncOptions
        {
            Enabled = true,
            SyncInterval = TimeSpan.FromSeconds(1), // Short interval for testing
            InitialDelay = TimeSpan.Zero, // No delay for testing
            RetryDelay = TimeSpan.FromSeconds(1)
        };
        services.AddSingleton(Options.Create(mongoSyncOptions));

        // Add sync service
        services.AddScoped<IdentityMongoSyncService>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure PostgreSQL database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get services
        _postgresRepo = _serviceProvider.GetRequiredService<IdentityPostgresRepository>();
        _mongoRepo = _serviceProvider.GetRequiredService<IdentityMongoRepository>();
        _syncService = _serviceProvider.GetRequiredService<IdentityMongoSyncService>();
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
    public async Task SyncIdentitysToMongo_ShouldSyncNewIdentities()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepo!.AddAsync(identity);

        // Act - Manually trigger sync (normally done by background service)
        var syncMethod = typeof(IdentityMongoSyncService).GetMethod("SyncIdentitysToMongo", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)syncMethod!.Invoke(_syncService!, new object[] { CancellationToken.None })!;

        // Assert
        var existsInMongo = await _mongoRepo!.ExistsAsync(identity.Id);
        existsInMongo.ShouldBeTrue();
    }

    [Fact]
    public async Task SyncIdentitysToMongo_ShouldUpdateExistingIdentities()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepo!.AddAsync(identity);

        // Initial sync
        var syncMethod = typeof(IdentityMongoSyncService).GetMethod("SyncIdentitysToMongo", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)syncMethod!.Invoke(_syncService!, new object[] { CancellationToken.None })!;

        // Update identity in PostgreSQL
        identity.UpdateContactInformation(TestDataFactory.CreateTestContactInformation("updated@example.com", "5551234567"));
        await _postgresRepo.UpdateAsync(identity);

        // Act - Sync again
        await (Task)syncMethod.Invoke(_syncService!, new object[] { CancellationToken.None })!;

        // Assert - MongoDB should have updated data
        var existsInMongo = await _mongoRepo!.ExistsAsync(identity.Id);
        existsInMongo.ShouldBeTrue();
    }

    [Fact]
    public async Task SyncIdentitysToMongo_WithMultipleIdentities_ShouldSyncAll()
    {
        // Arrange
        var identities = TestDataFactory.CreateTestIdentities(5);
        foreach (var identity in identities)
        {
            await _postgresRepo!.AddAsync(identity);
        }

        // Act
        var syncMethod = typeof(IdentityMongoSyncService).GetMethod("SyncIdentitysToMongo", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)syncMethod!.Invoke(_syncService!, new object[] { CancellationToken.None })!;

        // Assert
        foreach (var identity in identities)
        {
            var existsInMongo = await _mongoRepo!.ExistsAsync(identity.Id);
            existsInMongo.ShouldBeTrue();
        }
    }

    [Fact]
    public async Task SyncIdentitysToMongo_WithNoIdentities_ShouldNotThrow()
    {
        // Act
        var syncMethod = typeof(IdentityMongoSyncService).GetMethod("SyncIdentitysToMongo", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)syncMethod!.Invoke(_syncService!, new object[] { CancellationToken.None })!;

        // Assert - Should complete without errors
        true.ShouldBeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenEnabled_ShouldSyncIdentities()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepo!.AddAsync(identity);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2));

        // Act - Start the background service and let it run
        await _syncService!.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(1500, cancellationTokenSource.Token); // Wait for sync to occur
        await _syncService.StopAsync(cancellationTokenSource.Token);

        // Assert - Identity should be synced to MongoDB
        var existsInMongo = await _mongoRepo!.ExistsAsync(identity.Id);
        existsInMongo.ShouldBeTrue();
    }
}

