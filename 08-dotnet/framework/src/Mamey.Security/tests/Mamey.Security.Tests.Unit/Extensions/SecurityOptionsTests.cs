using Mamey.Security;
using Mamey.Security.Tests.Shared.Utilities;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Extensions;

/// <summary>
/// Comprehensive tests for SecurityOptions class covering all scenarios.
/// </summary>
public class SecurityOptionsTests
{
    #region Happy Paths

    [Fact]
    public void CreateSecurityOptions_WithAllProperties_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions
        {
            EncryptionKey = TestKeys.ValidAesKey,
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = TestKeys.ValidAesKey
            },
            Recovery = new SecurityOptions.RecoveryOptions
            {
                Issuer = "test-issuer",
                Expiry = TimeSpan.FromHours(1),
                ExpiryMinutes = 60,
                AllowedEndpoints = new[] { "endpoint1", "endpoint2" }
            },
            Certificate = new SecurityOptions.CertificateOptions
            {
                Location = "cert.pfx",
                Password = "password"
            }
        };

        // Assert
        options.ShouldNotBeNull();
        options.EncryptionKey.ShouldBe(TestKeys.ValidAesKey);
        options.Encryption.ShouldNotBeNull();
        options.Encryption.Enabled.ShouldBeTrue();
        options.Encryption.Key.ShouldBe(TestKeys.ValidAesKey);
        options.Recovery.ShouldNotBeNull();
        options.Recovery.Issuer.ShouldBe("test-issuer");
        options.Recovery.Expiry.ShouldBe(TimeSpan.FromHours(1));
        options.Recovery.ExpiryMinutes.ShouldBe(60);
        options.Recovery.AllowedEndpoints.ShouldNotBeNull();
        options.Certificate.ShouldNotBeNull();
        options.Certificate.Location.ShouldBe("cert.pfx");
        options.Certificate.Password.ShouldBe("password");
    }

    [Fact]
    public void CreateEncryptionOptions_WithDefaultValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions.EncryptionOptions
        {
            Enabled = false,
            Key = ""
        };

        // Assert
        options.ShouldNotBeNull();
        options.Enabled.ShouldBeFalse();
        options.Key.ShouldBe("");
    }

    [Fact]
    public void CreateRecoveryOptions_WithDefaultValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions.RecoveryOptions
        {
            Issuer = "",
            Expiry = null,
            ExpiryMinutes = 0,
            AllowedEndpoints = null
        };

        // Assert
        options.ShouldNotBeNull();
        options.Issuer.ShouldBe("");
        options.Expiry.ShouldBeNull();
        options.ExpiryMinutes.ShouldBe(0);
        options.AllowedEndpoints.ShouldBeNull();
    }

    [Fact]
    public void CreateCertificateOptions_WithDefaultValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions.CertificateOptions
        {
            Location = "",
            Password = ""
        };

        // Assert
        options.ShouldNotBeNull();
        options.Location.ShouldBe("");
        options.Password.ShouldBe("");
    }

    [Fact]
    public void CreateSecurityOptions_WithCustomValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions
        {
            EncryptionKey = TestKeys.ValidAesKey,
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = TestKeys.ValidAesKey
            },
            Recovery = new SecurityOptions.RecoveryOptions
            {
                Issuer = "custom-issuer",
                Expiry = TimeSpan.FromDays(1),
                ExpiryMinutes = 1440,
                AllowedEndpoints = new[] { "endpoint1" }
            },
            Certificate = new SecurityOptions.CertificateOptions
            {
                Location = "custom.pfx",
                Password = "custom-password"
            }
        };

        // Assert
        options.ShouldNotBeNull();
        options.Recovery.Issuer.ShouldBe("custom-issuer");
        options.Recovery.Expiry.ShouldBe(TimeSpan.FromDays(1));
        options.Recovery.ExpiryMinutes.ShouldBe(1440);
        options.Certificate.Location.ShouldBe("custom.pfx");
        options.Certificate.Password.ShouldBe("custom-password");
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void CreateSecurityOptions_WithNullValues_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions
        {
            EncryptionKey = null!,
            Encryption = null!,
            Recovery = null!,
            Certificate = null!
        };

        // Assert
        options.ShouldNotBeNull();
        // Null values are allowed in the model
    }

    [Fact]
    public void CreateRecoveryOptions_WithInvalidExpiry_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var options = new SecurityOptions.RecoveryOptions
        {
            Expiry = TimeSpan.FromDays(-1), // Negative expiry
            ExpiryMinutes = -1
        };

        // Assert
        options.ShouldNotBeNull();
        // Invalid values are allowed in the model, validation happens elsewhere
    }

    #endregion
}



