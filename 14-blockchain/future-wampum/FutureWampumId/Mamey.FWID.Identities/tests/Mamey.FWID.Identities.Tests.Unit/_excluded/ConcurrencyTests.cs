using System.Collections.Concurrent;
using Mamey.CQRS;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

/// <summary>
/// Concurrency and race condition tests for command handlers.
/// Tests concurrent operations on the same identity.
/// </summary>
public class ConcurrencyTests
{
    #region Concurrent Identity Updates

    [Fact]
    public async Task UpdateContactInformation_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent updates should be handled correctly
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        var command1 = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("updated1@example.com"),
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            )
        };

        var command2 = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("updated2@example.com"),
                new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
            )
        };

        // Act - Execute concurrently
        var tasks = new List<Task>
        {
            handler.HandleAsync(command1),
            handler.HandleAsync(command2)
        };

        await Task.WhenAll(tasks);

        // Assert
        await repository.Received(2).GetAsync(identityId, Arg.Any<CancellationToken>());
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBiometric_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent biometric updates should be handled correctly
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var storageService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricStorageService>();
        var evidenceService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdateBiometricHandler>>();
        var handler = new UpdateBiometricHandler(repository, eventProcessor, storageService, evidenceService, logger);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        var biometricBytes1 = new byte[] { 4, 5, 6 };
        var biometricHash1 = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes1)).ToLowerInvariant();
        var biometricData1 = new BiometricData(BiometricType.Facial, biometricBytes1, biometricHash1);

        var biometricBytes2 = new byte[] { 7, 8, 9 };
        var biometricHash2 = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes2)).ToLowerInvariant();
        var biometricData2 = new BiometricData(BiometricType.Fingerprint, biometricBytes2, biometricHash2);

        var command1 = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = biometricData1,
            VerificationBiometric = null
        };

        var command2 = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = biometricData2,
            VerificationBiometric = null
        };

        // Act - Execute concurrently
        var tasks = new List<Task>
        {
            handler.HandleAsync(command1),
            handler.HandleAsync(command2)
        };

        await Task.WhenAll(tasks);

        // Assert
        await repository.Received(2).GetAsync(identityId, Arg.Any<CancellationToken>());
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Concurrent Biometric Verifications

    [Fact]
    public async Task VerifyBiometric_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent biometric verifications should be handled correctly
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var biometricData = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            biometricData,
            "zone-001",
            null);

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var storageService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricStorageService>();
        var evidenceService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<VerifyBiometricHandler>>();
        var handler = new VerifyBiometricHandler(repository, eventProcessor, storageService, evidenceService, logger);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        var command = new VerifyBiometric
        {
            IdentityId = identityId,
            ProvidedBiometric = biometricData.ToDto(),
            Threshold = 0.85
        };

        // Act - Execute concurrently (10 concurrent verifications)
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => handler.HandleAsync(command))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        await repository.Received(10).GetAsync(identityId, Arg.Any<CancellationToken>());
        await repository.Received(10).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Concurrent Zone Updates

    [Fact]
    public async Task UpdateZone_Concurrently_ShouldHandleCorrectly()
    {
        // Arrange - Business Rule: Concurrent zone updates should be handled correctly
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateZoneHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        var command1 = new UpdateZone
        {
            IdentityId = identityId,
            Zone = "zone-002"
        };

        var command2 = new UpdateZone
        {
            IdentityId = identityId,
            Zone = "zone-003"
        };

        // Act - Execute concurrently
        var tasks = new List<Task>
        {
            handler.HandleAsync(command1),
            handler.HandleAsync(command2)
        };

        await Task.WhenAll(tasks);

        // Assert
        await repository.Received(2).GetAsync(identityId, Arg.Any<CancellationToken>());
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
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

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new AddIdentityHandler(repository, eventProcessor);

        // Simulate race condition: first call returns null, second call returns existing identity
        var callCount = 0;
        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return Task.FromResult<Identity?>(null);
                }
                else
                {
                    return Task.FromResult<Identity?>(new Identity(
                        new IdentityId(identityId),
                        command.Name,
                        command.PersonalDetails,
                        command.ContactInformation,
                        command.BiometricData,
                        command.Zone,
                        command.ClanRegistrarId));
                }
            });

        // Act - Execute concurrently (simulate race condition)
        var tasks = Enumerable.Range(0, 2)
            .Select(_ => handler.HandleAsync(command))
            .ToList();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (IdentityAlreadyExistsException)
        {
            // Expected: one should succeed, one should fail
        }

        // Assert
        await repository.Received(2).GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>());
        // At least one should succeed
        await repository.Received(1).AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion
}

