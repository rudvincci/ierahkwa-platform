using System.ComponentModel.DataAnnotations;

namespace Mamey.Biometrics;

/// <summary>
/// Configuration options for the biometrics service.
/// </summary>
public class BiometricsOptions
{
    /// <summary>
    /// Configuration section name for appsettings.json
    /// </summary>
    public const string SectionName = "Biometrics";

    /// <summary>
    /// Python biometric engine base URL
    /// </summary>
    [Required]
    public string BiometricsEngineBaseUrl { get; set; } = "http://localhost:6020";

    /// <summary>
    /// HTTP client timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum image size in bytes
    /// </summary>
    [Range(1024, 104857600)] // 1KB to 100MB
    public long MaxImageSizeBytes { get; set; } = 10485760; // 10MB

    /// <summary>
    /// Default verification threshold (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public double DefaultVerificationThreshold { get; set; } = 0.6;

    /// <summary>
    /// Default identification threshold (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public double DefaultIdentificationThreshold { get; set; } = 0.7;

    /// <summary>
    /// Cache expiration time in minutes
    /// </summary>
    [Range(1, 1440)] // 1 minute to 24 hours
    public int CacheExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Maximum number of templates to return in identification
    /// </summary>
    [Range(1, 1000)]
    public int MaxIdentificationResults { get; set; } = 100;

    /// <summary>
    /// Enable detailed logging
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}
