using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Monitoring;

public class AiMetricsService
{
    private readonly ILogger<AiMetricsService> _logger;

    public AiMetricsService(ILogger<AiMetricsService> logger)
    {
        _logger = logger;
    }

    public void RecordInferenceLatency(string modelName, double latencyMs)
    {
        _logger.LogInformation("Metric: ai_inference_latency_ms, Model={Model}, Value={Value}", modelName, latencyMs);
    }

    public void RecordInferenceCount(string modelName, string resultType)
    {
        _logger.LogInformation("Metric: ai_inference_count, Model={Model}, Type={Type}", modelName, resultType);
    }
    
    public void RecordError(string modelName, string errorType)
    {
        _logger.LogInformation("Metric: ai_inference_errors, Model={Model}, Error={Error}", modelName, errorType);
    }
}
