using Mamey.FWID.Identities.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.ValueObjects;

public class BiometricDataTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBiometricData()
    {
        // Arrange
        var type = BiometricType.Fingerprint;
        var encryptedTemplate = new byte[] { 1, 2, 3 };
        var hash = "test-hash";

        // Act
        var biometricData = new BiometricData(type, encryptedTemplate, hash);

        // Assert
        biometricData.ShouldNotBeNull();
        biometricData.Type.ShouldBe(type);
        biometricData.EncryptedTemplate.ShouldBe(encryptedTemplate);
        biometricData.Hash.ShouldBe(hash);
        biometricData.CapturedAt.ShouldNotBe(default);
    }

    [Fact]
    public void Constructor_WithEmptyEncryptedTemplate_ShouldThrowException()
    {
        // Arrange
        var type = BiometricType.Fingerprint;
        var encryptedTemplate = Array.Empty<byte>(); // Empty template should throw exception
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant();

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new BiometricData(type, encryptedTemplate, hash));
    }

    [Fact]
    public void Constructor_WithNullHash_ShouldThrowException()
    {
        // Arrange
        var type = BiometricType.Fingerprint;
        var encryptedTemplate = new byte[] { 1, 2, 3 };
        string? hash = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(
            () => new BiometricData(type, encryptedTemplate, hash!));
    }

    [Fact]
    public void Constructor_WithEmptyHash_ShouldThrowException()
    {
        // Arrange
        var type = BiometricType.Fingerprint;
        var encryptedTemplate = new byte[] { 1, 2, 3 };
        var hash = "";

        // Act & Assert
        Should.Throw<ArgumentException>(
            () => new BiometricData(type, encryptedTemplate, hash));
    }

    [Fact]
    public void Match_WithSameHash_ShouldReturnPerfectMatch()
    {
        // Arrange - Business Rule: When hashes match, return perfect match (1.0)
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // In tests, we use realistic 128-character hex strings to simulate stored hashes
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant();
        var biometricData1 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, hash);
        var biometricData2 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, hash);

        // Act
        var matchScore = biometricData1.Match(biometricData2);

        // Assert
        matchScore.ShouldBe(1.0); // Same hash = perfect match
    }

    [Fact]
    public void Match_WithDifferentType_ShouldReturnZero()
    {
        // Arrange
        var biometricData1 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());
        var biometricData2 = new BiometricData(BiometricType.Facial, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());

        // Act
        var matchScore = biometricData1.Match(biometricData2);

        // Assert
        matchScore.ShouldBe(0.0);
    }

    [Fact]
    public void Match_WithNull_ShouldReturnZero()
    {
        // Arrange
        var biometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());

        // Act
        var matchScore = biometricData.Match(null!);

        // Assert
        matchScore.ShouldBe(0.0);
    }

    [Fact]
    public void Match_WithSimilarTemplates_ShouldReturnMatchScore()
    {
        // Arrange - Business Rule: When hashes differ, calculate match score from templates
        // Note: Hash property is marked with [Hashed] and will be processed by SecurityAttributeProcessor
        // Different hashes mean we fall back to template matching
        var hash1 = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant();
        var hash2 = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 4 })).ToLowerInvariant();
        var biometricData1 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, hash1);
        var biometricData2 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 4 }, hash2);

        // Act
        var matchScore = biometricData1.Match(biometricData2);

        // Assert
        matchScore.ShouldBeGreaterThan(0.0); // Similar templates = partial match
        matchScore.ShouldBeLessThan(1.0); // Different hashes = not perfect match
        // 2 out of 3 bytes match = 0.666... match score
        matchScore.ShouldBe(2.0 / 3.0, 0.01);
    }

    [Fact]
    public void Equals_WithSameTypeAndHash_ShouldReturnTrue()
    {
        // Arrange
        var biometricData1 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());
        var biometricData2 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant());

        // Act
        var result = biometricData1.Equals(biometricData2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentHash_ShouldReturnFalse()
    {
        // Arrange
        var biometricData1 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, "hash1");
        var biometricData2 = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, "hash2");

        // Act
        var result = biometricData1.Equals(biometricData2);

        // Assert
        result.ShouldBeFalse();
    }
}

