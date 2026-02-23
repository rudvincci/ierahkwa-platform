#nullable enable
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Mamey.CQRS.Queries;

namespace Mamey.FWID.Identities.Tests.Integration.Concurrency;

/// <summary>
/// Simple test implementation of IPagedQuery for testing.
/// </summary>
internal class TestPagedQuery : PagedQueryBase
{
    public TestPagedQuery() : base() { }
    public TestPagedQuery(int page, int resultsPerPage) : base(page, resultsPerPage, string.Empty, string.Empty) { }
}

/// <summary>
/// Concurrency and race condition integration tests.
/// Tests concurrent operations on the same identity using real repositories.
/// </summary>
[Collection("Integration")]
public class ConcurrentIdentityUpdateTests : IClassFixture<PostgreSQLFixture>, IDisposable
{
    private readonly PostgreSQLFixture _postgresFixture;
    private readonly IServiceProvider _serviceProvider;
    private readonly IIdentityRepository _repository;

    public ConcurrentIdentityUpdateTests(PostgreSQLFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
        _serviceProvider = _postgresFixture.ServiceProvider;
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
    }

    public void Dispose()
    {
        // Cleanup handled by fixture
    }

    #region Concurrent Contact Information Updates

    [Fact]
    public async Task UpdateContactInformation_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent updates should be handled correctly
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        var update1 = new UpdateContactInformation
        {
            IdentityId = identity.Id,
            ContactInformation = new ContactInformation(
                new Email("updated1@example.com"),
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            )
        };

        var update2 = new UpdateContactInformation
        {
            IdentityId = identity.Id,
            ContactInformation = new ContactInformation(
                new Email("updated2@example.com"),
                new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
            )
        };

        var handler1 = new Application.Commands.Handlers.UpdateContactInformationHandler(_repository, _serviceProvider.GetRequiredService<IEventProcessor>());
        var handler2 = new Application.Commands.Handlers.UpdateContactInformationHandler(_repository, _serviceProvider.GetRequiredService<IEventProcessor>());

        // Act - Execute concurrently
        var tasks = new List<Task>
        {
            handler1.HandleAsync(update1),
            handler2.HandleAsync(update2)
        };

        await Task.WhenAll(tasks);

        // Assert
        var updatedIdentity = await _repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        // One of the updates should have succeeded (last write wins or optimistic locking)
        updatedIdentity.ContactInformation.Email.Value.ShouldBeOneOf("updated1@example.com", "updated2@example.com");
    }

    #endregion

    #region Concurrent Biometric Verifications

    [Fact]
    public async Task VerifyBiometric_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent biometric verifications should be handled correctly
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        var verifyCommand = new VerifyBiometric
        {
            IdentityId = identity.Id,
            ProvidedBiometric = providedBiometric,
            Threshold = 0.85
        };

        var handler = new Application.Commands.Handlers.VerifyBiometricHandler(
            _repository,
            _serviceProvider.GetRequiredService<IEventProcessor>(),
            _serviceProvider.GetRequiredService<Application.Services.IBiometricStorageService>(),
            _serviceProvider.GetRequiredService<Application.Services.IBiometricEvidenceService>(),
            _serviceProvider.GetRequiredService<ILogger<Application.Commands.Handlers.VerifyBiometricHandler>>());

        // Act - Execute concurrently (10 concurrent verifications)
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => handler.HandleAsync(verifyCommand))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        var verifiedIdentity = await _repository.GetAsync(identity.Id);
        verifiedIdentity.ShouldNotBeNull();
        verifiedIdentity.Status.ShouldBe(IdentityStatus.Verified);
        verifiedIdentity.VerifiedAt.ShouldNotBeNull();
    }

    #endregion

    #region Concurrent Zone Updates

    [Fact]
    public async Task UpdateZone_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent zone updates should be handled correctly
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository.AddAsync(identity);

        var update1 = new UpdateZone
        {
            IdentityId = identity.Id,
            Zone = "zone-002"
        };

        var update2 = new UpdateZone
        {
            IdentityId = identity.Id,
            Zone = "zone-003"
        };

        var handler1 = new Application.Commands.Handlers.UpdateZoneHandler(_repository, _serviceProvider.GetRequiredService<IEventProcessor>());
        var handler2 = new Application.Commands.Handlers.UpdateZoneHandler(_repository, _serviceProvider.GetRequiredService<IEventProcessor>());

        // Act - Execute concurrently
        var tasks = new List<Task>
        {
            handler1.HandleAsync(update1),
            handler2.HandleAsync(update2)
        };

        await Task.WhenAll(tasks);

        // Assert
        var updatedIdentity = await _repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        // One of the updates should have succeeded (last write wins or optimistic locking)
        updatedIdentity.Zone.ShouldBeOneOf("zone-002", "zone-003");
    }

    #endregion

    #region Race Condition: Identity Creation

    [Fact]
    public async Task AddIdentity_RaceCondition_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Race condition during identity creation should be handled
        var identityId = Guid.NewGuid();
        var command = new AddIdentity
        {
            Id = identityId,
            Name = new Name("John", "Doe"),
            PersonalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Zone = "zone-001"
        };

        var handler = new Application.Commands.Handlers.AddIdentityHandler(_repository, _serviceProvider.GetRequiredService<IEventProcessor>());

        // Act - Execute concurrently (simulate race condition)
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => handler.HandleAsync(command))
            .ToList();

        var results = new ConcurrentBag<Exception>();
        foreach (var task in tasks)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                results.Add(ex);
            }
        }

        // Assert
        // One should succeed, others should fail with IdentityAlreadyExistsException
        var query = new TestPagedQuery(1, 10);
        var identities = await _repository.BrowseAsync(query, _ => true);
        var createdIdentities = identities.Items.Where(i => i.Id.Value == identityId).ToList();
        createdIdentities.Count.ShouldBe(1); // Only one should be created
        
        // At least one should have thrown IdentityAlreadyExistsException
        results.Count.ShouldBeGreaterThan(0);
        results.All(e => e is Application.Exceptions.IdentityAlreadyExistsException).ShouldBeTrue();
    }

    #endregion
}

