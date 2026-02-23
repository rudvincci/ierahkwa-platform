using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Request to identify a face against all stored templates.
/// </summary>
public class IdentificationRequest
{
    /// <summary>
    /// Image data as base64 string
    /// </summary>
    [Required]
    public string ImageData { get; set; } = string.Empty;

    /// <summary>
    /// Image format (JPEG, PNG, etc.)
    /// </summary>
    public string ImageFormat { get; set; } = "JPEG";

    /// <summary>
    /// Identification threshold (0.0 to 1.0). If null, uses default threshold.
    /// </summary>
    [Range(0.0, 1.0)]
    public double? Threshold { get; set; }

    /// <summary>
    /// Maximum number of results to return
    /// </summary>
    [Range(1, 1000)]
    public int MaxResults { get; set; } = 10;

    /// <summary>
    /// Optional subject ID filter (only search templates for this subject)
    /// </summary>
    public Guid? SubjectId { get; set; }

    /// <summary>
    /// Optional tags filter (only search templates with these tags)
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Minimum quality score for templates to consider
    /// </summary>
    [Range(0.0, 1.0)]
    public double? MinQualityScore { get; set; }
}
