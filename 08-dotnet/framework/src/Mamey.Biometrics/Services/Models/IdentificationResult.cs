using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Result of biometric identification.
/// </summary>
public class IdentificationResult
{
    /// <summary>
    /// Whether the identification was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// List of matching templates
    /// </summary>
    [JsonPropertyName("matches")]
    public List<BiometricMatch> Matches { get; set; } = new();

    /// <summary>
    /// Number of templates searched
    /// </summary>
    [JsonPropertyName("templates_searched")]
    public int TemplatesSearched { get; set; }

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
    /// Error message if identification failed
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Identification timestamp
    /// </summary>
    [JsonPropertyName("identified_at")]
    public DateTime? IdentifiedAt { get; set; }
}

/// <summary>
/// A biometric match result.
/// </summary>
public class BiometricMatch
{
    /// <summary>
    /// Template ID
    /// </summary>
    [JsonPropertyName("template_id")]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Subject ID
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
    /// Quality score of the template
    /// </summary>
    [JsonPropertyName("template_quality_score")]
    public double TemplateQualityScore { get; set; }

    /// <summary>
    /// Template creation date
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Template tags
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}
