using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

public class VerifyBiometricHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IBiometricStorageService _storageService;
    private readonly ILogger<VerifyBiometricHandler> _logger;
    private readonly VerifyBiometricHandler _handler;
    private readonly IBiometricEvidenceService _evidenceService;
    private readonly Mamey.FWID.Identities.Application.Clients.ILedgerTransactionClient _ledgerClient;
    private readonly Mamey.Contexts.IContext _context;

    public VerifyBiometricHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _storageService = Substitute.For<IBiometricStorageService>();
        _evidenceService = Substitute.For<IBiometricEvidenceService>();
        _ledgerClient = Substitute.For<Mamey.FWID.Identities.Application.Clients.ILedgerTransactionClient>();
        _context = Substitute.For<Mamey.Contexts.IContext>();
        _logger = Substitute.For<ILogger<VerifyBiometricHandler>>();
        _handler = new VerifyBiometricHandler(_repository, _eventProcessor, _storageService, _evidenceService, _ledgerClient, _context, _logger);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldVerifyBiometric()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Use same hash for both biometrics to get perfect match
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        var threshold = 0.95;

        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = providedBiometric.ToDto(),
            Threshold = threshold
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
            new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash),
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
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
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());
        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = biometricData.ToDto(),
            Threshold = 0.95
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
    public async Task HandleAsync_WhenThresholdNotProvided_ShouldUseDefaultThreshold()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Use same hash for both biometrics to get perfect match
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = providedBiometric.ToDto(),
            Threshold = null
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
            new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash),
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldNotBeNull();
        identity.LastVerifiedAt.ShouldNotBeNull();
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenBiometricVerificationFails_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Use different hash to test verification failure
        var differentBytes = new byte[] { 9, 9, 9 };
        var differentHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(differentBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, differentBytes, differentHash);
        var threshold = 0.95;

        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = providedBiometric.ToDto(),
            Threshold = threshold
        };

        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Stored biometric has different hash than provided (different bytes)
        var storedBytes = new byte[] { 1, 2, 3 };
        var storedHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(storedBytes)).ToLowerInvariant();
        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, storedBytes, storedHash),
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<BiometricVerificationFailedException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.ShouldBe(identityId);
        exception.MatchScore.ShouldBeLessThan(threshold);
        identity.Status.ShouldBe(IdentityStatus.Pending); // Status should not change on failure
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityIsRevoked_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Use same hash for both biometrics to get perfect match
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = providedBiometric.ToDto(),
            Threshold = 0.95
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
            new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash),
            "zone-001",
            null);

        identity.Revoke("Test revocation", Guid.NewGuid());

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command));

        exception.Message.ShouldContain("Cannot verify revoked identity");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenMatchScoreEqualsThreshold_ShouldVerifySuccessfully()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Use same biometric data to get perfect match (1.0)
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var biometricData = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        var threshold = 1.0; // Set threshold to match score

        var command = new VerifyBiometric
        {
            IdentityId = identityId.Value,
            ProvidedBiometric = biometricData.ToDto(),
            Threshold = threshold
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
            biometricData, // Same biometric data
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldNotBeNull();
        identity.LastVerifiedAt.ShouldNotBeNull();
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

