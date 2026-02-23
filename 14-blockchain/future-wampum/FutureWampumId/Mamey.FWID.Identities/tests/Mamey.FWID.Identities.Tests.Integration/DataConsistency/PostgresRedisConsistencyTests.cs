#nullable enable
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.Redis.Services;
using Microsoft.Extensions.Hosting;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.DataConsistency;

/// <summary>
/// Data consistency tests between PostgreSQL and Redis.
/// Tests that data is synchronized correctly between write model and cache.
/// </summary>
[Collection("Integration")]
public class PostgresRedisConsistencyTests : IClassFixture<PostgreSQLFixture>, IClassFixture<RedisFixture>, IAsyncLifetime, IDisposable
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly RedisFixture _redisFixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly IIdentityRepository _postgresRepository;
    private readonly IIdentityRepository _redisRepository;

    public PostgresRedisConsistencyTests(PostgreSQLFixture postgresFixture, RedisFixture redisFixture)
    {
        _postgresFixture = postgresFixture;
        _redisFixture = redisFixture;
        _serviceProvider = _postgresFixture.ServiceProvider;
        _postgresRepository = _serviceProvider.GetRequiredService<IIdentityRepository>();
        // RedisFixture doesn't have a Repository property - use ServiceProvider instead
        _redisRepository = _redisFixture.ServiceProvider.GetRequiredService<IIdentityRepository>();
    }

    public async Task InitializeAsync()
    {
        // Wait for sync service to run (if needed)
        await Task.Delay(1000);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Cleanup handled by fixtures
    }

    #region Add Identity Consistency

    [Fact]
    public async Task AddIdentity_WhenIdentityAdded_ShouldSyncToRedis()
    {
        // Arrange - Business Rule: When identity is added to PostgreSQL, it should be synced to Redis
        var identity = TestDataFactory.CreateTestIdentity();

        // Act - Add to PostgreSQL
        await _postgresRepository.AddAsync(identity);

        // Wait for sync service to sync (background service syncs periodically)
        await Task.Delay(2000);

        // Assert - Verify in Redis
        var redisIdentity = await _redisRepository.GetAsync(identity.Id);
        redisIdentity.ShouldNotBeNull();
        redisIdentity.Id.ShouldBe(identity.Id);
        redisIdentity.Name.FirstName.ShouldBe(identity.Name.FirstName);
        redisIdentity.Name.LastName.ShouldBe(identity.Name.LastName);
    }

    #endregion

    #region Update Identity Consistency

    [Fact]
    public async Task UpdateIdentity_WhenIdentityUpdated_ShouldSyncToRedis()
    {
        // Arrange - Business Rule: When identity is updated in PostgreSQL, it should be synced to Redis
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepository.AddAsync(identity);
        await Task.Delay(2000); // Wait for initial sync

        // Act - Update in PostgreSQL
        var loadedIdentity = await _postgresRepository.GetAsync(identity.Id);
        loadedIdentity.ShouldNotBeNull();
        loadedIdentity.UpdateContactInformation(new ContactInformation(
            new Email("updated@example.com"),
            new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
        ));
        await _postgresRepository.UpdateAsync(loadedIdentity);

        // Wait for sync service to sync
        await Task.Delay(2000);

        // Assert - Verify update in Redis
        var redisIdentity = await _redisRepository.GetAsync(identity.Id);
        redisIdentity.ShouldNotBeNull();
        redisIdentity.ContactInformation.Email.Value.ShouldBe("updated@example.com");
    }

    #endregion

    #region Delete Identity Consistency

    [Fact]
    public async Task RevokeIdentity_WhenIdentityRevoked_ShouldSyncToRedis()
    {
        // Arrange - Business Rule: When identity is revoked in PostgreSQL, it should be synced to Redis
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepository.AddAsync(identity);
        await Task.Delay(2000); // Wait for initial sync

        // Act - Revoke in PostgreSQL
        var loadedIdentity = await _postgresRepository.GetAsync(identity.Id);
        loadedIdentity.ShouldNotBeNull();
        loadedIdentity.Revoke("Test revocation", Guid.NewGuid());
        await _postgresRepository.UpdateAsync(loadedIdentity);

        // Wait for sync service to sync
        await Task.Delay(2000);

        // Assert - Verify revocation in Redis
        var redisIdentity = await _redisRepository.GetAsync(identity.Id);
        redisIdentity.ShouldNotBeNull();
        redisIdentity.Status.ShouldBe(IdentityStatus.Revoked);
        redisIdentity.RevokedAt.ShouldNotBeNull();
    }

    #endregion

    #region Cache Invalidation

    [Fact]
    public async Task UpdateIdentity_WhenIdentityUpdated_ShouldInvalidateCache()
    {
        // Arrange - Business Rule: When identity is updated, cache should be invalidated and refreshed
        var identity = TestDataFactory.CreateTestIdentity();
        await _postgresRepository.AddAsync(identity);
        await Task.Delay(2000); // Wait for initial sync

        // Verify cached
        var cachedIdentity = await _redisRepository.GetAsync(identity.Id);
        cachedIdentity.ShouldNotBeNull();

        // Act - Update in PostgreSQL
        var loadedIdentity = await _postgresRepository.GetAsync(identity.Id);
        loadedIdentity.ShouldNotBeNull();
        loadedIdentity.UpdateZone("zone-002");
        await _postgresRepository.UpdateAsync(loadedIdentity);

        // Wait for sync service to sync (invalidates and refreshes cache)
        await Task.Delay(2000);

        // Assert - Verify updated cache
        var updatedCachedIdentity = await _redisRepository.GetAsync(identity.Id);
        updatedCachedIdentity.ShouldNotBeNull();
        updatedCachedIdentity.Zone.ShouldBe("zone-002");
    }

    #endregion
}

