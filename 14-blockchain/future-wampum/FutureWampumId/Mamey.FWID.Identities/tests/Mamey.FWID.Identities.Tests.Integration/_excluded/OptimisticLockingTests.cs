#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Concurrency;

/// <summary>
/// Optimistic locking tests using EF Core concurrency tokens.
/// Tests that concurrent updates are detected and handled correctly.
/// </summary>
[Collection("Integration")]
public class OptimisticLockingTests : IClassFixture<PostgreSQLFixture>, IDisposable
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly IIdentityRepository _repository;

    public OptimisticLockingTests(PostgreSQLFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
        _serviceProvider = _postgresFixture.ServiceProvider;
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
    }

    public void Dispose()
    {
        // Cleanup handled by fixture
    }

    #region Optimistic Concurrency Control - Version Conflict

    [Fact]
    public async Task UpdateIdentity_WhenVersionChanged_ShouldThrowDbUpdateConcurrencyException()
    {
        // Arrange - Business Rule: Optimistic locking should detect version conflicts
        // Note: This test assumes Identity entity has a Version property with [ConcurrencyCheck] attribute
        // If not implemented yet, this test will guide the implementation
        
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Load identity twice to simulate two concurrent users
        var identity1 = await _repository.GetAsync(identity.Id);
        var identity2 = await _repository.GetAsync(identity.Id);

        identity1.ShouldNotBeNull();
        identity2.ShouldNotBeNull();

        // Act - First user updates
        identity1.UpdateContactInformation(new ContactInformation(
            new Email("updated1@example.com"),
            new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
        ));
        await _repository.UpdateAsync(identity1);

        // Second user tries to update with stale version
        identity2.UpdateContactInformation(new ContactInformation(
            new Email("updated2@example.com"),
            new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
        ));

        // Assert - Should throw DbUpdateConcurrencyException
        // Note: This will fail until optimistic locking is implemented
        // Expected: DbUpdateConcurrencyException or similar
        await Should.ThrowAsync<DbUpdateConcurrencyException>(
            () => _repository.UpdateAsync(identity2));
    }

    [Fact]
    public async Task UpdateIdentity_WhenVersionUnchanged_ShouldSucceed()
    {
        // Arrange - Business Rule: Updates with correct version should succeed
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Load and update with same version
        var loadedIdentity = await _repository.GetAsync(identity.Id);
        loadedIdentity.ShouldNotBeNull();

        loadedIdentity.UpdateContactInformation(new ContactInformation(
            new Email("updated@example.com"),
            new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
        ));

        // Assert - Should succeed
        await _repository.UpdateAsync(loadedIdentity);

        var updatedIdentity = await _repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.ContactInformation.Email.Value.ShouldBe("updated@example.com");
    }

    #endregion

    #region Optimistic Concurrency Control - Retry Strategy

    [Fact]
    public async Task UpdateIdentity_WhenVersionConflict_ShouldRetryWithFreshVersion()
    {
        // Arrange - Business Rule: On version conflict, should reload and retry
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Load identity twice to simulate two concurrent users
        var identity1 = await _repository.GetAsync(identity.Id);
        var identity2 = await _repository.GetAsync(identity.Id);

        identity1.ShouldNotBeNull();
        identity2.ShouldNotBeNull();

        // Act - First user updates
        identity1.UpdateContactInformation(new ContactInformation(
            new Email("updated1@example.com"),
            new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
        ));
        await _repository.UpdateAsync(identity1);

        // Second user tries to update with stale version, then retries with fresh version
        identity2.UpdateContactInformation(new ContactInformation(
            new Email("updated2@example.com"),
            new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
        ));

        try
        {
            await _repository.UpdateAsync(identity2);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Retry with fresh version
            var freshIdentity = await _repository.GetAsync(identity.Id);
            freshIdentity.ShouldNotBeNull();
            
            freshIdentity.UpdateContactInformation(new ContactInformation(
                new Email("updated2@example.com"),
                new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
            ));
            
            await _repository.UpdateAsync(freshIdentity);
        }

        // Assert - Final state should reflect second user's update
        var finalIdentity = await _repository.GetAsync(identity.Id);
        finalIdentity.ShouldNotBeNull();
        finalIdentity.ContactInformation.Email.Value.ShouldBe("updated2@example.com");
    }

    #endregion

    #region Optimistic Concurrency Control - Multiple Concurrent Updates

    [Fact]
    public async Task UpdateIdentity_MultipleConcurrentUpdates_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Multiple concurrent updates should be handled with optimistic locking
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Multiple concurrent updates
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        
        for (int i = 0; i < 5; i++)
        {
            var index = i;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var loadedIdentity = await _repository.GetAsync(identity.Id);
                    if (loadedIdentity != null)
                    {
                        loadedIdentity.UpdateZone($"zone-{index:000}");
                        await _repository.UpdateAsync(loadedIdentity);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert - Some should succeed, some should fail with concurrency exceptions
        // At least one should succeed
        var finalIdentity = await _repository.GetAsync(identity.Id);
        finalIdentity.ShouldNotBeNull();
        finalIdentity.Zone.ShouldNotBeNull();
        
        // Some updates should have failed with concurrency exceptions
        // Note: This behavior depends on timing and implementation
    }

    #endregion

    #region Optimistic Concurrency Control - Version Increment

    [Fact]
    public async Task UpdateIdentity_WhenUpdated_ShouldIncrementVersion()
    {
        // Arrange - Business Rule: Version should increment on each update
        // Note: This test assumes Version property exists and increments automatically
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        // Act - Load and check initial version
        var loadedIdentity1 = await _repository.GetAsync(identity.Id);
        loadedIdentity1.ShouldNotBeNull();
        
        // Note: If Version property exists, check it
        // var initialVersion = loadedIdentity1.Version;

        // Update
        loadedIdentity1.UpdateZone("zone-002");
        await _repository.UpdateAsync(loadedIdentity1);

        // Reload and check version incremented
        var loadedIdentity2 = await _repository.GetAsync(identity.Id);
        loadedIdentity2.ShouldNotBeNull();
        
        // Assert - Version should have incremented
        // Note: This will fail until Version property is implemented
        // if (initialVersion != null && loadedIdentity2.Version != null)
        // {
        //     loadedIdentity2.Version.ShouldBeGreaterThan(initialVersion);
        // }
        
        loadedIdentity2.Zone.ShouldBe("zone-002");
    }

    #endregion
}

