using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for PostgreSQL repository with real database.
/// </summary>
[Collection("Integration")]
public class PostgreSQLRepositoryIntegrationTests : IClassFixture<PostgreSQLFixture>, IAsyncLifetime
{
    private readonly PostgreSQLFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private IIdentityRepository? _repository;

    public PostgreSQLRepositoryIntegrationTests(PostgreSQLFixture fixture)
    {
        _fixture = fixture;
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
            options.UseNpgsql(_fixture.ConnectionString);
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

        // Add repository
        services.AddScoped<IIdentityRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            var securityProcessor = provider.GetRequiredService<SecurityAttributeProcessor>();
            return new IdentityPostgresRepository(dbContext, securityProcessor);
        });

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
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
    public async Task AddAsync_ShouldPersistIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();

        // Act
        await _repository!.AddAsync(identity);

        // Assert
        var retrieved = await _repository.GetAsync(identity.Id);
        retrieved.ShouldNotBeNull();
        retrieved.Id.ShouldBe(identity.Id);
        retrieved.Name.FirstName.ShouldBe(identity.Name.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateIdentity()
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
    public async Task DeleteAsync_ShouldRemoveIdentity()
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
    public async Task GetAsync_WhenIdentityExists_ShouldReturnIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var result = await _repository.GetAsync(identity.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(identity.Id);
    }

    [Fact]
    public async Task GetAsync_WhenIdentityDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository!.GetAsync(new IdentityId(Guid.NewGuid()));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenIdentityExists_ShouldReturnTrue()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var exists = await _repository.ExistsAsync(identity.Id);

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenIdentityDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var exists = await _repository!.ExistsAsync(new IdentityId(Guid.NewGuid()));

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task BrowseAsync_ShouldReturnAllIdentities()
    {
        // Arrange
        var identities = TestDataFactory.CreateTestIdentities(5);
        foreach (var identity in identities)
        {
            await _repository!.AddAsync(identity);
        }

        // Act
        var result = await _repository.BrowseAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnFilteredIdentities()
    {
        // Arrange
        var identity1 = TestDataFactory.CreateTestIdentity(zone: "zone-001");
        var identity2 = TestDataFactory.CreateTestIdentity(zone: "zone-002");
        var identity3 = TestDataFactory.CreateTestIdentity(zone: "zone-001");

        await _repository!.AddAsync(identity1);
        await _repository!.AddAsync(identity2);
        await _repository!.AddAsync(identity3);

        // Act
        var result = await _repository.FindAsync(i => i.Zone == "zone-001");

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.All(i => i.Zone == "zone-001").ShouldBeTrue();
    }
}

