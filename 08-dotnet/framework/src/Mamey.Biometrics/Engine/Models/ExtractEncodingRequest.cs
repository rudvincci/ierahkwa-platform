using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Request to extract face encoding from image data.
/// </summary>
public class ExtractEncodingRequest
{
    /// <summary>
    /// Base64 encoded image data
    /// </summary>
    [Required]
    [JsonPropertyName("image_data")]
    public string ImageData { get; set; } = string.Empty;

    /// <summary>
    /// Image format (JPEG, PNG, etc.)
    /// </summary>
    [JsonPropertyName("image_format")]
    public string ImageFormat { get; set; } = "JPEG";
}
