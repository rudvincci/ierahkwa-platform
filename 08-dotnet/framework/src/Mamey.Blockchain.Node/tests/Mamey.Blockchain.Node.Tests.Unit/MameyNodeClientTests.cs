using FluentAssertions;
using Mamey.Blockchain.Node;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Blockchain.Node.Tests.Unit;

public class MameyNodeClientTests
{
    private readonly Mock<ILogger<MameyNodeClient>> _loggerMock;
    private readonly MameyNodeClientOptions _options;

    public MameyNodeClientTests()
    {
        _loggerMock = new Mock<ILogger<MameyNodeClient>>();
        _options = new MameyNodeClientOptions
        {
            NodeUrl = "http://localhost:50051"
        };
    }

    [Fact]
    public void Constructor_WithOptions_ShouldInitialize()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);

        // Act
        var client = new MameyNodeClient(optionsWrapper, _loggerMock.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MameyNodeClient(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WithIOptions_ShouldInitialize()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);

        // Act
        var client = new MameyNodeClient(optionsWrapper, _loggerMock.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);
        var client = new MameyNodeClient(optionsWrapper, _loggerMock.Object);

        // Act & Assert
        client.Invoking(c => c.Dispose()).Should().NotThrow();
    }
}


























