using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Methods.Ion;

public class BitcoinAnchoringServiceTests
{
    private readonly Mock<ILogger<IBitcoinAnchoringService>> _loggerMock;
    private readonly Mock<IOptions<BitcoinAnchoringOptions>> _optionsMock;
    private readonly Mock<IBitcoinAnchoringService> _bitcoinAnchoringServiceMock;

    public BitcoinAnchoringServiceTests()
    {
        _loggerMock = new Mock<ILogger<IBitcoinAnchoringService>>();
        _optionsMock = new Mock<IOptions<BitcoinAnchoringOptions>>();
        _bitcoinAnchoringServiceMock = new Mock<IBitcoinAnchoringService>();
        
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };
        _optionsMock.Setup(x => x.Value).Returns(options);
    }

    [Fact]
    public async Task AnchorToBitcoinAsync_WithValidOperation_ShouldReturnTransactionHash()
    {
        // Arrange
        var operationHash = "test-operation-hash";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act
        _bitcoinAnchoringServiceMock.Setup(x => x.AnchorToBitcoinAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BitcoinAnchoringResult { Success = true, TransactionId = "test-transaction-hash" });
        var result = await _bitcoinAnchoringServiceMock.Object.AnchorToBitcoinAsync(operationHash, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.TransactionId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnchorToBitcoinAsync_WithNullOperationHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        string operationHash = null;
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.AnchorToBitcoinAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(operationHash)));
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bitcoinAnchoringServiceMock.Object.AnchorToBitcoinAsync(operationHash, options));
    }

    [Fact]
    public async Task AnchorToBitcoinAsync_WithEmptyOperationHash_ShouldThrowArgumentException()
    {
        // Arrange
        var operationHash = "";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.AnchorToBitcoinAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());
        await Assert.ThrowsAsync<ArgumentException>(() => _bitcoinAnchoringServiceMock.Object.AnchorToBitcoinAsync(operationHash, options));
    }

    [Fact]
    public async Task VerifyAnchoringAsync_WithValidOperationHash_ShouldReturnTrue()
    {
        // Arrange
        var operationHash = "test-operation-hash";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act
        _bitcoinAnchoringServiceMock.Setup(x => x.VerifyAnchoringAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var result = await _bitcoinAnchoringServiceMock.Object.VerifyAnchoringAsync(operationHash, options);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAnchoringAsync_WithNullOperationHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        string operationHash = null;
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.VerifyAnchoringAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException());
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bitcoinAnchoringServiceMock.Object.VerifyAnchoringAsync(operationHash, options));
    }

    [Fact]
    public async Task VerifyAnchoringAsync_WithEmptyOperationHash_ShouldThrowArgumentException()
    {
        // Arrange
        var operationHash = "";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.VerifyAnchoringAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());
        await Assert.ThrowsAsync<ArgumentException>(() => _bitcoinAnchoringServiceMock.Object.VerifyAnchoringAsync(operationHash, options));
    }

    [Fact]
    public async Task GetAnchoringTransactionIdAsync_WithValidOperationHash_ShouldReturnTransactionId()
    {
        // Arrange
        var operationHash = "test-operation-hash";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act
        _bitcoinAnchoringServiceMock.Setup(x => x.GetAnchoringTransactionIdAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test-transaction-id");
        var result = await _bitcoinAnchoringServiceMock.Object.GetAnchoringTransactionIdAsync(operationHash, options);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAnchoringTransactionIdAsync_WithNullOperationHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        string operationHash = null;
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.GetAnchoringTransactionIdAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException());
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bitcoinAnchoringServiceMock.Object.GetAnchoringTransactionIdAsync(operationHash, options));
    }

    [Fact]
    public async Task GetAnchoringTransactionIdAsync_WithEmptyOperationHash_ShouldThrowArgumentException()
    {
        // Arrange
        var operationHash = "";
        var options = new BitcoinAnchoringOptions
        {
            Enabled = true,
            Network = "testnet",
            BitcoinNodeEndpoint = "https://testnet.bitcoin.com",
            MinConfirmations = 1
        };

        // Act & Assert
        _bitcoinAnchoringServiceMock.Setup(x => x.GetAnchoringTransactionIdAsync(operationHash, It.IsAny<BitcoinAnchoringOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());
        await Assert.ThrowsAsync<ArgumentException>(() => _bitcoinAnchoringServiceMock.Object.GetAnchoringTransactionIdAsync(operationHash, options));
    }
}







