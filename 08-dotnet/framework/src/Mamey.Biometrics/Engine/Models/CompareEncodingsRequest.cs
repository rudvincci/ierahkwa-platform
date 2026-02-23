using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using System;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Request to compare two face encodings.
/// </summary>
public class CompareEncodingsRequest
{
    /// <summary>
    /// First face encoding (128 dimensions)
    /// </summary>
    [Required]
    [JsonPropertyName("encoding1")]
    public double[] Encoding1 { get; set; } = Array.Empty<double>();

    /// <summary>
    /// Second face encoding (128 dimensions)
    /// </summary>
    [Required]
    [JsonPropertyName("encoding2")]
    public double[] Encoding2 { get; set; } = Array.Empty<double>();
}
