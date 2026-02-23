using Mamey.CQRS;
using System;
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

public class RevokeIdentityHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly RevokeIdentityHandler _handler;

    public RevokeIdentityHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _handler = new RevokeIdentityHandler(_repository, _eventProcessor);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldRevokeIdentity()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new RevokeIdentity
        {
            IdentityId = identityId,
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new RevokeIdentity
        {
            IdentityId = identityId,
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.ShouldBe(identityId);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityAlreadyRevoked_ShouldNotThrowAndShouldNotUpdateRevokedAt()
    {
        // Arrange - Business Rule: Revocation is idempotent
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var firstRevokedBy = Guid.NewGuid();
        identity.Revoke("First revocation", firstRevokedBy);
        var initialRevokedAt = identity.RevokedAt;

        var command = new RevokeIdentity
        {
            IdentityId = identityId,
            Reason = "Second revocation",
            RevokedBy = Guid.NewGuid()
        };

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert - Business Rule: Should not update RevokedAt on subsequent revocations
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldBe(initialRevokedAt); // Should remain unchanged
        identity.Metadata["RevokedBy"].ShouldBe(firstRevokedBy.ToString()); // Should remain first revoker
        // Note: The domain method returns early if already revoked, so no event is raised
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenReasonIsNull_ShouldStillRevokeIdentity()
    {
        // Arrange - Business Rule: Reason can be null
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
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
            Reason = null,
            RevokedBy = Guid.NewGuid()
        };

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldNotBeNull();
        identity.Metadata["RevocationReason"].ShouldBe(string.Empty); // Domain sets empty string for null
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

