using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Interfaces;

public interface IPredictiveAnalyticsService
{
    Task<ForecastResult> ForecastVolumeAsync(IEnumerable<double> historicalData, int horizon, CancellationToken cancellationToken = default);
    Task<TimeSpan> PredictProcessingTimeAsync(object applicationData, CancellationToken cancellationToken = default);
}
