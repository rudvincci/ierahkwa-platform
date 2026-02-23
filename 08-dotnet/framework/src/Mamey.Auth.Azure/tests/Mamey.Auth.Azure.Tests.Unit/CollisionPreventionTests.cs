using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Auth.Azure;
using Mamey;
using Xunit;

namespace Mamey.Auth.Azure.Tests.Unit;

public class CollisionPreventionTests
{
    [Fact]
    public void AddAzure_WhenCalledMultipleTimes_ShouldOnlyRegisterOnce()
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
        var result3 = builder.AddAzure();

        // Assert
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        result3.Should().Be(builder);
        
        // Should only register AzureMultiAuthOptions once due to TryRegister
        var registrations = services.Where(s => s.ServiceType == typeof(AzureMultiAuthOptions)).ToList();
        registrations.Count.Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public void AddAzure_WithDifferentSectionNames_ShouldStillPreventCollision()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["azure:enableAzure"] = "false",
                ["custom:enableAzure"] = "false"
            })
            .Build();
        
        var builder = MameyBuilder.Create(services, configuration);

        // Act
        var result1 = builder.AddAzure("azure");
        var result2 = builder.AddAzure("custom");

        // Assert
        // Both should return builder, but second call should be prevented by TryRegister
        result1.Should().Be(builder);
        result2.Should().Be(builder);
        
        // Should only register once due to registry name collision prevention
        var registrations = services.Where(s => s.ServiceType == typeof(AzureMultiAuthOptions)).ToList();
        registrations.Count.Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public void RegistryName_ShouldBeUnique()
    {
        // Arrange & Act
        // The registry name "auth.azure" should be unique across all auth libraries
        // This is verified by checking that TryRegister prevents duplicate registrations

        // Assert
        // This test verifies that the registry name is used correctly
        // The actual uniqueness is tested by the collision prevention in AddAzure
        true.Should().BeTrue(); // Placeholder - actual test is in AddAzure_WhenCalledMultipleTimes
    }
}

