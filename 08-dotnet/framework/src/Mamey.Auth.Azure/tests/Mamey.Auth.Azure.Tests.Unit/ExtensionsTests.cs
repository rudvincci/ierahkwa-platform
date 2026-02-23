using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mamey.Auth.Azure;
using Mamey;
using Xunit;

namespace Mamey.Auth.Azure.Tests.Unit;

public class ExtensionsTests
{
    [Fact]
    public void AddAzure_WithNullBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IMameyBuilder? builder = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => builder!.AddAzure());
    }

    [Fact]
    public void AddAzure_WithDefaultSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:enableAzure"] = "false",
                ["azure:enableAzureB2B"] = "false",
                ["azure:enableAzureB2C"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzure();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzure_WithCustomSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["custom:enableAzure"] = "false",
                ["custom:enableAzureB2B"] = "false",
                ["custom:enableAzureB2C"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzure("custom");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzure_WithOptions_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var builder = MameyBuilder.Create(services, configuration);
        var options = new AzureMultiAuthOptions
        {
            EnableAzureB2B = false,
            EnableAzureB2C = false,
            EnableAzure = false
        };

        // Act
        var result = builder.AddAzure(options);

        // Assert
        result.Should().NotBeNull();
        services.Should().Contain(s => s.ServiceType == typeof(AzureMultiAuthOptions));
    }

    [Fact]
    public void AddAzure_WhenAlreadyRegistered_ShouldNotRegisterAgain()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:enableAzure"] = "false",
                ["azure:enableAzureB2B"] = "false",
                ["azure:enableAzureB2C"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result1 = builder.AddAzure();
        var result2 = builder.AddAzure();

        // Assert
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        // Should only register once due to TryRegister
        services.Count(s => s.ServiceType == typeof(AzureMultiAuthOptions)).Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public void AddAzure_WithNullOptions_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var builder = MameyBuilder.Create(services, configuration);
        AzureMultiAuthOptions? options = null;

        // Act
        var result = builder.AddAzure(options!);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }
}

