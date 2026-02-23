using FluentAssertions;
using Xunit;

namespace Mamey.Blockchain.DID.Tests.Unit;

/// <summary>
/// Unit tests for DIDClientOptions.
/// </summary>
public class DIDClientOptionsTests
{
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new DIDClientOptions();

        // Assert
        options.NodeUrl.Should().Be("http://localhost:50051");
        options.TimeoutSeconds.Should().Be(30);
        options.IncludeMetadataInResolution.Should().BeTrue();
        options.DefaultMethod.Should().Be("futurewampum");
        options.MaxRetryAttempts.Should().Be(3);
        options.RetryDelayMs.Should().Be(1000);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var options = new DIDClientOptions();

        // Act
        options.NodeUrl = "https://node.example.com:50052";
        options.TimeoutSeconds = 60;
        options.IncludeMetadataInResolution = false;
        options.DefaultMethod = "custom";
        options.MaxRetryAttempts = 5;
        options.RetryDelayMs = 2000;

        // Assert
        options.NodeUrl.Should().Be("https://node.example.com:50052");
        options.TimeoutSeconds.Should().Be(60);
        options.IncludeMetadataInResolution.Should().BeFalse();
        options.DefaultMethod.Should().Be("custom");
        options.MaxRetryAttempts.Should().Be(5);
        options.RetryDelayMs.Should().Be(2000);
    }
}
