using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.Entities;

/// <summary>
/// Comprehensive state transition tests for Identity entity.
/// Tests all valid and invalid state transitions.
/// </summary>
public class IdentityStateTransitionTests
{
    private Identity CreateTestIdentity()
    {
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

    #region Pending → Verified Transitions

    [Fact]
    public void VerifyBiometric_WhenPending_ShouldTransitionToVerified()
    {
        // Arrange - Business Rule: Pending → Verified via biometric verification
        var identity = CreateTestIdentity();
        identity.Status.ShouldBe(IdentityStatus.Pending);
        
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        // Act
        identity.VerifyBiometric(providedBiometric, 0.85);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldNotBeNull();
        identity.Events.ShouldContain(e => e is IdentityVerified);
    }

    [Fact]
    public void VerifyBiometric_WhenVerified_ShouldRemainVerified()
    {
        // Arrange - Business Rule: Verified → Verified (idempotent)
        var identity = CreateTestIdentity();
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        
        identity.VerifyBiometric(providedBiometric, 0.85);
        identity.Status.ShouldBe(IdentityStatus.Verified);
        var firstVerifiedAt = identity.VerifiedAt;

        // Act - Verify again
        identity.VerifyBiometric(providedBiometric, 0.85);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Verified);
        identity.VerifiedAt.ShouldBe(firstVerifiedAt); // Should remain unchanged
    }

    #endregion

    #region Pending → Revoked Transitions

    [Fact]
    public void Revoke_WhenPending_ShouldTransitionToRevoked()
    {
        // Arrange - Business Rule: Pending → Revoked via revocation
        var identity = CreateTestIdentity();
        identity.Status.ShouldBe(IdentityStatus.Pending);

        // Act
        identity.Revoke("Test revocation", Guid.NewGuid());

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldNotBeNull();
        identity.Events.ShouldContain(e => e is IdentityRevoked);
    }

    #endregion

    #region Verified → Revoked Transitions

    [Fact]
    public void Revoke_WhenVerified_ShouldTransitionToRevoked()
    {
        // Arrange - Business Rule: Verified → Revoked via revocation
        var identity = CreateTestIdentity();
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        
        identity.VerifyBiometric(providedBiometric, 0.85);
        identity.Status.ShouldBe(IdentityStatus.Verified);

        // Act
        identity.Revoke("Test revocation", Guid.NewGuid());

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldNotBeNull();
        identity.Events.ShouldContain(e => e is IdentityRevoked);
    }

    #endregion

    #region Revoked → (Cannot Re-verify)

    [Fact]
    public void VerifyBiometric_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Revoked → (cannot re-verify)
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        identity.Status.ShouldBe(IdentityStatus.Revoked);

        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.VerifyBiometric(providedBiometric, 0.85));
        
        identity.Status.ShouldBe(IdentityStatus.Revoked); // Status should remain unchanged
    }

    [Fact]
    public void Revoke_WhenRevoked_ShouldRemainRevoked()
    {
        // Arrange - Business Rule: Revoked → Revoked (idempotent)
        var identity = CreateTestIdentity();
        var firstRevokedBy = Guid.NewGuid();
        identity.Revoke("First revocation", firstRevokedBy);
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        var firstRevokedAt = identity.RevokedAt;

        // Act - Revoke again
        identity.Revoke("Second revocation", Guid.NewGuid());

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.RevokedAt.ShouldBe(firstRevokedAt); // Should remain unchanged
    }

    #endregion

    #region Multiple Rapid State Transitions

    [Fact]
    public void MultipleRapidStateTransitions_ShouldMaintainCorrectState()
    {
        // Arrange - Business Rule: Multiple rapid state transitions should maintain correct state
        var identity = CreateTestIdentity();
        identity.Status.ShouldBe(IdentityStatus.Pending);

        // Act - Rapid transitions: Pending → Verified → Revoked
        var biometricBytes = new byte[] { 1, 2, 3 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricBytes)).ToLowerInvariant();
        var providedBiometric = new BiometricData(BiometricType.Fingerprint, biometricBytes, biometricHash);
        
        identity.VerifyBiometric(providedBiometric, 0.85);
        identity.Status.ShouldBe(IdentityStatus.Verified);

        identity.Revoke("Test revocation", Guid.NewGuid());
        identity.Status.ShouldBe(IdentityStatus.Revoked);

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Revoked);
        identity.VerifiedAt.ShouldNotBeNull();
        identity.RevokedAt.ShouldNotBeNull();
    }

    #endregion

    #region State Transition with Invalid Previous State

    [Fact]
    public void UpdateBiometric_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot update biometric when revoked
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        identity.Status.ShouldBe(IdentityStatus.Revoked);

        var newBiometricBytes = new byte[] { 4, 5, 6 };
        var newBiometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(newBiometricBytes)).ToLowerInvariant();
        var newBiometric = new BiometricData(BiometricType.Facial, newBiometricBytes, newBiometricHash);

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateBiometric(newBiometric, null, null, false));
        
        identity.Status.ShouldBe(IdentityStatus.Revoked); // Status should remain unchanged
    }

    [Fact]
    public void UpdateContactInformation_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot update contact information when revoked
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        identity.Status.ShouldBe(IdentityStatus.Revoked);

        var newContactInfo = new ContactInformation(
            new Email("updated@example.com"),
            new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
            new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
        );

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateContactInformation(newContactInfo));
        
        identity.Status.ShouldBe(IdentityStatus.Revoked); // Status should remain unchanged
    }

    [Fact]
    public void UpdateZone_WhenRevoked_ShouldThrowException()
    {
        // Arrange - Business Rule: Cannot update zone when revoked
        var identity = CreateTestIdentity();
        identity.Revoke("Test revocation", Guid.NewGuid());
        identity.Status.ShouldBe(IdentityStatus.Revoked);

        // Act & Assert
        Should.Throw<InvalidOperationException>(
            () => identity.UpdateZone("zone-002"));
        
        identity.Status.ShouldBe(IdentityStatus.Revoked); // Status should remain unchanged
    }

    #endregion
}

