namespace Mamey.AI.Identity.ML;

/// <summary>
/// Interface for running inference on ML models.
/// </summary>
public interface IInferenceEngine
{
    /// <summary>
    /// Runs inference using an ML.NET model.
    /// </summary>
    Task<TResult> PredictAsync<TInput, TResult>(
        object model,
        TInput input,
        CancellationToken cancellationToken = default)
        where TInput : class
        where TResult : class, new();

    /// <summary>
    /// Runs inference using an ONNX model.
    /// </summary>
    Task<float[]> PredictOnnxAsync(
        byte[] onnxModel,
        Dictionary<string, float[]> inputs,
        CancellationToken cancellationToken = default);
}
