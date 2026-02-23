namespace Mamey.Image.BackgroundRemoval;

/// <summary>
/// Configuration options for background removal client.
/// </summary>
public class BackgroundRemovalOptions
{
    /// <summary>
    /// Base URL of the background removal API service.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// API timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Maximum retry attempts for failed requests.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Retry delay in milliseconds.
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Default output format for processed images.
    /// </summary>
    public string DefaultOutputFormat { get; set; } = "PNG";

    /// <summary>
    /// Maximum file size in bytes for single image processing.
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Maximum number of files for batch processing.
    /// </summary>
    public int MaxBatchSize { get; set; } = 10;
}

