namespace Mamey.AI.Identity.Configuration;

/// <summary>
/// Configuration options for Mamey.AI.Identity services.
/// </summary>
public class AiIdentityOptions
{
    /// <summary>
    /// Section name in appsettings.json.
    /// </summary>
    public const string SectionName = "aiIdentity";

    /// <summary>
    /// Whether AI services are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Path to local ML models directory.
    /// </summary>
    public string LocalModelsPath { get; set; } = "wwwroot/ml-models";

    /// <summary>
    /// Base URI for cloud model server (Azure ML, etc.).
    /// </summary>
    public string? CloudModelServerUri { get; set; }

    /// <summary>
    /// API key for cloud model server.
    /// </summary>
    public string? CloudModelApiKey { get; set; }

    /// <summary>
    /// Fraud detection configuration.
    /// </summary>
    public FraudDetectionConfig FraudDetection { get; set; } = new();

    /// <summary>
    /// Biometric matching configuration.
    /// </summary>
    public BiometricMatchingConfig BiometricMatching { get; set; } = new();

    /// <summary>
    /// Anomaly detection configuration.
    /// </summary>
    public AnomalyDetectionConfig AnomalyDetection { get; set; } = new();
}

/// <summary>
/// Configuration for fraud detection service.
/// </summary>
public class FraudDetectionConfig
{
    /// <summary>
    /// Threshold for fraud score (0-100).
    /// </summary>
    public double FraudThreshold { get; set; } = 50.0;

    /// <summary>
    /// Path to fraud detection model.
    /// </summary>
    public string? ModelPath { get; set; }
}

/// <summary>
/// Configuration for biometric matching service.
/// </summary>
public class BiometricMatchingConfig
{
    /// <summary>
    /// Face matching threshold (0-1.0).
    /// </summary>
    public double FaceMatchThreshold { get; set; } = 0.85;

    /// <summary>
    /// Fingerprint matching threshold (0-1.0).
    /// </summary>
    public double FingerprintMatchThreshold { get; set; } = 0.90;

    /// <summary>
    /// Voice matching threshold (0-1.0).
    /// </summary>
    public double VoiceMatchThreshold { get; set; } = 0.80;

    /// <summary>
    /// Path to face recognition model.
    /// </summary>
    public string? FaceModelPath { get; set; }

    /// <summary>
    /// Path to fingerprint matching model.
    /// </summary>
    public string? FingerprintModelPath { get; set; }

    /// <summary>
    /// Path to voice recognition model.
    /// </summary>
    public string? VoiceModelPath { get; set; }
}

/// <summary>
/// Configuration for anomaly detection service.
/// </summary>
public class AnomalyDetectionConfig
{
    /// <summary>
    /// Anomaly detection threshold (0-1.0).
    /// </summary>
    public double AnomalyThreshold { get; set; } = 0.7;

    /// <summary>
    /// Path to anomaly detection model.
    /// </summary>
    public string? ModelPath { get; set; }
}
