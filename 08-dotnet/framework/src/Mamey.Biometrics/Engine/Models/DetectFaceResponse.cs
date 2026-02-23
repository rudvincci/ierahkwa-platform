using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Response from face detection.
/// </summary>
public class DetectFaceResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Number of faces detected
    /// </summary>
    [JsonPropertyName("faces_detected")]
    public int? FacesDetected { get; set; }

    /// <summary>
    /// Locations of detected faces
    /// </summary>
    [JsonPropertyName("locations")]
    public FaceLocation[]? Locations { get; set; }

    /// <summary>
    /// Quality scores for each detected face
    /// </summary>
    [JsonPropertyName("quality_scores")]
    public double[]? QualityScores { get; set; }

    /// <summary>
    /// Average quality score
    /// </summary>
    [JsonPropertyName("average_quality")]
    public double? AverageQuality { get; set; }

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
