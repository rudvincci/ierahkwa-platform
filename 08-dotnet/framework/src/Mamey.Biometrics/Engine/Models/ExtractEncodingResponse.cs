using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Response from face encoding extraction.
/// </summary>
public class ExtractEncodingResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Face encoding (128-dimensional array)
    /// </summary>
    [JsonPropertyName("encoding")]
    public double[]? Encoding { get; set; }

    /// <summary>
    /// Face location in the image
    /// </summary>
    [JsonPropertyName("face_location")]
    public FaceLocation? FaceLocation { get; set; }

    /// <summary>
    /// Quality score of the detected face
    /// </summary>
    [JsonPropertyName("quality_score")]
    public double? QualityScore { get; set; }

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

/// <summary>
/// Face location coordinates.
/// </summary>
public class FaceLocation
{
    /// <summary>
    /// Top coordinate
    /// </summary>
    [JsonPropertyName("top")]
    public int Top { get; set; }

    /// <summary>
    /// Right coordinate
    /// </summary>
    [JsonPropertyName("right")]
    public int Right { get; set; }

    /// <summary>
    /// Bottom coordinate
    /// </summary>
    [JsonPropertyName("bottom")]
    public int Bottom { get; set; }

    /// <summary>
    /// Left coordinate
    /// </summary>
    [JsonPropertyName("left")]
    public int Left { get; set; }
}
