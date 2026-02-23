namespace Mamey.AI.Government.Interfaces;

public interface IAnomalyDetectionService
{
    Task<bool> DetectAnomalyAsync(IEnumerable<double> timeSeriesData, CancellationToken cancellationToken = default);
    Task<double> CalculateAnomalyScoreAsync(object dataPoint, CancellationToken cancellationToken = default);
}
