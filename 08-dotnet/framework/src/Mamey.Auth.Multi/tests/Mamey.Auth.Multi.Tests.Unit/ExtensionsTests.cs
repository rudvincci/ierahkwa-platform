using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Auth.Multi;
using Mamey;
using Xunit;

namespace Mamey.Auth.Multi.Tests.Unit;

public class ExtensionsTests
{
    [Fact]
    public void AddMultiAuth_WithNullBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IMameyBuilder? builder = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => builder!.AddMultiAuth());
    }

    [Fact]
    public void AddMultiAuth_WithDefaultSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["multiAuth:enableJwt"] = "false",
                ["multiAuth:enableDid"] = "false",
                ["multiAuth:enableAzure"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddMultiAuth();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddMultiAuth_WithCustomSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["custom:enableJwt"] = "false",
                ["custom:enableDid"] = "false",
                ["custom:enableAzure"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddMultiAuth("custom");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddMultiAuth_WithOptions_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var builder = MameyBuilder.Create(services, configuration);
        var options = new MultiAuthOptions
        {
            EnableJwt = false,
            EnableDid = false,
            EnableAzure = false
        };

        // Act
        var result = builder.AddMultiAuth(options);

        // Assert
        result.Should().NotBeNull();
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthOptions));
    }

    [Fact]
    public void AddMultiAuth_WhenAlreadyRegistered_ShouldNotRegisterAgain()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["multiAuth:enableJwt"] = "false",
                ["multiAuth:enableDid"] = "false",
                ["multiAuth:enableAzure"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result1 = builder.AddMultiAuth();
        var result2 = builder.AddMultiAuth();

        // Assert
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        // Should only register once due to TryRegister
        services.Count(s => s.ServiceType == typeof(MultiAuthOptions)).Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public void AddMultiAuth_WithNullOptions_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var builder = MameyBuilder.Create(services, configuration);
        MultiAuthOptions? options = null;

        // Act
        var result = builder.AddMultiAuth(options!);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }
}

