using System.Diagnostics;
using Mamey.FWID.Identities.Application.AI.Models;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// AI-powered identity verification orchestration service.
/// Coordinates document analysis, biometric matching, and fraud detection.
/// </summary>
public class AIIdentityVerificationService : IAIIdentityVerificationService
{
    private readonly IDocumentAnalysisService _documentService;
    private readonly IBiometricMatchingService _biometricService;
    private readonly IFraudDetectionService _fraudService;
    private readonly IBusPublisher _publisher;
    private readonly ILogger<AIIdentityVerificationService> _logger;
    
    private readonly Dictionary<Guid, VerificationResult> _verificationResults = new();
    private readonly object _lock = new();
    
    public AIIdentityVerificationService(
        IDocumentAnalysisService documentService,
        IBiometricMatchingService biometricService,
        IFraudDetectionService fraudService,
        IBusPublisher publisher,
        ILogger<AIIdentityVerificationService> logger)
    {
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        _biometricService = biometricService ?? throw new ArgumentNullException(nameof(biometricService));
        _fraudService = fraudService ?? throw new ArgumentNullException(nameof(fraudService));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<VerificationResult> VerifyIdentityAsync(
        IdentityVerificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Starting AI identity verification for {IdentityId}", request.IdentityId);
        
        var result = new VerificationResult
        {
            IdentityId = request.IdentityId
        };
        
        try
        {
            // Step 1: Document Analysis
            if (request.DocumentFrontImage != null)
            {
                _logger.LogDebug("Analyzing document for {IdentityId}", request.IdentityId);
                
                result.DocumentResult = await _documentService.AnalyzeDocumentAsync(
                    new DocumentAnalysisRequest
                    {
                        IdentityId = request.IdentityId,
                        DocumentImage = request.DocumentFrontImage,
                        DocumentBackImage = request.DocumentBackImage,
                        ExpectedType = request.ExpectedDocumentType
                    },
                    cancellationToken);
                
                result.DecisionAuditTrail.Add(new AIDecisionAudit
                {
                    Timestamp = DateTime.UtcNow,
                    DecisionType = "DocumentAnalysis",
                    ModelUsed = "DocumentCNN",
                    ModelVersion = "1.0.0",
                    Confidence = result.DocumentResult.AuthenticityScore / 100,
                    Explanation = $"Document classified as {result.DocumentResult.Classification}"
                });
                
                // Add flags for document issues
                if (result.DocumentResult.TamperingDetected)
                {
                    result.Flags.Add(new VerificationFlag
                    {
                        Code = "DOC_TAMPER",
                        Description = "Potential document tampering detected",
                        Severity = FlagSeverity.Critical,
                        Recommendation = "Manual review required for document authenticity"
                    });
                }
                
                // Validate extracted data against expected
                ValidateExtractedData(result, request);
            }
            
            // Step 2: Biometric Matching
            if (request.SelfieImage != null && request.DocumentFrontImage != null)
            {
                _logger.LogDebug("Performing biometric matching for {IdentityId}", request.IdentityId);
                
                result.BiometricResult = await _biometricService.MatchFaceToDocumentAsync(
                    request.SelfieImage,
                    request.DocumentFrontImage,
                    cancellationToken);
                
                result.DecisionAuditTrail.Add(new AIDecisionAudit
                {
                    Timestamp = DateTime.UtcNow,
                    DecisionType = "BiometricMatch",
                    ModelUsed = "FaceNet",
                    ModelVersion = "1.0.0",
                    Confidence = result.BiometricResult.Confidence,
                    FeatureImportance = new Dictionary<string, double>
                    {
                        ["FaceMatch"] = result.BiometricResult.MatchScore,
                        ["Liveness"] = result.BiometricResult.LivenessCheck?.LivenessScore ?? 0,
                        ["AntiSpoof"] = 1 - (result.BiometricResult.SpoofDetection?.SpoofProbability ?? 0)
                    },
                    Explanation = result.BiometricResult.IsMatch 
                        ? "Face match confirmed" 
                        : "Face match failed"
                });
                
                // Add flags for biometric issues
                if (!result.BiometricResult.IsMatch)
                {
                    result.Flags.Add(new VerificationFlag
                    {
                        Code = "BIO_MISMATCH",
                        Description = "Selfie does not match document photo",
                        Severity = FlagSeverity.Critical,
                        Recommendation = "Verify subject identity through alternative means"
                    });
                }
                
                if (result.BiometricResult.SpoofDetection?.SpoofDetected == true)
                {
                    result.Flags.Add(new VerificationFlag
                    {
                        Code = "SPOOF_DETECT",
                        Description = $"Potential {result.BiometricResult.SpoofDetection.DetectedSpoofType} attack detected",
                        Severity = FlagSeverity.Critical,
                        Recommendation = "Reject verification - spoof attempt detected"
                    });
                }
                
                if (result.BiometricResult.LivenessCheck?.IsLive == false)
                {
                    result.Flags.Add(new VerificationFlag
                    {
                        Code = "LIVENESS_FAIL",
                        Description = "Liveness check failed",
                        Severity = FlagSeverity.Critical,
                        Recommendation = "Request new selfie with active liveness challenge"
                    });
                }
            }
            
            // Step 3: Fraud Detection
            _logger.LogDebug("Running fraud detection for {IdentityId}", request.IdentityId);
            
            result.FraudAnalysis = await _fraudService.CalculateFraudScoreAsync(
                new FraudDetectionRequest
                {
                    IdentityId = request.IdentityId,
                    IPAddress = request.IPAddress,
                    DeviceFingerprint = request.DeviceFingerprint,
                    BehaviorData = request.BehaviorData
                },
                cancellationToken);
            
            result.DecisionAuditTrail.Add(new AIDecisionAudit
            {
                Timestamp = DateTime.UtcNow,
                DecisionType = "FraudDetection",
                ModelUsed = "XGBoostFraud",
                ModelVersion = "1.0.0",
                Confidence = 1 - result.FraudAnalysis.OverallScore / 100,
                FeatureImportance = result.FraudAnalysis.Signals.ToDictionary(
                    s => s.SignalType,
                    s => s.Score / 100),
                Explanation = result.FraudAnalysis.Recommendation
            });
            
            // Add flags for fraud issues
            if (result.FraudAnalysis.RiskLevel >= FraudRiskLevel.High)
            {
                result.Flags.Add(new VerificationFlag
                {
                    Code = "FRAUD_HIGH",
                    Description = $"High fraud risk detected: {result.FraudAnalysis.RiskLevel}",
                    Severity = FlagSeverity.Critical,
                    Recommendation = result.FraudAnalysis.Recommendation
                });
            }
            else if (result.FraudAnalysis.RiskLevel == FraudRiskLevel.Medium)
            {
                result.Flags.Add(new VerificationFlag
                {
                    Code = "FRAUD_MEDIUM",
                    Description = "Moderate fraud risk detected",
                    Severity = FlagSeverity.Warning,
                    Recommendation = result.FraudAnalysis.Recommendation
                });
            }
            
            // Step 4: Calculate overall result
            result.OverallConfidence = CalculateOverallConfidence(result);
            result.Status = DetermineVerificationStatus(result);
            
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            
            // Store result
            lock (_lock)
            {
                _verificationResults[result.VerificationId] = result;
            }
            
            // Publish verification event
            await _publisher.PublishAsync(new AIVerificationCompletedEvent
            {
                VerificationId = result.VerificationId,
                IdentityId = request.IdentityId,
                Status = result.Status,
                OverallConfidence = result.OverallConfidence,
                FlagCount = result.Flags.Count,
                CriticalFlagCount = result.Flags.Count(f => f.Severity == FlagSeverity.Critical),
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds,
                CompletedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation(
                "AI verification completed for {IdentityId}. Status: {Status}, Confidence: {Confidence}, Flags: {FlagCount}",
                request.IdentityId, result.Status, result.OverallConfidence, result.Flags.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during AI verification for {IdentityId}", request.IdentityId);
            
            result.Status = VerificationStatus.Failed;
            result.Flags.Add(new VerificationFlag
            {
                Code = "SYSTEM_ERROR",
                Description = "Verification system error occurred",
                Severity = FlagSeverity.Critical,
                Recommendation = "Retry verification or use manual process"
            });
            
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            
            return result;
        }
    }
    
    /// <inheritdoc />
    public Task<VerificationResult?> GetVerificationResultAsync(
        Guid verificationId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _verificationResults.TryGetValue(verificationId, out var result);
            return Task.FromResult(result);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<VerificationResult>> GetVerificationHistoryAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var history = _verificationResults.Values
                .Where(r => r.IdentityId == identityId)
                .OrderByDescending(r => r.VerifiedAt)
                .ToList();
            return Task.FromResult<IReadOnlyList<VerificationResult>>(history);
        }
    }
    
    /// <inheritdoc />
    public async Task<VerificationResult> ReVerifyAsync(
        Guid verificationId,
        CancellationToken cancellationToken = default)
    {
        var original = await GetVerificationResultAsync(verificationId, cancellationToken);
        if (original == null)
            throw new InvalidOperationException($"Verification {verificationId} not found");
        
        // Re-run with same identity but fresh analysis
        return await VerifyIdentityAsync(new IdentityVerificationRequest
        {
            IdentityId = original.IdentityId
        }, cancellationToken);
    }
    
    #region Private Methods
    
    private void ValidateExtractedData(VerificationResult result, IdentityVerificationRequest request)
    {
        var extracted = result.DocumentResult?.ExtractedData;
        if (extracted == null) return;
        
        // Validate name
        if (!string.IsNullOrEmpty(request.ExpectedFirstName) &&
            !string.Equals(extracted.FirstName, request.ExpectedFirstName, StringComparison.OrdinalIgnoreCase))
        {
            result.Flags.Add(new VerificationFlag
            {
                Code = "NAME_MISMATCH",
                Description = "Extracted first name does not match expected",
                Severity = FlagSeverity.Warning,
                Recommendation = "Verify name spelling and document data"
            });
        }
        
        // Validate DOB
        if (request.ExpectedDateOfBirth.HasValue && extracted.DateOfBirth.HasValue &&
            request.ExpectedDateOfBirth.Value.Date != extracted.DateOfBirth.Value.Date)
        {
            result.Flags.Add(new VerificationFlag
            {
                Code = "DOB_MISMATCH",
                Description = "Extracted date of birth does not match expected",
                Severity = FlagSeverity.Critical,
                Recommendation = "Verify date of birth accuracy"
            });
        }
    }
    
    private double CalculateOverallConfidence(VerificationResult result)
    {
        var scores = new List<double>();
        
        if (result.DocumentResult != null)
            scores.Add(result.DocumentResult.AuthenticityScore / 100);
        
        if (result.BiometricResult != null)
            scores.Add(result.BiometricResult.Confidence);
        
        if (result.FraudAnalysis != null)
            scores.Add(1 - result.FraudAnalysis.OverallScore / 100);
        
        if (!scores.Any()) return 0;
        
        // Weighted average with emphasis on critical checks
        return scores.Average();
    }
    
    private VerificationStatus DetermineVerificationStatus(VerificationResult result)
    {
        var criticalFlags = result.Flags.Count(f => f.Severity == FlagSeverity.Critical);
        var warningFlags = result.Flags.Count(f => f.Severity == FlagSeverity.Warning);
        
        if (criticalFlags > 0)
        {
            // Check if it's a definitive rejection
            if (result.Flags.Any(f => f.Code is "SPOOF_DETECT" or "DOC_TAMPER" or "FRAUD_HIGH"))
                return VerificationStatus.Rejected;
            
            return VerificationStatus.PendingReview;
        }
        
        if (warningFlags > 1)
            return VerificationStatus.PendingReview;
        
        if (result.OverallConfidence >= 0.85)
            return VerificationStatus.Verified;
        
        if (result.OverallConfidence >= 0.7)
            return VerificationStatus.PendingReview;
        
        return VerificationStatus.Failed;
    }
    
    #endregion
}

/// <summary>
/// Event published when AI verification completes.
/// </summary>
public record AIVerificationCompletedEvent
{
    public Guid VerificationId { get; init; }
    public Guid IdentityId { get; init; }
    public VerificationStatus Status { get; init; }
    public double OverallConfidence { get; init; }
    public int FlagCount { get; init; }
    public int CriticalFlagCount { get; init; }
    public long ProcessingTimeMs { get; init; }
    public DateTime CompletedAt { get; init; }
}
