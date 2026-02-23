using Mamey.CQRS;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

/// <summary>
/// Idempotency tests for command handlers.
/// Tests that duplicate operations are handled correctly (idempotent or throw exception).
/// </summary>
public class IdempotencyTests
{
    #region Identity Creation Idempotency

    [Fact]
    public async Task AddIdentity_WithSameIdTwice_ShouldThrowException()
    {
        // Arrange - Business Rule: Creating identity with same ID twice should fail
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

        var existingIdentity = new Identity(
            new IdentityId(identityId),
            command.Name,
            command.PersonalDetails,
            command.ContactInformation,
            command.BiometricData,
            command.Zone,
            command.ClanRegistrarId);

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new AddIdentityHandler(repository, eventProcessor);

        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(existingIdentity);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityAlreadyExistsException>(
            () => handler.HandleAsync(command));

        exception.IdentityId.Value.ShouldBe(identityId);
        await repository.DidNotReceive().AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Biometric Update Idempotency

    [Fact]
    public async Task UpdateBiometric_WithSameDataTwice_ShouldSucceed()
    {
        // Arrange - Business Rule: Updating biometric with same data is idempotent
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

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = biometricData,
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

        // Act - Update with same data twice
        await handler.HandleAsync(command);
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Revocation Idempotency

    [Fact]
    public async Task RevokeIdentity_WhenAlreadyRevoked_ShouldSucceed()
    {
        // Arrange - Business Rule: Revocation is idempotent
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

        identity.Revoke("First revocation", Guid.NewGuid());
        var firstRevokedAt = identity.RevokedAt;

        var command = new RevokeIdentity
        {
            IdentityId = identityId,
            Reason = "Second revocation",
            RevokedBy = Guid.NewGuid()
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new RevokeIdentityHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act - Revoke again
        await handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldBe(firstRevokedAt); // Should remain unchanged
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Verification Idempotency

    [Fact]
    public async Task VerifyBiometric_WhenAlreadyVerified_ShouldSucceed()
    {
        // Arrange - Business Rule: Verification is idempotent
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

        identity.VerifyBiometric(biometricData, 0.85);
        identity.Status.ShouldBe(IdentityStatus.Verified);
        var firstVerifiedAt = identity.VerifiedAt;

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

        // Act - Verify again
        await handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldBe(firstVerifiedAt); // Should remain unchanged
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Contact Information Update Idempotency

    [Fact]
    public async Task UpdateContactInformation_WithSameDataTwice_ShouldSucceed()
    {
        // Arrange - Business Rule: Updating contact information with same data is idempotent
        var identityId = new IdentityId(Guid.NewGuid());
        var contactInfo = new ContactInformation(
            new Email("john.doe@example.com"),
            new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
        );

        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            contactInfo,
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var command = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = contactInfo
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act - Update with same data twice
        await handler.HandleAsync(command);
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Zone Update Idempotency

    [Fact]
    public async Task UpdateZone_WithSameZoneTwice_ShouldSucceed()
    {
        // Arrange - Business Rule: Updating zone with same value is idempotent
        var identityId = new IdentityId(Guid.NewGuid());
        var zone = "zone-001";

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
            zone,
            null);

        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = zone
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateZoneHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act - Update with same zone twice
        await handler.HandleAsync(command);
        await handler.HandleAsync(command);

        // Assert
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion
}

