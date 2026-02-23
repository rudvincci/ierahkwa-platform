using FluentAssertions;
using Mamey.Auth.Azure;
using Xunit;

namespace Mamey.Auth.Azure.Tests.Unit;

public class AzureMultiAuthOptionsTests
{
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new AzureMultiAuthOptions();

        // Assert
        options.EnableAzure.Should().BeFalse();
        options.EnableAzureB2B.Should().BeFalse();
        options.EnableAzureB2C.Should().BeFalse();
        options.Policy.Should().Be(AzureAuthenticationPolicy.EitherOr);
        options.AzureScheme.Should().Be("AzureAD");
        options.AzureB2BScheme.Should().Be("AzureB2B");
        options.AzureB2CScheme.Should().Be("AzureB2C");
        options.AzureSectionName.Should().Be("azure");
        options.AzureB2BSectionName.Should().Be("azure:b2b");
        options.AzureB2CSectionName.Should().Be("azure:b2c");
    }

    [Fact]
    public void EnableAzure_WhenSet_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new AzureMultiAuthOptions { EnableAzure = true };

        // Act & Assert
        options.EnableAzure.Should().BeTrue();
    }

    [Fact]
    public void EnableAzureB2B_WhenSet_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new AzureMultiAuthOptions { EnableAzureB2B = true };

        // Act & Assert
        options.EnableAzureB2B.Should().BeTrue();
    }

    [Fact]
    public void EnableAzureB2C_WhenSet_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new AzureMultiAuthOptions { EnableAzureB2C = true };

        // Act & Assert
        options.EnableAzureB2C.Should().BeTrue();
    }

    [Theory]
    [InlineData(AzureAuthenticationPolicy.AzureOnly)]
    [InlineData(AzureAuthenticationPolicy.B2BOnly)]
    [InlineData(AzureAuthenticationPolicy.B2COnly)]
    [InlineData(AzureAuthenticationPolicy.EitherOr)]
    [InlineData(AzureAuthenticationPolicy.AllRequired)]
    public void Policy_WhenSet_ShouldReturnCorrectValue(AzureAuthenticationPolicy policy)
    {
        // Arrange
        var options = new AzureMultiAuthOptions { Policy = policy };

        // Act & Assert
        options.Policy.Should().Be(policy);
    }

    [Fact]
    public void SchemeNames_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            AzureScheme = "CustomAzure",
            AzureB2BScheme = "CustomB2B",
            AzureB2CScheme = "CustomB2C"
        };

        // Act & Assert
        options.AzureScheme.Should().Be("CustomAzure");
        options.AzureB2BScheme.Should().Be("CustomB2B");
        options.AzureB2CScheme.Should().Be("CustomB2C");
    }

    [Fact]
    public void SectionNames_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            AzureSectionName = "custom:azure",
            AzureB2BSectionName = "custom:b2b",
            AzureB2CSectionName = "custom:b2c"
        };

        // Act & Assert
        options.AzureSectionName.Should().Be("custom:azure");
        options.AzureB2BSectionName.Should().Be("custom:b2b");
        options.AzureB2CSectionName.Should().Be("custom:b2c");
    }
}


