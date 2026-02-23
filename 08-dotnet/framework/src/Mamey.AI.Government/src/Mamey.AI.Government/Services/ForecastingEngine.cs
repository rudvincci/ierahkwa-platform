using Microsoft.Extensions.Logging;
using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Services;

public class ForecastingEngine
{
    private readonly ILogger<ForecastingEngine> _logger;

    public ForecastingEngine(ILogger<ForecastingEngine> logger)
    {
        _logger = logger;
    }

    public async Task<ForecastResult> ForecastAsync(IEnumerable<double> history, int horizon, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating forecast for horizon {Horizon}...", horizon);
        await Task.Delay(50, cancellationToken);

        // Mock: Simple linear projection or moving average
        var list = history.ToList();
        var result = new ForecastResult { ModelUsed = "SimpleMovingAverage" };
        
        if (list.Count == 0) return result;

        double last = list.Last();
        double avg = list.Average();
        
        // Project forward
        for (int i = 0; i < horizon; i++)
        {
            result.ForecastValues.Add(avg + (i * 0.1)); // Slight mock growth
            result.Dates.Add(DateTime.UtcNow.AddDays(i + 1));
        }

        result.ConfidenceIntervalUpper = avg * 1.2;
        result.ConfidenceIntervalLower = avg * 0.8;

        return result;
    }
}
