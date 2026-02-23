using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Mamey.AI.Identity.ML;

/// <summary>
/// Implementation of IInferenceEngine for running ML model inference.
/// </summary>
public class InferenceEngine : IInferenceEngine
{
    private readonly ILogger<InferenceEngine> _logger;
    private readonly MLContext _mlContext;

    public InferenceEngine(ILogger<InferenceEngine> logger)
    {
        _logger = logger;
        _mlContext = new MLContext();
    }

    public async Task<TResult> PredictAsync<TInput, TResult>(
        object model,
        TInput input,
        CancellationToken cancellationToken = default)
        where TInput : class
        where TResult : class, new()
    {
        try
        {
            if (model is not ITransformer transformer)
            {
                throw new ArgumentException("Model must be an ITransformer", nameof(model));
            }

            _logger.LogDebug("Running ML.NET prediction for type: {Type}", typeof(TInput).Name);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TInput, TResult>(transformer);
            var result = await Task.Run(() => predictionEngine.Predict(input), cancellationToken);

            _logger.LogDebug("ML.NET prediction completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run ML.NET prediction");
            throw;
        }
    }

    public async Task<float[]> PredictOnnxAsync(
        byte[] onnxModel,
        Dictionary<string, float[]> inputs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Running ONNX prediction with {InputCount} inputs", inputs.Count);

            using var session = new InferenceSession(onnxModel);

            // Convert inputs to NamedOnnxValue
            var onnxInputs = inputs.Select(kvp =>
            {
                var tensor = new DenseTensor<float>(kvp.Value, new[] { kvp.Value.Length });
                return NamedOnnxValue.CreateFromTensor(kvp.Key, tensor);
            }).ToList();

            // Run inference
            using var results = await Task.Run(() => session.Run(onnxInputs), cancellationToken);

            // Extract output (assuming single output tensor)
            var output = results.First().Value as Tensor<float>;
            if (output == null)
            {
                throw new InvalidOperationException("ONNX model output is not a float tensor");
            }

            var outputArray = output.ToArray();
            _logger.LogDebug("ONNX prediction completed successfully, output size: {Size}", outputArray.Length);
            return outputArray;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run ONNX prediction");
            throw;
        }
    }
}
