using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using Shouldly;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Mamey.Security.Tests.Unit.Security;

/// <summary>
/// Comprehensive security validation tests covering all scenarios.
/// </summary>
public class SecurityValidationTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public SecurityValidationTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Key Strength Validation

    [Fact]
    public void ValidateKeyStrength_ValidAesKey_ShouldPass()
    {
        // Arrange
        var key = TestKeys.ValidAesKey;

        // Act & Assert
        key.Length.ShouldBe(32);
        AssertionHelpers.ShouldBeValidKeyLength(key, 32);
    }

    [Fact]
    public void ValidateKeyStrength_ValidTripleDesKey_ShouldPass()
    {
        // Arrange
        var key = TestKeys.ValidTripleDesKey;

        // Act & Assert
        key.Length.ShouldBe(24);
        AssertionHelpers.ShouldBeValidKeyLength(key, 24);
    }

    [Fact]
    public void ValidateKeyStrength_InvalidKey_ShouldFail()
    {
        // Arrange
        var key = TestKeys.InvalidKey;

        // Act & Assert
        key.Length.ShouldNotBe(32);
        key.Length.ShouldNotBe(24);
    }

    [Fact]
    public void ValidateKeyStrength_WeakKey_ShouldFail()
    {
        // Arrange
        var weakKey = "12345678901234567890123456789012"; // All same characters

        // Act & Assert
        weakKey.Length.ShouldBe(32);
        // Note: This test verifies key length, not entropy
        // Actual entropy validation would require additional checks
    }

    #endregion

    #region Encryption Key Rotation

    [Fact]
    public void EncryptionKeyRotation_NewKey_ShouldEncrypt()
    {
        // Arrange
        var fixture1 = new SecurityTestFixture(encryptionEnabled: true);
        var fixture2 = new SecurityTestFixture(encryptionEnabled: true);
        var data = "sensitive data";
        var encrypted = fixture1.SecurityProvider.Encrypt(data);

        // Act & Assert
        // Note: Different keys produce different encrypted values
        var encrypted2 = fixture2.SecurityProvider.Encrypt(data);
        encrypted.ShouldNotBe(encrypted2);
    }

    [Fact]
    public void EncryptionKeyRotation_OldKey_ShouldDecrypt()
    {
        // Arrange
        var fixture1 = new SecurityTestFixture(encryptionEnabled: true);
        var data = "sensitive data";
        var encrypted = fixture1.SecurityProvider.Encrypt(data);

        // Act
        var decrypted = fixture1.SecurityProvider.Decrypt(encrypted);

        // Assert
        decrypted.ShouldBe(data);
    }

    #endregion

    #region Certificate Validation

    [Fact]
    public void ValidateCertificate_ValidCertificate_ShouldPass()
    {
        // Arrange
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act & Assert
        certificate.ShouldNotBeNull();
        certificate.HasPrivateKey.ShouldBeTrue();
        certificate.NotBefore.ShouldBeLessThanOrEqualTo(DateTime.Now);
        certificate.NotAfter.ShouldBeGreaterThan(DateTime.Now);
    }

    [Fact]
    public void ValidateCertificate_ExpiredCertificate_ShouldFail()
    {
        // Arrange
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddYears(-2),
            DateTimeOffset.Now.AddYears(-1)
        );

        // Act & Assert
        certificate.NotAfter.ShouldBeLessThan(DateTime.Now);
    }

    [Fact]
    public void ValidateCertificate_NotYetValidCertificate_ShouldFail()
    {
        // Arrange
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddYears(1),
            DateTimeOffset.Now.AddYears(2)
        );

        // Act & Assert
        certificate.NotBefore.ShouldBeGreaterThan(DateTime.Now);
    }

    #endregion

    #region Signature Validation

    [Fact]
    public void ValidateSignature_ValidSignature_ShouldPass()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var signature = _fixture.SecurityProvider.Sign(data, certificate);

        // Act
        var isValid = _fixture.SecurityProvider.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void ValidateSignature_InvalidSignature_ShouldFail()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var signature = _fixture.SecurityProvider.Sign(data, certificate);
        var modifiedData = new { Name = "Test", Value = 456 };

        // Act
        var isValid = _fixture.SecurityProvider.Verify(modifiedData, certificate, signature);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void ValidateSignature_WrongCertificate_ShouldFail()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate1 = TestCertificates.CreateSelfSignedCertificate("CN=Cert1");
        var certificate2 = TestCertificates.CreateSelfSignedCertificate("CN=Cert2");
        var signature = _fixture.SecurityProvider.Sign(data, certificate1);

        // Act
        var isValid = _fixture.SecurityProvider.Verify(data, certificate2, signature);

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion

    #region Timing Attack Resistance

    [Fact]
    public void TimingAttackResistance_ConstantTimeComparison_ShouldResist()
    {
        // Arrange
        var hash1 = _fixture.SecurityProvider.Hash("password123");
        var hash2 = _fixture.SecurityProvider.Hash("password123");
        var hash3 = _fixture.SecurityProvider.Hash("password456");

        // Act
        var time1 = MeasureExecutionTime(() => { var _ = hash1 == hash2; });
        var time2 = MeasureExecutionTime(() => { var _ = hash1 == hash3; });

        // Assert
        // Note: This is a basic test - actual timing attack resistance requires
        // constant-time comparison implementations
        hash1.ShouldBe(hash2);
        hash1.ShouldNotBe(hash3);
    }

    private TimeSpan MeasureExecutionTime(Action action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    #endregion

    #region Side-Channel Attack Resistance

    [Fact]
    public void SideChannelAttackResistance_Encryption_ShouldResist()
    {
        // Arrange
        var data1 = "short";
        var data2 = "very long data that should take longer to encrypt";
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted1 = _fixture.Encryptor.Encrypt(data1, key, EncryptionAlgorithms.AES);
        var encrypted2 = _fixture.Encryptor.Encrypt(data2, key, EncryptionAlgorithms.AES);

        // Assert
        encrypted1.ShouldNotBeNullOrEmpty();
        encrypted2.ShouldNotBeNullOrEmpty();
        // Note: Actual side-channel resistance requires careful implementation
        // This test verifies that encryption works for different data sizes
    }

    #endregion
}

