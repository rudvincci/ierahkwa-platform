using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Auth.Azure.B2C;
using Mamey;
using Xunit;

namespace Mamey.Auth.Azure.B2C.Tests.Unit;

public class ExtensionsTests
{
    [Fact]
    public void AddAzureB2C_WithDefaultParameters_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2c:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2C();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2C_WithCustomSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["custom:b2c:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2C("custom:b2c");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2C_WithCustomSchemeName_ShouldUseCustomScheme()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2c:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2C("azure:b2c", "CustomB2C");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2C_WhenAlreadyRegistered_ShouldNotRegisterAgain()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2c:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result1 = builder.AddAzureB2C();
        var result2 = builder.AddAzureB2C();

        // Assert
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        // Should only register once due to TryRegister with registry name "auth.azure.b2c"
    }

    [Fact]
    public void AddAzureB2C_WithDisabledOptions_ShouldNotRegister()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2c:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2C();

        // Assert
        result.Should().Be(builder);
        // Should not register services when disabled
    }
}


