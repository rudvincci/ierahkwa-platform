using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Response from face encoding comparison.
/// </summary>
public class CompareEncodingsResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Similarity score (0.0 to 1.0, higher is more similar)
    /// </summary>
    [JsonPropertyName("similarity")]
    public double? Similarity { get; set; }

    /// <summary>
    /// Distance between encodings
    /// </summary>
    [JsonPropertyName("distance")]
    public double? Distance { get; set; }

    /// <summary>
    /// Whether the faces match based on threshold
    /// </summary>
    [JsonPropertyName("match")]
    public bool? Match { get; set; }

    /// <summary>
    /// Threshold used for matching
    /// </summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
