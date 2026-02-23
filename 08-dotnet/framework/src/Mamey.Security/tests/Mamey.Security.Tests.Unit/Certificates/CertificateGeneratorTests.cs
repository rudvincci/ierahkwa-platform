using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Mamey.Security.Tests.Unit.Certificates;

/// <summary>
/// Comprehensive tests for CertificateGenerator class covering all scenarios.
/// </summary>
public class CertificateGeneratorTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public CertificateGeneratorTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths

    [Fact]
    public void Generate_DefaultOptions_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GeneratePrivateKey(Arg.Any<int>(), Arg.Any<bool>())
            .Returns(new PrivateKeyResult<PrivateKey>(privateKey, "key"));
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);

        // Act
        var certificateResult = generator.Generate();

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateResult.Certificate.ShouldNotBeNull();
        certificateResult.PrivateKey.ShouldNotBeNull();
    }

    [Fact]
    public void Generate_CustomSubject_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate("CN=Custom Subject");
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GeneratePrivateKey(Arg.Any<int>(), Arg.Any<bool>())
            .Returns(new PrivateKeyResult<PrivateKey>(privateKey, "key"));
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var subject = "CN=Custom Subject";

        // Act
        var certificateResult = generator.Generate(50, false, subject);

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateResult.Certificate.ShouldNotBeNull();
    }

    [Fact]
    public void GenerateFromPrivateKey_ValidPrivateKey_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);

        // Act
        var certificateResult = generator.GenerateFromPrivateKey(privateKey);

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateResult.Certificate.ShouldNotBeNull();
        certificateResult.PrivateKey.ShouldBe(privateKey);
    }

    [Fact]
    public void GenerateFromPrivateKey_CustomSubject_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate("CN=Custom Subject");
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var subject = "CN=Custom Subject";

        // Act
        var certificateResult = generator.GenerateFromPrivateKey(privateKey, subject);

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateResult.Certificate.ShouldNotBeNull();
    }

    [Fact]
    public void ExportToFile_ValidCertificate_ShouldExportSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var filePath = Path.Combine(Path.GetTempPath(), $"test_cert_{Guid.NewGuid()}.crt");

        try
        {
            // Act
            generator.ExportToFile(certificate, filePath);

            // Assert
            File.Exists(filePath).ShouldBeTrue();
            var fileData = File.ReadAllBytes(filePath);
            fileData.Length.ShouldBeGreaterThan(0);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public void Generate_CustomKeyLength_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GeneratePrivateKey(Arg.Any<int>(), Arg.Any<bool>())
            .Returns(new PrivateKeyResult<PrivateKey>(privateKey, "key"));
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var keyLength = 100;

        // Act
        var certificateResult = generator.Generate(keyLength);

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateProvider.Received(1).GeneratePrivateKey(keyLength, false);
    }

    [Fact]
    public void Generate_SpecialCharacters_ShouldGenerateSuccessfully()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var privateKey = new PrivateKey("encrypted", "signature");
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var result = new CertificateResult<PrivateKey>(privateKey, certificate);
        certificateProvider.GeneratePrivateKey(Arg.Any<int>(), Arg.Any<bool>())
            .Returns(new PrivateKeyResult<PrivateKey>(privateKey, "key"));
        certificateProvider.GenerateFromPrivateKey(Arg.Any<PrivateKey>(), Arg.Any<string>())
            .Returns((ICertificateResult<PrivateKey>)result);
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);

        // Act
        var certificateResult = generator.Generate(50, true);

        // Assert
        certificateResult.ShouldNotBeNull();
        certificateProvider.Received(1).GeneratePrivateKey(50, true);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void GenerateFromPrivateKey_NullPrivateKey_ShouldThrowArgumentNullException()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        PrivateKey? privateKey = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => generator.GenerateFromPrivateKey(privateKey!));
    }

    [Fact]
    public void GenerateFromPrivateKey_NullOrganizationId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = null };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var privateKey = new PrivateKey("encrypted", "signature");

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => generator.GenerateFromPrivateKey(privateKey));
    }

    [Fact]
    public void GenerateFromPrivateKey_EmptyOrganizationId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.Empty };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var privateKey = new PrivateKey("encrypted", "signature");

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => generator.GenerateFromPrivateKey(privateKey));
    }

    [Fact]
    public void ExportToFile_NullCertificate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        X509Certificate2? certificate = null;
        var filePath = "test.crt";

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => generator.ExportToFile(certificate!, filePath));
    }

    [Fact]
    public void ExportToFile_InvalidFilePath_ShouldThrowException()
    {
        // Arrange
        var appOptions = new AppOptions { OrganizationId = Guid.NewGuid() };
        var certificateProvider = Substitute.For<ICertificateProvider<PrivateKey>>();
        var generator = new CertificateGenerator<PrivateKey>(appOptions, certificateProvider);
        var certificate = TestCertificates.CreateSelfSignedCertificate();
        var filePath = "/invalid/path/that/does/not/exist/test.crt";

        // Act & Assert
        Should.Throw<Exception>(() => generator.ExportToFile(certificate, filePath));
    }

    #endregion
}

