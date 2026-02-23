using Xunit;
using Moq;
using FluentAssertions;
using Mamey.AI.Government.Services;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Tests.Unit;

public class DocumentVerificationServiceTests
{
    private readonly Mock<DocumentClassifier> _classifier;
    private readonly Mock<TamperDetector> _tamperDetector;
    private readonly Mock<OcrService> _ocrService;
    private readonly Mock<ILogger<DocumentVerificationService>> _logger;
    private readonly DocumentVerificationService _service;

    public DocumentVerificationServiceTests()
    {
        _classifier = new Mock<DocumentClassifier>(Mock.Of<ILogger<DocumentClassifier>>());
        _tamperDetector = new Mock<TamperDetector>(Mock.Of<ILogger<TamperDetector>>());
        _ocrService = new Mock<OcrService>(Mock.Of<ILogger<OcrService>>());
        _logger = new Mock<ILogger<DocumentVerificationService>>();

        _service = new DocumentVerificationService(
            _classifier.Object,
            _tamperDetector.Object,
            _ocrService.Object,
            _logger.Object);
    }

    [Fact]
    public async Task VerifyDocumentAsync_ShouldReturnSuccess_WhenDocumentIsValid()
    {
        // Arrange
        var stream = new MemoryStream();
        var expectedType = "Passport";
        
        _classifier.Setup(x => x.ClassifyAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Passport");
        _classifier.Setup(x => x.GetConfidenceAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.99);
            
        _tamperDetector.Setup(x => x.CheckAuthenticityAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 1.0, new List<string>()));
            
        _ocrService.Setup(x => x.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, string> { { "Name", "John" } });

        // Act
        var result = await _service.VerifyDocumentAsync(stream, expectedType);

        // Assert
        result.IsVerified.Should().BeTrue();
        result.DocumentType.Should().Be(expectedType);
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public async Task VerifyDocumentAsync_ShouldReturnFailure_WhenTypeMismatch()
    {
        // Arrange
        var stream = new MemoryStream();
        
        _classifier.Setup(x => x.ClassifyAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("DriversLicense"); // Mismatch

        // Act
        var result = await _service.VerifyDocumentAsync(stream, "Passport");

        // Assert
        result.IsVerified.Should().BeFalse();
        result.ValidationErrors.Should().ContainMatch("*type mismatch*");
    }
}
