using Mamey.CQRS;
using System;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers.Integration;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers.Integration;

public class VerifyIdentityIntegrationCommandHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<VerifyIdentityIntegrationCommandHandler> _logger;
    private readonly VerifyIdentityIntegrationCommandHandler _handler;

    public VerifyIdentityIntegrationCommandHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _logger = Substitute.For<ILogger<VerifyIdentityIntegrationCommandHandler>>();
        _handler = new VerifyIdentityIntegrationCommandHandler(_repository, _eventProcessor, _logger);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldVerifyBiometric()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());
        var threshold = 0.95;

        var command = new VerifyIdentityIntegrationCommand
        {
            IdentityId = identityId,
            ProvidedBiometric = providedBiometric,
            Threshold = threshold
        };

        var identity = new Identity(
            new IdentityId(identityId),
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

        _repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var command = new VerifyIdentityIntegrationCommand
        {
            IdentityId = identityId,
            ProvidedBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Threshold = 0.95
        };

        _repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.Value.ShouldBe(identityId);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenThresholdNotProvided_ShouldUseDefaultThreshold()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());

        var command = new VerifyIdentityIntegrationCommand
        {
            IdentityId = identityId,
            ProvidedBiometric = providedBiometric,
            Threshold = null
        };

        var identity = new Identity(
            new IdentityId(identityId),
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

        _repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

