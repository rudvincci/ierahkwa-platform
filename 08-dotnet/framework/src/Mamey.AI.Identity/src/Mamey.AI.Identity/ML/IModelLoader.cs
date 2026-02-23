using Microsoft.ML;

namespace Mamey.AI.Identity.ML;

/// <summary>
/// Interface for loading ML models from local storage or cloud.
/// </summary>
public interface IModelLoader
{
    /// <summary>
    /// Loads an ML.NET model from a file path.
    /// </summary>
    Task<ITransformer> LoadMlNetModelAsync(string modelPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads an ONNX model from a file path.
    /// </summary>
    Task<byte[]> LoadOnnxModelAsync(string modelPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a model from cloud storage (Azure ML, S3, etc.).
    /// </summary>
    Task<ITransformer> LoadCloudModelAsync(string modelUri, CancellationToken cancellationToken = default);
}
