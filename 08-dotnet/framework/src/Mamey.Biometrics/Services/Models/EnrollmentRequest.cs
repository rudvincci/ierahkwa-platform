using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Request to enroll a new biometric template.
/// </summary>
public class EnrollmentRequest
{
    /// <summary>
    /// Subject ID (user GUID)
    /// </summary>
    [Required]
    public Guid SubjectId { get; set; }

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
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> CustomData { get; set; } = new();

    /// <summary>
    /// Minimum quality score required (0.0 to 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public double? MinQualityScore { get; set; }
}
