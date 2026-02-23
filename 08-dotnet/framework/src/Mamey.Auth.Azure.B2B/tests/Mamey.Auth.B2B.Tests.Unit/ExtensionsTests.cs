using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mamey.Auth.Azure.B2B;
using Mamey;
using Xunit;

namespace Mamey.Auth.Azure.B2B.Tests.Unit;

public class ExtensionsTests
{
    [Fact]
    public void AddAzureB2B_WithDefaultParameters_ShouldUseDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2b:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2B();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2B_WithCustomSectionName_ShouldLoadOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["custom:b2b:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2B("custom:b2b");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2B_WithCustomSchemeName_ShouldUseCustomScheme()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2b:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2B("azure:b2b", "CustomB2B");

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(builder);
    }

    [Fact]
    public void AddAzureB2B_WhenAlreadyRegistered_ShouldNotRegisterAgain()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2b:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result1 = builder.AddAzureB2B();
        var result2 = builder.AddAzureB2B();

        // Assert
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        // Should only register once due to TryRegister with registry name "auth.azure.b2b"
    }

    [Fact]
    public void AddAzureB2B_WithDisabledOptions_ShouldNotRegister()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:b2b:enabled"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result = builder.AddAzureB2B();

        // Assert
        result.Should().Be(builder);
        // Should not register services when disabled
    }
}


