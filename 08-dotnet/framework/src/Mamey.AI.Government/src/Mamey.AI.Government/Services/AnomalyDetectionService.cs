using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    private readonly PatternAnalyzer _analyzer;
    private readonly ILogger<AnomalyDetectionService> _logger;

    public AnomalyDetectionService(
        PatternAnalyzer analyzer,
        ILogger<AnomalyDetectionService> logger)
    {
        _analyzer = analyzer;
        _logger = logger;
    }

    public async Task<bool> DetectAnomalyAsync(IEnumerable<double> timeSeriesData, CancellationToken cancellationToken = default)
    {
        var score = await _analyzer.AnalyzeAsync(timeSeriesData, cancellationToken);
        return score > 0.8; // Threshold
    }

    public async Task<double> CalculateAnomalyScoreAsync(object dataPoint, CancellationToken cancellationToken = default)
    {
        // For single data point, we'd need a baseline or model context. 
        // Stub implementation.
        await Task.Delay(10, cancellationToken);
        return 0.1; 
    }
    
    public async Task<AnomalyAlert?> CheckForAlertAsync(IEnumerable<double> timeSeriesData, string context, CancellationToken cancellationToken = default)
    {
        var score = await _analyzer.AnalyzeAsync(timeSeriesData, cancellationToken);
        if (score > 0.8)
        {
            return new AnomalyAlert
            {
                AlertType = "PatternAnomaly",
                Score = score,
                Description = $"Anomaly detected in {context} with score {score:F2}",
                Data = new Dictionary<string, object> { { "LatestValue", timeSeriesData.LastOrDefault() } }
            };
        }
        return null;
    }
}
