using System.ComponentModel.DataAnnotations;

using System;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Request to verify a face against a stored template.
/// </summary>
public class VerificationRequest
{
    /// <summary>
    /// Template ID to verify against
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

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
    /// Verification threshold (0.0 to 1.0). If null, uses default threshold.
    /// </summary>
    [Range(0.0, 1.0)]
    public double? Threshold { get; set; }
}
