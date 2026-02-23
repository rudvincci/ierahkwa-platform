using System.Text.Json.Serialization;

using System;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Result of biometric verification.
/// </summary>
public class VerificationResult
{
    /// <summary>
    /// Whether the verification was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Whether the faces match
    /// </summary>
    [JsonPropertyName("match")]
    public bool Match { get; set; }

    /// <summary>
    /// Template ID that was verified against
    /// </summary>
    [JsonPropertyName("template_id")]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Subject ID of the template
    /// </summary>
    [JsonPropertyName("subject_id")]
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Similarity score (0.0 to 1.0)
    /// </summary>
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }

    /// <summary>
    /// Distance between encodings
    /// </summary>
    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    /// <summary>
    /// Threshold used for matching
    /// </summary>
    [JsonPropertyName("threshold")]
    public double Threshold { get; set; }

    /// <summary>
    /// Quality score of the probe image
    /// </summary>
    [JsonPropertyName("probe_quality_score")]
    public double? ProbeQualityScore { get; set; }

    /// <summary>
    /// Error message if verification failed
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Verification timestamp
    /// </summary>
    [JsonPropertyName("verified_at")]
    public DateTime? VerifiedAt { get; set; }
}
