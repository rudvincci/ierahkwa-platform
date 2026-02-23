using System.Text.Json.Serialization;

using System;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Result of biometric template enrollment.
/// </summary>
public class EnrollmentResult
{
    /// <summary>
    /// Whether the enrollment was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Template ID if successful
    /// </summary>
    [JsonPropertyName("template_id")]
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// Subject ID
    /// </summary>
    [JsonPropertyName("subject_id")]
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Quality score of the enrolled face
    /// </summary>
    [JsonPropertyName("quality_score")]
    public double? QualityScore { get; set; }

    /// <summary>
    /// Face location in the image
    /// </summary>
    [JsonPropertyName("face_location")]
    public FaceLocation? FaceLocation { get; set; }

    /// <summary>
    /// Error message if enrollment failed
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Enrollment timestamp
    /// </summary>
    [JsonPropertyName("enrolled_at")]
    public DateTime? EnrolledAt { get; set; }
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
