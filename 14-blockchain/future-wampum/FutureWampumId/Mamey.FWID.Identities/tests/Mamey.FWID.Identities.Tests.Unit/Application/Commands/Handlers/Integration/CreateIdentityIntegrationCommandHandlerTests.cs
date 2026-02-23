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

public class CreateIdentityIntegrationCommandHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<CreateIdentityIntegrationCommandHandler> _logger;
    private readonly CreateIdentityIntegrationCommandHandler _handler;

    public CreateIdentityIntegrationCommandHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _logger = Substitute.For<ILogger<CreateIdentityIntegrationCommandHandler>>();
        _handler = new CreateIdentityIntegrationCommandHandler(_repository, _eventProcessor, _logger);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldCreateIdentity()
    {
        // Arrange
        var command = new CreateIdentityIntegrationCommand
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe", "M."),
            PersonalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Zone = "zone-001"
        };

        _repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<Identity>(i => i.Id.Value == command.Id), Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var command = new CreateIdentityIntegrationCommand
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe", "M."),
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
            new IdentityId(command.Id),
            command.Name,
            command.PersonalDetails,
            command.ContactInformation,
            command.BiometricData,
            command.Zone,
            command.ClanRegistrarId);

        _repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(existingIdentity);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityAlreadyExistsException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.Value.ShouldBe(command.Id);
        await _repository.DidNotReceive().AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

