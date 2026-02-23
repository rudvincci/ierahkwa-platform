using Mamey.Exceptions;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using Mamey.Secrets.Vault;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Certificates;

/// <summary>
/// Comprehensive tests for PrivateKeyService class covering all scenarios.
/// </summary>
public class PrivateKeyServiceTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;
    private readonly PrivateKeyService _service;

    public PrivateKeyServiceTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
        var vaultOptions = new VaultOptions { Enabled = false, Pki = new VaultOptions.PkiOptions { Enabled = false } };
        var securityOptions = new SecurityOptions
        {
            EncryptionKey = TestKeys.ValidAesKey,
            Certificate = new SecurityOptions.CertificateOptions { Location = "", Password = "" }
        };
        _service = new PrivateKeyService(
            _fixture.Rng,
            _fixture.Encryptor,
            _fixture.Signer,
            vaultOptions,
            securityOptions
        );
    }

    #region Happy Paths

    [Fact]
    public void Generate_DefaultLength_ShouldGenerateSuccessfully()
    {
        // Act
        var (privateKey, plainKey) = _service.Generate();

        // Assert
        privateKey.ShouldNotBeNull();
        privateKey.EncryptedPrivateKey.ShouldNotBeNullOrEmpty();
        plainKey.ShouldNotBeNullOrEmpty();
        plainKey.ShouldNotBe(privateKey.EncryptedPrivateKey);
    }

    [Fact]
    public void Generate_CustomLength_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 100;

        // Act
        var (privateKey, plainKey) = _service.Generate(length);

        // Assert
        privateKey.ShouldNotBeNull();
        privateKey.EncryptedPrivateKey.ShouldNotBeNullOrEmpty();
        plainKey.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_MultipleUniqueKeys_ShouldBeUnique()
    {
        // Act
        var (key1, plain1) = _service.Generate();
        var (key2, plain2) = _service.Generate();

        // Assert
        key1.EncryptedPrivateKey.ShouldNotBe(key2.EncryptedPrivateKey);
        plain1.ShouldNotBe(plain2);
        AssertionHelpers.ShouldBeUnique(plain1, plain2);
    }

    [Fact]
    public void Generate_DifferentLengths_ShouldGenerateSuccessfully()
    {
        // Act
        var (key1, _) = _service.Generate(50);
        var (key2, _) = _service.Generate(100);

        // Assert
        key1.ShouldNotBeNull();
        key2.ShouldNotBeNull();
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Generate_ZeroLength_ShouldHandleGracefully()
    {
        // Act
        var (privateKey, plainKey) = _service.Generate(0);

        // Assert
        privateKey.ShouldNotBeNull();
        plainKey.ShouldNotBeNull();
    }

    [Fact]
    public void Generate_NegativeLength_ShouldThrowException()
    {
        // Act & Assert
        Should.Throw<Exception>(() => _service.Generate(-1));
    }

    [Fact]
    public void ValidatePrivateKey_NullPrivateKey_ShouldThrowException()
    {
        // Arrange
        PrivateKey? privateKey = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _service.ValidatePrivateKey(privateKey!));
    }

    [Fact]
    public void ValidatePrivateKey_WithoutSignature_ShouldThrowMameyException()
    {
        // Arrange
        var privateKey = new PrivateKey("encrypted", null!);

        // Act & Assert
        Should.Throw<MameyException>(() => _service.ValidatePrivateKey(privateKey))
            .Message.ShouldContain("Private Key is not signed");
    }

    [Fact]
    public void ValidatePrivateKey_EmptySignature_ShouldThrowMameyException()
    {
        // Arrange
        var privateKey = new PrivateKey("encrypted", "");

        // Act & Assert
        Should.Throw<MameyException>(() => _service.ValidatePrivateKey(privateKey))
            .Message.ShouldContain("Private Key is not signed");
    }

    [Fact]
    public void ValidatePrivateKey_VaultDisabled_ShouldThrowMameyException()
    {
        // Arrange
        var privateKey = new PrivateKey("encrypted", "signature");
        var vaultOptions = new VaultOptions { Enabled = false, Pki = new VaultOptions.PkiOptions { Enabled = false } };
        var securityOptions = new SecurityOptions
        {
            EncryptionKey = TestKeys.ValidAesKey,
            Certificate = new SecurityOptions.CertificateOptions { Location = "", Password = "" }
        };
        var service = new PrivateKeyService(
            _fixture.Rng,
            _fixture.Encryptor,
            _fixture.Signer,
            vaultOptions,
            securityOptions
        );

        // Act & Assert
        Should.Throw<MameyException>(() => service.ValidatePrivateKey(privateKey))
            .Message.ShouldContain("Vault is disabled");
    }

    #endregion
}

