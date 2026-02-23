namespace Mamey.AI.Identity.Models;

/// <summary>
/// Represents the result of an anomaly detection operation.
/// </summary>
public class AnomalyResult
{
    /// <summary>
    /// Whether an anomaly was detected.
    /// </summary>
    public bool IsAnomalous { get; set; }

    /// <summary>
    /// Anomaly score (0-1.0, where higher = more anomalous).
    /// </summary>
    public double AnomalyScore { get; set; }

    /// <summary>
    /// Type of anomaly detected.
    /// </summary>
    public string AnomalyType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the detected anomaly.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// List of contributing factors to the anomaly.
    /// </summary>
    public List<string> ContributingFactors { get; set; } = new();

    /// <summary>
    /// Timestamp when the anomaly was detected.
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}
