using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class FraudDetectionService : IFraudDetectionService
{
    private readonly DuplicateDetector _duplicateDetector;
    private readonly BehavioralAnalyzer _behavioralAnalyzer;
    private readonly ILogger<FraudDetectionService> _logger;

    public FraudDetectionService(
        DuplicateDetector duplicateDetector, 
        BehavioralAnalyzer behavioralAnalyzer,
        ILogger<FraudDetectionService> logger)
    {
        _duplicateDetector = duplicateDetector;
        _behavioralAnalyzer = behavioralAnalyzer;
        _logger = logger;
    }

    public async Task<FraudScore> AnalyzeApplicationAsync(object applicationData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing application for fraud...");
        
        var score = new FraudScore();
        
        // 1. Check for duplicates
        var dupIndicators = await _duplicateDetector.CheckForDuplicatesAsync(applicationData, cancellationToken);
        score.Indicators.AddRange(dupIndicators);

        // 2. Behavioral Analysis
        var behaviorIndicators = await _behavioralAnalyzer.AnalyzeBehaviorAsync(applicationData, cancellationToken);
        score.Indicators.AddRange(behaviorIndicators);

        // 3. Calculate Score
        CalculateScore(score);
        
        _logger.LogInformation("Fraud analysis complete. Score: {Score}, Level: {Level}", score.Score, score.RiskLevel);
        return score;
    }

    public async Task<FraudScore> AnalyzeTransactionAsync(object transactionData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing transaction for fraud...");
        
        var score = new FraudScore();
        
        // Reuse components logic
        var behaviorIndicators = await _behavioralAnalyzer.AnalyzeBehaviorAsync(transactionData, cancellationToken);
        score.Indicators.AddRange(behaviorIndicators);

        CalculateScore(score);
        
        return score;
    }

    private void CalculateScore(FraudScore score)
    {
        // Simple weighted sum for now
        double totalWeight = 0;
        
        foreach (var indicator in score.Indicators)
        {
            // Assuming confidence is 0-1.0
            // We assign arbitrary weights for this stub
            double weight = 10; // Base weight per indicator
            if (indicator.Type.Contains("Duplicate")) weight = 50;
            
            totalWeight += weight * indicator.Confidence;
        }

        // Normalize to 0-100 (clamped)
        score.Score = Math.Min(100, totalWeight);
        
        if (score.Score < 20) score.RiskLevel = "Low";
        else if (score.Score < 50) score.RiskLevel = "Medium";
        else if (score.Score < 80) score.RiskLevel = "High";
        else score.RiskLevel = "Critical";
        
        if (score.RiskLevel == "Low") score.Recommendation = "Approve";
        else if (score.RiskLevel == "Medium") score.Recommendation = "Manual Review";
        else score.Recommendation = "Reject";
    }
}
