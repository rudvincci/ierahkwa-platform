using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Request to detect faces in image data.
/// </summary>
public class DetectFaceRequest
{
    /// <summary>
    /// Base64 encoded image data
    /// </summary>
    [Required]
    [JsonPropertyName("image_data")]
    public string ImageData { get; set; } = string.Empty;
}
