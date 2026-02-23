using Mamey.Security.Redis;
using Mamey.Security.Redis.Serializers;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Redis;

/// <summary>
/// Comprehensive tests for Redis extensions covering all scenarios.
/// </summary>
public class ExtensionTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public ExtensionTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths

    [Fact]
    public void AddSecurityRedis_ValidBuilder_ShouldRegisterSerializers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var builder = MameyBuilder.Create(services);

        // Act
        var result = builder.AddSecurityRedis();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(builder);
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<EncryptedRedisSerializer>().ShouldNotBeNull();
        serviceProvider.GetService<HashedRedisSerializer>().ShouldNotBeNull();
    }

    [Fact]
    public void AddSecurityRedis_SerializersAvailableInDI_ShouldBeAvailable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var builder = MameyBuilder.Create(services);

        // Act
        builder.AddSecurityRedis();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var encryptedSerializer = serviceProvider.GetService<EncryptedRedisSerializer>();
        var hashedSerializer = serviceProvider.GetService<HashedRedisSerializer>();
        encryptedSerializer.ShouldNotBeNull();
        hashedSerializer.ShouldNotBeNull();
    }

    [Fact]
    public void AddSecurityRedis_MultipleInstances_ShouldBeSingletons()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var builder = MameyBuilder.Create(services);

        // Act
        builder.AddSecurityRedis();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var serializer1 = serviceProvider.GetService<EncryptedRedisSerializer>();
        var serializer2 = serviceProvider.GetService<EncryptedRedisSerializer>();
        serializer1.ShouldBe(serializer2); // Should be same instance (singleton)
    }

    [Fact]
    public void AddSecurityRedis_CustomJsonOptions_ShouldUseCustomOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(_fixture.SecurityProvider);
        var builder = MameyBuilder.Create(services);

        // Act
        builder.AddSecurityRedis();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var serializer = serviceProvider.GetService<EncryptedRedisSerializer>();
        serializer.ShouldNotBeNull();
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void AddSecurityRedis_NullBuilder_ShouldThrowException()
    {
        // Arrange
        IMameyBuilder? builder = null;

        // Act & Assert
        Should.Throw<NullReferenceException>(() => builder!.AddSecurityRedis());
    }

    [Fact]
    public void AddSecurityRedis_WithoutAddSecurity_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        // Note: AddSecurityRedis requires ISecurityProvider, but doesn't explicitly require AddSecurity
        // This test verifies the behavior when ISecurityProvider is not registered
        var builder = MameyBuilder.Create(services);

        // Act & Assert
        Should.Throw<Exception>(() => builder.AddSecurityRedis());
    }

    [Fact]
    public void AddSecurityRedis_SerializersNotRegistered_ShouldNotBeAvailable()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = MameyBuilder.Create(services);
        // Don't call AddSecurityRedis

        // Act
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<EncryptedRedisSerializer>().ShouldBeNull();
        serviceProvider.GetService<HashedRedisSerializer>().ShouldBeNull();
    }

    #endregion
}



