using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class PatternAnalyzer
{
    private readonly ILogger<PatternAnalyzer> _logger;

    public PatternAnalyzer(ILogger<PatternAnalyzer> logger)
    {
        _logger = logger;
    }

    public async Task<double> AnalyzeAsync(IEnumerable<double> data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analyzing pattern...");
        await Task.Delay(50, cancellationToken);

        // Mock: Simple Z-Score calculation or similar
        var list = data.ToList();
        if (list.Count < 2) return 0;

        double avg = list.Average();
        double sum = list.Sum(d => Math.Pow(d - avg, 2));
        double stdDev = Math.Sqrt(sum / (list.Count - 1));

        double lastValue = list.Last();
        if (stdDev == 0) return 0;
        
        double zScore = Math.Abs((lastValue - avg) / stdDev);
        
        // Normalize zScore to 0-1 range for anomaly probability (rough approx)
        return Math.Min(1.0, zScore / 3.0); // Z-Score > 3 is typically anomalous
    }
}
