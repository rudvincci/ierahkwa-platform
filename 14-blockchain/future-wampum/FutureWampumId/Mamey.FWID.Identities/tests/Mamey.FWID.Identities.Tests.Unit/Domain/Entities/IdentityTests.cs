using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.Entities;

public class IdentityTests
{
    private Identity CreateTestIdentity()
    {
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // when persisted. In tests, we use realistic 128-character hex strings to simulate stored hashes.
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        
        return new Identity(
            new IdentityId(Guid.NewGuid()),
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
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateIdentity()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var name = new Name("John", "Doe", "M.");
        var personalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan");
        var contactInfo = new ContactInformation(
            new Email("john.doe@example.com"),
            new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
        );
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var biometricData = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        // Act
        var identity = new Identity(identityId, name, personalDetails, contactInfo, biometricData, "zone-001", null);

        // Assert
        identity.ShouldNotBeNull();
        identity.Id.ShouldBe(identityId);
        identity.Name.ShouldBe(name);
        identity.PersonalDetails.ShouldBe(personalDetails);
        identity.ContactInformation.ShouldBe(contactInfo);
        identity.BiometricData.ShouldBe(biometricData);
        identity.Status.ShouldBe(IdentityStatus.Pending);
        identity.Zone.ShouldBe("zone-001");
        identity.Events.ShouldContain(e => e is IdentityCreated);
    }

    [Fact]
    public void VerifyBiometric_WithMatchingBiometric_ShouldVerifyIdentity()
    {
        // Arrange - Business Rule: Matching biometrics verify the identity
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identity = CreateTestIdentity();
        var biometricData = new byte[] { 1, 2, 3 };
        // Use same hash as stored biometric to get perfect match
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricData)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricData, biometricHash);
        var threshold = 0.95;

        // Act
        identity.VerifyBiometric(providedBiometric, threshold);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldNotBeNull();
        identity.LastVerifiedAt.ShouldNotBeNull();
        identity.Events.ShouldContain(e => e is IdentityVerified);
    }

    [Fact]
    public void VerifyBiometric_WithNonMatchingBiometric_ShouldThrowException()
    {
        // Arrange - Business Rule: Non-matching biometrics fail verification
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identity = CreateTestIdentity();
        var differentBytes = new byte[] { 9, 9, 9 };
        var differentHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(differentBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, differentBytes, differentHash);
        var threshold = 0.95;

        // Act & Assert
        var exception = Should.Throw<BiometricVerificationFailedException>(
            () => identity.VerifyBiometric(providedBiometric, threshold));

        exception.IdentityId.ShouldBe(identity.Id);
        exception.MatchScore.ShouldBeLessThan(threshold);
        identity.Status.ShouldBe(IdentityStatus.Pending);
    }

    [Fact]
    public void VerifyBiometric_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot verify revoked identity
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.VerifyBiometric(providedBiometric));
    }

    [Fact]
    public void Revoke_WithValidReason_ShouldRevokeIdentity()
    {
        // Arrange
        var identity = CreateTestIdentity();
        var reason = "Test revocation";
        var revokedBy = Guid.NewGuid();

        // Act
        identity.Revoke(reason, revokedBy);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldNotBeNull();
        identity.Metadata["RevocationReason"].ShouldBe(reason);
        identity.Metadata["RevokedBy"].ShouldBe(revokedBy.ToString());
        identity.Events.ShouldContain(e => e is IdentityRevoked);
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldNotThrowException()
    {
        // Arrange
        var identity = CreateTestIdentity();
        identity.Revoke("First revocation", Guid.NewGuid());
        var initialRevokedAt = identity.RevokedAt;

        // Act
        identity.Revoke("Second revocation", Guid.NewGuid());

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldBe(initialRevokedAt);
    }

    [Fact]
    public void UpdateBiometric_WithValidBiometric_ShouldUpdateBiometric()
    {
        // Arrange - Business Rule: Update biometric requires verification with old biometric first
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identity = CreateTestIdentity();
        var newBytes = new byte[] { 4, 5, 6 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        
        // Verification biometric must match stored biometric (same hash)
        var verificationBytes = new byte[] { 1, 2, 3 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash);

        // Act
        identity.UpdateBiometric(newBiometric, verificationBiometric);

        // Assert
        identity.BiometricData.ShouldBe(newBiometric);
        identity.Events.ShouldContain(e => e is BiometricUpdated);
    }

    [Fact]
    public void UpdateBiometric_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot update biometric for revoked identity
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        var newBytes = new byte[] { 4, 5, 6 };
        var newHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBytes, newHash);
        
        var verificationBytes = new byte[] { 1, 2, 3 };
        var verificationHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(verificationBytes)).ToLowerInvariant();
        var verificationBiometric = new BiometricData(BiometricType.Fingerprint, verificationBytes, verificationHash);

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateBiometric(newBiometric, verificationBiometric));
    }

    [Fact]
    public void UpdateZone_WithValidZone_ShouldUpdateZone()
    {
        // Arrange
        var identity = CreateTestIdentity();
        var newZone = "zone-002";

        // Act
        identity.UpdateZone(newZone);

        // Assert
        identity.Zone.ShouldBe(newZone);
        identity.Events.ShouldContain(e => e is ZoneUpdated);
    }

    [Fact]
    public void UpdateZone_WhenRevoked_ShouldThrowException()
    {
        // Arrange
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateZone("zone-002"));
    }

    [Fact]
    public void UpdateContactInformation_WithValidContactInfo_ShouldUpdateContactInfo()
    {
        // Arrange
        var identity = CreateTestIdentity();
        var newContactInfo = new ContactInformation(
            new Email("new.email@example.com"),
            new Address("", "456 New St", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559876543", null, Phone.PhoneType.Mobile) }
        );

        // Act
        identity.UpdateContactInformation(newContactInfo);

        // Assert
        identity.ContactInformation.ShouldBe(newContactInfo);
        identity.Events.ShouldContain(e => e is ContactInformationUpdated);
    }

    [Fact]
    public void UpdateContactInformation_WhenRevoked_ShouldThrowException()
    {
        // Arrange
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        var newContactInfo = new ContactInformation(
            new Email("new.email@example.com"),
            new Address("", "456 New St", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559876543", null, Phone.PhoneType.Mobile) }
        );

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateContactInformation(newContactInfo));
    }

    [Fact]
    public void UpdateContactInformation_WithNullContactInfo_ShouldThrowException()
    {
        // Arrange
        var identity = CreateTestIdentity();

        // Act & Assert
        Should.Throw<ArgumentNullException>(
            () => identity.UpdateContactInformation(null!));
    }
}

