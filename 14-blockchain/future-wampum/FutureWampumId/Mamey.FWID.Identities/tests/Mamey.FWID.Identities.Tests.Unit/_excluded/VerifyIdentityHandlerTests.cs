using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Queries.Handlers;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Queries.Handlers;

public class VerifyIdentityHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly VerifyIdentityHandler _handler;

    public VerifyIdentityHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _handler = new VerifyIdentityHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldReturnVerificationResult()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        var query = new VerifyIdentity(identityId, providedBiometric);

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
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Threshold.ShouldBe(0.85); // Default threshold is 0.85 per Biometric Verification Microservice spec
        result.VerifiedAt.ShouldNotBe(default);
        await _repository.Received(1).GetAsync(identityId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        var query = new VerifyIdentity(identityId, providedBiometric);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler.HandleAsync(query));

        exception.IdentityId.ShouldBe(identityId);
    }

    [Fact]
    public async Task HandleAsync_WhenMatchScoreAboveThreshold_ShouldReturnVerified()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricData = new byte[] { 1, 2, 3 };
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricData, "hash");
        var query = new VerifyIdentity(identityId, providedBiometric);

        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, biometricData, "hash"),
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.MatchScore.ShouldBeGreaterThanOrEqualTo(0.95);
        result.IsVerified.ShouldBeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenMatchScoreBelowThreshold_ShouldReturnNotVerified()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var storedBiometric = new byte[] { 1, 2, 3, 4, 5 };
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var differentBytes = new byte[] { 9, 9, 9, 9, 9 };
        var differentHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(differentBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, differentBytes, differentHash);
        var query = new VerifyIdentity(identityId, providedBiometric);

        var identity = new Identity(
            identityId,
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, storedBiometric, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(storedBiometric)).ToLowerInvariant()), // Different hash
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.MatchScore.ShouldBeLessThan(0.95); // Different hashes and different bytes = low match score
        result.IsVerified.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityIsRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot verify revoked identity
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        var query = new VerifyIdentity(identityId, providedBiometric);

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
            () => _handler.HandleAsync(query));

        exception.Message.ShouldContain("Cannot verify revoked identity");
    }

    [Fact]
    public async Task HandleAsync_WhenBiometricTypesDiffer_ShouldReturnZeroMatchScore()
    {
        // Arrange - Business Rule: Different biometric types return 0.0 match score
        var identityId = new IdentityId(Guid.NewGuid());
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var facialBytes = new byte[] { 1, 2, 3 };
        var facialHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(facialBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Facial, facialBytes, facialHash);
        var query = new VerifyIdentity(identityId, providedBiometric);

        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Stored biometric has different type (Fingerprint) than provided (Facial)
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
            new BiometricData(BiometricType.Fingerprint, storedBytes, storedHash), // Different type
            "zone-001",
            null);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.MatchScore.ShouldBe(0.0); // Different types return 0.0
        result.IsVerified.ShouldBeFalse();
    }
}

