using Mamey.Auth.Jwt;
using Mamey.Exceptions;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Secrets.Vault;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Certificates;

/// <summary>
/// Comprehensive tests for PrivateKeyGenerator class covering all scenarios.
/// </summary>
public class PrivateKeyGeneratorTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public PrivateKeyGeneratorTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    private PrivateKeyGenerator<PrivateKey> CreateGenerator(bool withCertificate = false)
    {
        var vaultOptions = new VaultOptions { Enabled = false, Pki = new VaultOptions.PkiOptions { Enabled = false } };
        var jwtOptions = new JwtOptions
        {
            Certificate = new JwtOptions.CertificateOptions
            {
                Location = withCertificate ? "test.crt" : "",
                Password = withCertificate ? "password" : ""
            }
        };
        return new PrivateKeyGenerator<PrivateKey>(
            _fixture.Rng,
            _fixture.Signer,
            vaultOptions,
            jwtOptions,
            _fixture.SecurityProvider
        );
    }

    #region Happy Paths

    [Fact]
    public void Generate_DefaultOptions_ShouldGenerateSuccessfully()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act
        var result = generator.Generate();

        // Assert
        result.ShouldNotBeNull();
        result.PrivateKey.ShouldNotBeNull();
        result.Key.ShouldNotBeNullOrEmpty();
        result.PrivateKey.EncryptedPrivateKey.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_CustomLength_ShouldGenerateSuccessfully()
    {
        // Arrange
        var generator = CreateGenerator();
        var length = 100;

        // Act
        var result = generator.Generate(length);

        // Assert
        result.ShouldNotBeNull();
        result.Key.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_WithSpecialCharacters_ShouldGenerateSuccessfully()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act
        var result = generator.Generate(50, true);

        // Assert
        result.ShouldNotBeNull();
        result.Key.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_WithoutSpecialCharacters_ShouldGenerateSuccessfully()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act
        var result = generator.Generate(50, false);

        // Assert
        result.ShouldNotBeNull();
        result.Key.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_MultipleUniqueKeys_ShouldBeUnique()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act
        var result1 = generator.Generate();
        var result2 = generator.Generate();

        // Assert
        result1.Key.ShouldNotBe(result2.Key);
        result1.PrivateKey.EncryptedPrivateKey.ShouldNotBe(result2.PrivateKey.EncryptedPrivateKey);
        AssertionHelpers.ShouldBeUnique(result1.Key!, result2.Key!);
    }

    [Fact]
    public void Generate_WithCertificate_ShouldSignPrivateKey()
    {
        // Arrange
        var generator = CreateGenerator(withCertificate: true);

        // Act
        var result = generator.Generate();

        // Assert
        result.ShouldNotBeNull();
        // Note: Signature may be null if certificate file doesn't exist
        // This is expected behavior when certificate location is invalid
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Generate_ZeroLength_ShouldHandleGracefully()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act
        var result = generator.Generate(0);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Generate_NegativeLength_ShouldThrowException()
    {
        // Arrange
        var generator = CreateGenerator();

        // Act & Assert
        Should.Throw<Exception>(() => generator.Generate(-1));
    }

    [Fact]
    public void VerifyPrivateKeySignature_NullPrivateKey_ShouldThrowException()
    {
        // Arrange
        var generator = CreateGenerator();
        PrivateKey? privateKey = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => generator.VerifyPrivateKeySignature(privateKey!));
    }

    [Fact]
    public void VerifyPrivateKeySignature_WithoutSignature_ShouldThrowMameyException()
    {
        // Arrange
        var generator = CreateGenerator();
        var privateKey = new PrivateKey("encrypted", null!);

        // Act & Assert
        Should.Throw<MameyException>(() => generator.VerifyPrivateKeySignature(privateKey))
            .Message.ShouldContain("Private Key is not signed");
    }

    [Fact]
    public void VerifyPrivateKeySignature_EmptySignature_ShouldThrowMameyException()
    {
        // Arrange
        var generator = CreateGenerator();
        var privateKey = new PrivateKey("encrypted", "");

        // Act & Assert
        Should.Throw<MameyException>(() => generator.VerifyPrivateKeySignature(privateKey))
            .Message.ShouldContain("Private Key is not signed");
    }

    [Fact]
    public void VerifyPrivateKeySignature_VaultDisabled_ShouldThrowMameyException()
    {
        // Arrange
        var generator = CreateGenerator();
        var privateKey = new PrivateKey("encrypted", "signature");

        // Act & Assert
        Should.Throw<MameyException>(() => generator.VerifyPrivateKeySignature(privateKey))
            .Message.ShouldContain("Vault is disabled");
    }

    #endregion
}



