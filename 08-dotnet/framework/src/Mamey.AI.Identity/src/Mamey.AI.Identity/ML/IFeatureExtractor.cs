namespace Mamey.AI.Identity.ML;

/// <summary>
/// Interface for extracting features from raw data for ML models.
/// </summary>
public interface IFeatureExtractor
{
    /// <summary>
    /// Extracts features from identity application data.
    /// </summary>
    Task<float[]> ExtractIdentityFeaturesAsync(
        object applicationData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts features from biometric data.
    /// </summary>
    Task<float[]> ExtractBiometricFeaturesAsync(
        Stream biometricData,
        string biometricType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts features from behavioral patterns.
    /// </summary>
    Task<float[]> ExtractBehavioralFeaturesAsync(
        object behavioralData,
        CancellationToken cancellationToken = default);
}
