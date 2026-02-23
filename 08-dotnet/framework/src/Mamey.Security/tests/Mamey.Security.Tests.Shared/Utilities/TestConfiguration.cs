using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Security;
using Mamey.Types;

namespace Mamey.Security.Tests.Shared.Utilities;

/// <summary>
/// Provides test configuration utilities for security tests.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Creates a test SecurityOptions with encryption enabled.
    /// </summary>
    public static SecurityOptions CreateSecurityOptionsWithEncryption()
    {
        return new SecurityOptions
        {
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "12345678901234567890123456789012" // 32 characters for AES-256
            },
            Certificate = new SecurityOptions.CertificateOptions
            {
                Location = "",
                Password = ""
            }
        };
    }

    /// <summary>
    /// Creates a test SecurityOptions with encryption disabled.
    /// </summary>
    public static SecurityOptions CreateSecurityOptionsWithoutEncryption()
    {
        return new SecurityOptions
        {
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = false,
                Key = ""
            }
        };
    }

    /// <summary>
    /// Creates a test SecurityOptions with invalid key length.
    /// </summary>
    public static SecurityOptions CreateSecurityOptionsWithInvalidKey()
    {
        return new SecurityOptions
        {
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "invalid" // Too short
            }
        };
    }

    /// <summary>
    /// Creates a test IMameyBuilder with security configured.
    /// </summary>
    public static IMameyBuilder CreateTestMameyBuilder(bool encryptionEnabled = true)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", encryptionEnabled.ToString() },
                { "security:encryption:key", "12345678901234567890123456789012" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        
        var builder = MameyBuilder.Create(services, configuration);
        return builder;
    }
}

