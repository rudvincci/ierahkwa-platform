#nullable enable
using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Events;

/// <summary>
/// Event publishing tests for command handlers.
/// Tests that domain events are properly published after command execution.
/// </summary>
public class EventPublishingTests
{
    #region IdentityCreated Event Publishing

    [Fact]
    public async Task AddIdentity_WhenIdentityCreated_ShouldPublishIdentityCreatedEvent()
    {
        // Arrange - Business Rule: IdentityCreated event should be published when identity is created
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
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

        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is IdentityCreated)));
    }

    #endregion

    #region IdentityRevoked Event Publishing

    [Fact]
    public async Task RevokeIdentity_WhenIdentityRevoked_ShouldPublishIdentityRevokedEvent()
    {
        // Arrange - Business Rule: IdentityRevoked event should be published when identity is revoked
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

        var command = new RevokeIdentity
        {
            IdentityId = identityId,
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new RevokeIdentityHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is IdentityRevoked)));
    }

    #endregion

    #region IdentityVerified Event Publishing

    [Fact]
    public async Task VerifyBiometric_WhenIdentityVerified_ShouldPublishIdentityVerifiedEvent()
    {
        // Arrange - Business Rule: IdentityVerified event should be published when identity is verified
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

        var command = new VerifyBiometric
        {
            IdentityId = identityId,
            ProvidedBiometric = biometricData.ToDto(),
            Threshold = 0.85
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var storageService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricStorageService>();
        var evidenceService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<VerifyBiometricHandler>>();
        var handler = new VerifyBiometricHandler(repository, eventProcessor, storageService, evidenceService, logger);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is IdentityVerified || e is BiometricVerified)));
    }

    #endregion

    #region ContactInformationUpdated Event Publishing

    [Fact]
    public async Task UpdateContactInformation_WhenContactInformationUpdated_ShouldPublishContactInformationUpdatedEvent()
    {
        // Arrange - Business Rule: ContactInformationUpdated event should be published when contact information is updated
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

        var command = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("updated@example.com"),
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            )
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is ContactInformationUpdated)));
    }

    #endregion

    #region ZoneUpdated Event Publishing

    [Fact]
    public async Task UpdateZone_WhenZoneUpdated_ShouldPublishZoneUpdatedEvent()
    {
        // Arrange - Business Rule: ZoneUpdated event should be published when zone is updated
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

        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = "zone-002"
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateZoneHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is ZoneUpdated)));
    }

    #endregion

    #region BiometricUpdated Event Publishing

    [Fact]
    public async Task UpdateBiometric_WhenBiometricUpdated_ShouldPublishBiometricUpdatedEvent()
    {
        // Arrange - Business Rule: BiometricUpdated event should be published when biometric is updated
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

        var newBiometricBytes = new byte[] { 4, 5, 6 };
        var newBiometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBiometricBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBiometricBytes, newBiometricHash);

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = null
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var storageService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricStorageService>();
        var evidenceService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdateBiometricHandler>>();
        var handler = new UpdateBiometricHandler(repository, eventProcessor, storageService, evidenceService, logger);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await eventProcessor.Received(1).ProcessAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => 
                events.Any(e => e is BiometricUpdated || e is BiometricEnrolled)));
    }

    #endregion

    #region Event Ordering Tests

    [Fact]
    public async Task AddIdentity_WhenIdentityCreated_ShouldPublishEventsInCorrectOrder()
    {
        // Arrange - Business Rule: Events should be published in the order they were raised
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
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

        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        var capturedEvents = new List<IDomainEvent>();
        eventProcessor.ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var events = callInfo.Arg<IEnumerable<IDomainEvent>>();
                capturedEvents.AddRange(events);
            });

        // Act
        await handler.HandleAsync(command);

        // Assert
        capturedEvents.ShouldNotBeEmpty();
        // IdentityCreated should be the first event
        capturedEvents.First().ShouldBeOfType<IdentityCreated>();
    }

    #endregion

    #region Event Idempotency Tests

    [Fact]
    public async Task AddIdentity_WhenCalledTwice_ShouldPublishEventOnlyOnce()
    {
        // Arrange - Business Rule: Events should be published only once per operation
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

        // Act - First call should succeed
        await handler.HandleAsync(command);

        // Second call should fail with IdentityAlreadyExistsException
        await Should.ThrowAsync<Mamey.FWID.Identities.Application.Exceptions.IdentityAlreadyExistsException>(
            () => handler.HandleAsync(command));

        // Assert
        // First call should publish event, second call should not reach event publishing
        await eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    #endregion
}

