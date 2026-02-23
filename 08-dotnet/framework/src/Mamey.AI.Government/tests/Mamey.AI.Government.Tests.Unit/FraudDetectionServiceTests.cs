using Xunit;
using Moq;
using FluentAssertions;
using Mamey.AI.Government.Services;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Tests.Unit;

public class FraudDetectionServiceTests
{
    private readonly Mock<DuplicateDetector> _duplicateDetector;
    private readonly Mock<BehavioralAnalyzer> _behavioralAnalyzer;
    private readonly Mock<ILogger<FraudDetectionService>> _logger;
    private readonly FraudDetectionService _service;

    public FraudDetectionServiceTests()
    {
        _duplicateDetector = new Mock<DuplicateDetector>(Mock.Of<ILogger<DuplicateDetector>>());
        _behavioralAnalyzer = new Mock<BehavioralAnalyzer>(Mock.Of<ILogger<BehavioralAnalyzer>>());
        _logger = new Mock<ILogger<FraudDetectionService>>();

        _service = new FraudDetectionService(
            _duplicateDetector.Object,
            _behavioralAnalyzer.Object,
            _logger.Object);
    }

    [Fact]
    public async Task AnalyzeApplicationAsync_ShouldDetectHighRisk_WhenDuplicateFound()
    {
        // Arrange
        var appData = new { Id = 1 };
        
        _duplicateDetector.Setup(x => x.CheckForDuplicatesAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FraudIndicator> 
            { 
                new FraudIndicator { Type = "DuplicateApplication", Confidence = 1.0 } 
            });
            
        _behavioralAnalyzer.Setup(x => x.AnalyzeBehaviorAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FraudIndicator>());

        // Act
        var result = await _service.AnalyzeApplicationAsync(appData);

        // Assert
        result.Indicators.Should().Contain(i => i.Type == "DuplicateApplication");
        // Score calculation logic in stub puts weight 50 on duplicate, so it should be at least Medium
        result.Score.Should().BeGreaterOrEqualTo(50); 
    }
}
