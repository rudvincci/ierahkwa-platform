using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Redis.Options;
using Mamey.FWID.Identities.Infrastructure.Redis.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for Redis repository with real database.
/// </summary>
[Collection("Integration")]
public class RedisRepositoryIntegrationTests : IClassFixture<RedisFixture>, IAsyncLifetime
{
    private readonly RedisFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private IIdentityRepository? _repository;

    public RedisRepositoryIntegrationTests(RedisFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Redis cache
        services.AddSingleton<ICache>(provider =>
        {
            var database = _fixture.Database;
            return new RedisCache(database);
        });

        // Add Redis sync options
        var redisSyncOptions = new RedisSyncOptions
        {
            Enabled = true,
            CacheTimeToLive = TimeSpan.FromHours(24)
        };
        services.AddSingleton(Options.Create(redisSyncOptions));

        // Add repository
        services.AddScoped<IIdentityRepository>(provider =>
        {
            var cache = provider.GetRequiredService<ICache>();
            var options = provider.GetRequiredService<IOptions<RedisSyncOptions>>();
            return new IdentityRedisRepository(cache, options);
        });

        _serviceProvider = services.BuildServiceProvider();

        // Get repository
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Fact]
    public async Task AddAsync_ShouldCacheIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();

        // Act
        await _repository!.AddAsync(identity);

        // Assert
        var cached = await _repository.GetAsync(identity.Id);
        cached.ShouldNotBeNull();
        cached.Id.ShouldBe(identity.Id);
        cached.Name.FirstName.ShouldBe(identity.Name.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCachedIdentity()
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
    public async Task DeleteAsync_ShouldRemoveCachedIdentity()
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
    public async Task GetAsync_WhenIdentityExists_ShouldReturnCachedIdentity()
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
}

