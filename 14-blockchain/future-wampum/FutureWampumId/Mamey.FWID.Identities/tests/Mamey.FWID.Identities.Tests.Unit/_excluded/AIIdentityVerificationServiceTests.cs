using FluentAssertions;
using Mamey.FWID.Identities.Application.AI.Models;
using Mamey.FWID.Identities.Application.AI.Services;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.AI.Services;

public class AIIdentityVerificationServiceTests
{
    private readonly IDocumentAnalysisService _documentService;
    private readonly IBiometricMatchingService _biometricService;
    private readonly IFraudDetectionService _fraudService;
    private readonly IBusPublisher _publisher;
    private readonly ILogger<AIIdentityVerificationService> _logger;
    private readonly AIIdentityVerificationService _sut;
    
    public AIIdentityVerificationServiceTests()
    {
        _documentService = Substitute.For<IDocumentAnalysisService>();
        _biometricService = Substitute.For<IBiometricMatchingService>();
        _fraudService = Substitute.For<IFraudDetectionService>();
        _publisher = Substitute.For<IBusPublisher>();
        _logger = Substitute.For<ILogger<AIIdentityVerificationService>>();
        
        _sut = new AIIdentityVerificationService(
            _documentService,
            _biometricService,
            _fraudService,
            _publisher,
            _logger);
    }
    
    [Fact]
    public async Task VerifyIdentityAsync_Should_ReturnVerified_When_AllChecksPass()
    {
        // Arrange
        var request = new IdentityVerificationRequest
        {
            IdentityId = Guid.NewGuid(),
            DocumentFrontImage = new byte[] { 1, 2, 3 },
            SelfieImage = new byte[] { 4, 5, 6 }
        };
        
        _documentService.AnalyzeDocumentAsync(Arg.Any<DocumentAnalysisRequest>(), Arg.Any<CancellationToken>())
            .Returns(new DocumentAnalysisResult
            {
                AuthenticityScore = 95,
                Classification = DocumentClassification.Passport,
                TamperingDetected = false
            });
        
        _biometricService.MatchFaceToDocumentAsync(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new BiometricMatchResult
            {
                IsMatch = true,
                MatchScore = 0.95,
                Confidence = 0.92,
                LivenessCheck = new LivenessResult { IsLive = true, LivenessScore = 0.95 },
                SpoofDetection = new SpoofDetectionResult { SpoofDetected = false, SpoofProbability = 0.05 }
            });
        
        _fraudService.CalculateFraudScoreAsync(Arg.Any<FraudDetectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FraudScore
            {
                OverallScore = 15,
                RiskLevel = FraudRiskLevel.VeryLow
            });
        
        // Act
        var result = await _sut.VerifyIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VerificationStatus.Verified);
        result.Flags.Should().BeEmpty();
        result.OverallConfidence.Should().BeGreaterThan(0.8);
    }
    
    [Fact]
    public async Task VerifyIdentityAsync_Should_ReturnPendingReview_When_DocumentTamperingDetected()
    {
        // Arrange
        var request = new IdentityVerificationRequest
        {
            IdentityId = Guid.NewGuid(),
            DocumentFrontImage = new byte[] { 1, 2, 3 },
            SelfieImage = new byte[] { 4, 5, 6 }
        };
        
        _documentService.AnalyzeDocumentAsync(Arg.Any<DocumentAnalysisRequest>(), Arg.Any<CancellationToken>())
            .Returns(new DocumentAnalysisResult
            {
                AuthenticityScore = 45,
                Classification = DocumentClassification.Passport,
                TamperingDetected = true,
                TamperingIndicators = new List<TamperingIndicator>
                {
                    new() { Type = "PhotoManipulation", Confidence = 0.9 }
                }
            });
        
        _biometricService.MatchFaceToDocumentAsync(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new BiometricMatchResult
            {
                IsMatch = true,
                MatchScore = 0.90,
                Confidence = 0.88,
                LivenessCheck = new LivenessResult { IsLive = true, LivenessScore = 0.92 },
                SpoofDetection = new SpoofDetectionResult { SpoofDetected = false }
            });
        
        _fraudService.CalculateFraudScoreAsync(Arg.Any<FraudDetectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FraudScore { OverallScore = 20, RiskLevel = FraudRiskLevel.Low });
        
        // Act
        var result = await _sut.VerifyIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VerificationStatus.PendingReview);
        result.Flags.Should().Contain(f => f.Code == "DOC_TAMPER");
    }
    
    [Fact]
    public async Task VerifyIdentityAsync_Should_ReturnRejected_When_SpoofDetected()
    {
        // Arrange
        var request = new IdentityVerificationRequest
        {
            IdentityId = Guid.NewGuid(),
            DocumentFrontImage = new byte[] { 1, 2, 3 },
            SelfieImage = new byte[] { 4, 5, 6 }
        };
        
        _documentService.AnalyzeDocumentAsync(Arg.Any<DocumentAnalysisRequest>(), Arg.Any<CancellationToken>())
            .Returns(new DocumentAnalysisResult
            {
                AuthenticityScore = 90,
                Classification = DocumentClassification.Passport,
                TamperingDetected = false
            });
        
        _biometricService.MatchFaceToDocumentAsync(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new BiometricMatchResult
            {
                IsMatch = true,
                MatchScore = 0.88,
                Confidence = 0.50,
                LivenessCheck = new LivenessResult { IsLive = false, LivenessScore = 0.3 },
                SpoofDetection = new SpoofDetectionResult 
                { 
                    SpoofDetected = true, 
                    SpoofProbability = 0.85,
                    DetectedSpoofType = SpoofType.Deepfake
                }
            });
        
        _fraudService.CalculateFraudScoreAsync(Arg.Any<FraudDetectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FraudScore { OverallScore = 30, RiskLevel = FraudRiskLevel.Medium });
        
        // Act
        var result = await _sut.VerifyIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VerificationStatus.Rejected);
        result.Flags.Should().Contain(f => f.Code == "SPOOF_DETECT");
    }
    
    [Fact]
    public async Task VerifyIdentityAsync_Should_PublishEvent_When_VerificationCompletes()
    {
        // Arrange
        var request = new IdentityVerificationRequest
        {
            IdentityId = Guid.NewGuid()
        };
        
        _fraudService.CalculateFraudScoreAsync(Arg.Any<FraudDetectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FraudScore { OverallScore = 10, RiskLevel = FraudRiskLevel.VeryLow });
        
        // Act
        await _sut.VerifyIdentityAsync(request);
        
        // Assert
        await _publisher.Received(1).PublishAsync(Arg.Any<AIVerificationCompletedEvent>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task VerifyIdentityAsync_Should_RecordAuditTrail_ForAllDecisions()
    {
        // Arrange
        var request = new IdentityVerificationRequest
        {
            IdentityId = Guid.NewGuid(),
            DocumentFrontImage = new byte[] { 1, 2, 3 },
            SelfieImage = new byte[] { 4, 5, 6 }
        };
        
        _documentService.AnalyzeDocumentAsync(Arg.Any<DocumentAnalysisRequest>(), Arg.Any<CancellationToken>())
            .Returns(new DocumentAnalysisResult { AuthenticityScore = 90 });
        
        _biometricService.MatchFaceToDocumentAsync(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new BiometricMatchResult 
            { 
                IsMatch = true, 
                Confidence = 0.9,
                LivenessCheck = new LivenessResult { IsLive = true },
                SpoofDetection = new SpoofDetectionResult { SpoofDetected = false }
            });
        
        _fraudService.CalculateFraudScoreAsync(Arg.Any<FraudDetectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(new FraudScore 
            { 
                OverallScore = 15,
                Signals = new List<FraudSignal>
                {
                    new() { SignalType = "Velocity", Score = 10 }
                }
            });
        
        // Act
        var result = await _sut.VerifyIdentityAsync(request);
        
        // Assert
        result.DecisionAuditTrail.Should().HaveCountGreaterOrEqualTo(3);
        result.DecisionAuditTrail.Should().Contain(a => a.DecisionType == "DocumentAnalysis");
        result.DecisionAuditTrail.Should().Contain(a => a.DecisionType == "BiometricMatch");
        result.DecisionAuditTrail.Should().Contain(a => a.DecisionType == "FraudDetection");
    }
}
