using FluentAssertions;
using Mamey.Blockchain.Banking;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Blockchain.Banking.Tests.Unit;

public class MameyBankingClientTests
{
    private readonly Mock<ILogger<MameyBankingClient>> _loggerMock;
    private readonly MameyBankingClientOptions _options;

    public MameyBankingClientTests()
    {
        _loggerMock = new Mock<ILogger<MameyBankingClient>>();
        _options = new MameyBankingClientOptions
        {
            NodeUrl = "http://localhost:50052"
        };
    }

    [Fact]
    public void Constructor_WithOptions_ShouldInitialize()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);

        // Act
        var client = new MameyBankingClient(optionsWrapper, _loggerMock.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MameyBankingClient(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WithIOptions_ShouldInitialize()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);

        // Act
        var client = new MameyBankingClient(optionsWrapper, _loggerMock.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(_options);
        var client = new MameyBankingClient(optionsWrapper, _loggerMock.Object);

        // Act & Assert
        client.Invoking(c => c.Dispose()).Should().NotThrow();
    }
}


























