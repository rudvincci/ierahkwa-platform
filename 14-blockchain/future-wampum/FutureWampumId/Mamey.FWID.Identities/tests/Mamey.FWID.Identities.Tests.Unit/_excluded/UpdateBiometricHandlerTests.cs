using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Application.Mappers;
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

public class UpdateBiometricHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly IBiometricStorageService _storageService;
    private readonly IBiometricEvidenceService _evidenceService;
    private readonly ILogger<UpdateBiometricHandler> _logger;
    private readonly UpdateBiometricHandler _handler;

    public UpdateBiometricHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _eventProcessor = Substitute.For<IEventProcessor>();
        _storageService = Substitute.For<IBiometricStorageService>();
        _evidenceService = Substitute.For<IBiometricEvidenceService>();
        _logger = Substitute.For<ILogger<UpdateBiometricHandler>>();
        _handler = new UpdateBiometricHandler(_repository, _eventProcessor, _storageService, _evidenceService, _logger);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldUpdateBiometric()
    {
        // Arrange - Business Rule: Update biometric requires verification with old biometric first
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identityId = new IdentityId(Guid.NewGuid());
        var newBytes = new byte[] { 1, 2, 3 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        
        var verificationBytes = new byte[] { 4, 5, 6 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash);

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
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
            new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash), // Must match verification biometric
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        _storageService.UploadBiometricAsync(identityId, newBiometric.Type, newBiometric.EncryptedTemplate, Arg.Any<CancellationToken>())
            .Returns("storage-ref-123");

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.BiometricData.Type.ShouldBe(newBiometric.Type);
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
        await _storageService.Received(1).UploadBiometricAsync(identityId, newBiometric.Type, newBiometric.EncryptedTemplate, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = new BiometricData(BiometricType.Facial, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            VerificationBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant())
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
    public async Task HandleAsync_WhenMinIOUploadFails_ShouldContinueWithoutStorage()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var newBytes = new byte[] { 1, 2, 3 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Verification biometric must match stored biometric
        var verificationBytes = new byte[] { 4, 5, 6 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash);

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
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
            new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash), // Same as verification
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        _storageService.UploadBiometricAsync(identityId, newBiometric.Type, newBiometric.EncryptedTemplate, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<string>(new Exception("MinIO error")));

        // Act
        await _handler.HandleAsync(command);

        // Assert
        identity.BiometricData.Type.ShouldBe(newBiometric.Type);
        await _repository.Received(1).UpdateAsync(identity, Arg.Any<CancellationToken>());
        await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityIsRevoked_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var newBytes = new byte[] { 1, 2, 3 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 4, 5, 6 }, "verify-hash");

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
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
            new BiometricData(BiometricType.Fingerprint, new byte[] { 4, 5, 6 }, "verify-hash"),
            "zone-001",
            null);

        identity.Revoke("Test revocation", Guid.NewGuid());

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command));

        exception.Message.ShouldContain("Cannot update biometric for revoked identity");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenVerificationBiometricDoesNotMatch_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var newBytes = new byte[] { 1, 2, 3 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Verification biometric that doesn't match stored biometric
        var verificationBytes = new byte[] { 9, 9, 9 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash);

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
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
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()), // Different from verification
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<BiometricVerificationFailedException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.ShouldBe(identityId);
        exception.MatchScore.ShouldBeLessThan(0.95); // Default threshold
        identity.BiometricData.Type.ShouldBe(BiometricType.Fingerprint); // Should not be updated
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }

    [Fact]
    public async Task HandleAsync_WhenVerificationBiometricHasDifferentType_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var newBytes = new byte[] { 1, 2, 3 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Verification biometric with different type
        var verificationBytes = new byte[] { 1, 2, 3 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Facial, verificationBytes, verificationHash);

        var command = new UpdateBiometric
        {
            IdentityId = identityId,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
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
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()), // Different type
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act & Assert
        var exception = await Should.ThrowAsync<BiometricVerificationFailedException>(
            () => _handler.HandleAsync(command));

        exception.IdentityId.ShouldBe(identityId);
        exception.MatchScore.ShouldBe(0.0); // Different types return 0.0 match score
        identity.BiometricData.Type.ShouldBe(BiometricType.Fingerprint); // Should not be updated
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
        await _eventProcessor.DidNotReceive().ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
    }
}

