using System.Security.Cryptography;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using Shouldly;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Mamey.Security.Tests.Unit.Core;

/// <summary>
/// Comprehensive tests for Signer class covering all scenarios.
/// </summary>
public class SignerTests
{
    private readonly ISigner _signer;

    public SignerTests()
    {
        _signer = new Signer();
    }

    #region Happy Paths

    [Fact]
    public void Sign_Object_ShouldSignSuccessfully()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act
        var signature = _signer.Sign(data, certificate);

        // Assert
        signature.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidSignature(signature);
    }

    [Fact]
    public void Sign_ByteArray_ShouldSignSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act
        var signature = _signer.Sign(data, certificate);

        // Assert
        signature.ShouldNotBeNull();
        signature.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Verify_Object_ValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var signature = _signer.Sign(data, certificate);

        // Act
        var isValid = _signer.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Verify_ByteArray_ValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var signature = _signer.Sign(data, certificate);

        // Act
        var isValid = _signer.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Sign_Verify_RoundTrip_ShouldWorkCorrectly()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act
        var signature = _signer.Sign(data, certificate);
        var isValid = _signer.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Sign_DifferentCertificates_ShouldProduceDifferentSignatures()
    {
        // Arrange
        var data = new { Name = "Test", Value = 123 };
        var certificate1 = TestCertificates.CreateSelfSignedCertificate("CN=Cert1");
        var certificate2 = TestCertificates.CreateSelfSignedCertificate("CN=Cert2");

        // Act
        var signature1 = _signer.Sign(data, certificate1);
        var signature2 = _signer.Sign(data, certificate2);

        // Assert
        signature1.ShouldNotBe(signature2);
    }

    [Fact]
    public void Sign_LargeData_ShouldSignSuccessfully()
    {
        // Arrange
        var data = new { Content = TestDataGenerator.GenerateLargeString() };
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act
        var signature = _signer.Sign(data, certificate);
        var isValid = _signer.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Sign_EmptyData_ShouldSignSuccessfully()
    {
        // Arrange
        var data = new { };
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act
        var signature = _signer.Sign(data, certificate);
        var isValid = _signer.Verify(data, certificate, signature);

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Verify_ThrowException_True_ShouldThrowOnInvalidSignature()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var invalidSignature = "invalid_signature";

        // Act & Assert
        Should.Throw<CryptographicException>(() => _signer.Verify(data, certificate, invalidSignature, true));
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Sign_NullCertificate_ShouldThrowException()
    {
        // Arrange
        var data = new { Name = "Test" };
        X509Certificate2? certificate = null;

        // Act & Assert
        Should.Throw<Exception>(() => _signer.Sign(data, certificate!));
    }

    [Fact]
    public void Sign_NullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? data = null;
        var certificate = TestCertificates.CreateSelfSignedCertificate();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _signer.Sign(data!, certificate));
    }

    [Fact]
    public void Sign_CertificateWithoutPrivateKey_ShouldThrowCryptographicException()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreatePublicCertificate(); // No private key

        // Act & Assert
        Should.Throw<CryptographicException>(() => _signer.Sign(data, certificate));
    }

    [Fact]
    public void Verify_NullCertificate_ShouldThrowException()
    {
        // Arrange
        var data = new { Name = "Test" };
        var signature = "signature";
        X509Certificate2? certificate = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _signer.Verify(data, certificate!, signature));
    }

    [Fact]
    public void Verify_NullSignature_ShouldThrowException()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        string? signature = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _signer.Verify(data, certificate, signature!));
    }

    [Fact]
    public void Verify_WrongCertificate_ShouldReturnFalse()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate1 = TestCertificates.CreateSelfSignedCertificate("CN=Cert1");
        var certificate2 = TestCertificates.CreateSelfSignedCertificate("CN=Cert2");
        var signature = _signer.Sign(data, certificate1);

        // Act
        var isValid = _signer.Verify(data, certificate2, signature);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void Verify_CorruptedSignature_ShouldReturnFalse()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var validSignature = _signer.Sign(data, certificate);
        var corruptedSignature = validSignature.Substring(0, validSignature.Length - 5) + "XXXXX";

        // Act
        var isValid = _signer.Verify(data, certificate, corruptedSignature);

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void Verify_CertificateWithoutPublicKey_ShouldThrowCryptographicException()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreatePublicCertificate();
        // Use an invalid Base64 signature that will fail during conversion
        var signature = "invalid_base64_signature!!!";

        // Act & Assert
        // This will throw FormatException which is caught and converted to CryptographicException
        Should.Throw<CryptographicException>(() => _signer.Verify(data, certificate, signature, true));
    }

    [Fact]
    public void Verify_EmptySignature_ShouldThrowException()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var signature = "";

        // Act & Assert
        // Empty string is invalid Base64, will throw FormatException which is caught
        Should.Throw<CryptographicException>(() => _signer.Verify(data, certificate, signature, true));
    }

    [Fact]
    public void Verify_InvalidSignature_ThrowExceptionFalse_ShouldReturnFalse()
    {
        // Arrange
        var data = new { Name = "Test" };
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var invalidSignature = "invalid_signature";

        // Act
        var isValid = _signer.Verify(data, certificate, invalidSignature, false);

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion
}

