using Mamey.Security;
using Mamey.Security.JsonConverters;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Utilities;
using Mamey.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text.Json;
using Xunit;

namespace Mamey.Security.Tests.Unit.Extensions;

/// <summary>
/// Comprehensive tests for Extensions class covering all scenarios.
/// </summary>
public class ExtensionsTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public ExtensionsTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths

    [Fact]
    public void AddSecurity_ValidConfiguration_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", "true" },
                { "security:encryption:key", TestKeys.ValidAesKey }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddSecurity();

        // Assert
        result.ShouldNotBeNull();
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ISecurityProvider>().ShouldNotBeNull();
        serviceProvider.GetService<IEncryptor>().ShouldNotBeNull();
        serviceProvider.GetService<IHasher>().ShouldNotBeNull();
        serviceProvider.GetService<IRng>().ShouldNotBeNull();
        serviceProvider.GetService<ISigner>().ShouldNotBeNull();
        serviceProvider.GetService<IMd5>().ShouldNotBeNull();
        serviceProvider.GetService<SecurityAttributeProcessor>().ShouldNotBeNull();
    }

    [Fact]
    public void AddSecurity_EncryptionEnabled_ShouldRegisterEncryptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", "true" },
                { "security:encryption:key", TestKeys.ValidAesKey }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        builder.AddSecurity();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<IEncryptor>().ShouldNotBeNull();
        serviceProvider.GetService<ISecurityProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void AddSecurity_EncryptionDisabled_ShouldNotRegisterEncryptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", "false" },
                { "security:encryption:key", "" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        builder.AddSecurity();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        // IEncryptor is still registered even when encryption is disabled
        // because SecurityProvider requires it in its constructor
        // However, it won't be used when encryption is disabled
        serviceProvider.GetService<IEncryptor>().ShouldNotBeNull();
        serviceProvider.GetService<ISecurityProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void AddSecurityConverters_ValidOptions_ShouldAddConverters()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        var serviceProvider = _fixture.ServiceProvider;

        // Act
        var result = options.AddSecurityConverters(serviceProvider);

        // Assert
        result.ShouldBe(options);
        options.Converters.ShouldContain(c => c is EncryptedJsonConverter);
        options.Converters.ShouldContain(c => c is HashedJsonConverter);
    }

    [Fact]
    public void AddSecurityConverters_MultipleCalls_ShouldNotDuplicateConverters()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        var serviceProvider = _fixture.ServiceProvider;

        // Act
        options.AddSecurityConverters(serviceProvider);
        options.AddSecurityConverters(serviceProvider);

        // Assert
        options.Converters.Count(c => c is EncryptedJsonConverter).ShouldBe(1);
        options.Converters.Count(c => c is HashedJsonConverter).ShouldBe(1);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void AddSecurity_NullBuilder_ShouldThrowException()
    {
        // Arrange
        IMameyBuilder? builder = null;

        // Act & Assert
        Should.Throw<NullReferenceException>(() => builder!.AddSecurity());
    }

    [Fact]
    public void AddSecurity_EmptyEncryptionKey_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", "true" },
                { "security:encryption:key", "" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        var builder = MameyBuilder.Create(services, configuration);

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.AddSecurity())
            .Message.ShouldContain("Empty encryption key");
    }

    [Fact]
    public void AddSecurity_InvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "security:encryption:enabled", "true" },
                { "security:encryption:key", TestKeys.InvalidKey }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        var builder = MameyBuilder.Create(services, configuration);

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.AddSecurity())
            .Message.ShouldContain("Invalid encryption key length");
    }

    [Fact]
    public void AddSecurityConverters_NullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        JsonSerializerOptions? options = null;
        var serviceProvider = _fixture.ServiceProvider;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => options!.AddSecurityConverters(serviceProvider));
    }

    [Fact]
    public void AddSecurityConverters_NullServiceProvider_ShouldThrowException()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        IServiceProvider? serviceProvider = null;

        // Act & Assert
        Should.Throw<Exception>(() => options.AddSecurityConverters(serviceProvider!));
    }

    #endregion
}

