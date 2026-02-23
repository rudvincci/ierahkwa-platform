using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class PredictiveAnalyticsService : IPredictiveAnalyticsService
{
    private readonly ForecastingEngine _engine;
    private readonly ILogger<PredictiveAnalyticsService> _logger;

    public PredictiveAnalyticsService(
        ForecastingEngine engine,
        ILogger<PredictiveAnalyticsService> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    public async Task<ForecastResult> ForecastVolumeAsync(IEnumerable<double> historicalData, int horizon, CancellationToken cancellationToken = default)
    {
        return await _engine.ForecastAsync(historicalData, horizon, cancellationToken);
    }

    public async Task<TimeSpan> PredictProcessingTimeAsync(object applicationData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Predicting processing time...");
        await Task.Delay(20, cancellationToken);

        // Mock: Based on complexity
        return TimeSpan.FromDays(3);
    }
}
