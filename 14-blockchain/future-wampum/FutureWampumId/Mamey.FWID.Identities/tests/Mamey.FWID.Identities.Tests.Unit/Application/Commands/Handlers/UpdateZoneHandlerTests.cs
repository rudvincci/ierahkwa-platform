using Mamey.CQRS;
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

public class UpdateZoneHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly UpdateZoneHandler _handler;

    public UpdateZoneHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _handler = new UpdateZoneHandler(_repository, _eventProcessor);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldUpdateZone()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var newZone = "zone-002";

        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = newZone
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
        identity.Zone.ShouldBe(newZone);
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = "zone-002"
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
    public async Task HandleAsync_WhenIdentityIsRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot update zone for revoked identity
        var identityId = new IdentityId(Guid.NewGuid());
        var newZone = "zone-002";

        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = newZone
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

        identity.Revoke("Test revocation", Guid.NewGuid());

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command));

        exception.Message.ShouldContain("Cannot update zone for revoked identity");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenZoneIsNull_ShouldUpdateZoneToNull()
    {
        // Arrange - Business Rule: Zone can be null (clearing zone assignment)
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = null
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
        identity.Zone.ShouldBeNull();
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenZoneIsEmptyString_ShouldUpdateZoneToEmptyString()
    {
        // Arrange - Business Rule: Zone can be empty string
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new UpdateZone
        {
            IdentityId = identityId,
            Zone = string.Empty
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
        identity.Zone.ShouldBe(string.Empty);
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

