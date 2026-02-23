namespace Mamey.Image.BackgroundRemoval.Models;

/// <summary>
/// Response model for health check.
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Service status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Service name.
    /// </summary>
    public string Service { get; set; } = string.Empty;

    /// <summary>
    /// Service version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Timestamp of the health check.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

