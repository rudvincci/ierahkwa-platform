using System.Text.Json.Serialization;

using System;

namespace Mamey.Biometrics.Engine.Models;

/// <summary>
/// Service information response.
/// </summary>
public class ServiceInfoResponse
{
    /// <summary>
    /// Service name
    /// </summary>
    [JsonPropertyName("service")]
    public string Service { get; set; } = string.Empty;

    /// <summary>
    /// Service version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Service description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Available capabilities
    /// </summary>
    [JsonPropertyName("capabilities")]
    public string[] Capabilities { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Supported image formats
    /// </summary>
    [JsonPropertyName("supported_formats")]
    public string[] SupportedFormats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Maximum image size in MB
    /// </summary>
    [JsonPropertyName("max_image_size_mb")]
    public int MaxImageSizeMb { get; set; }

    /// <summary>
    /// Face detection model
    /// </summary>
    [JsonPropertyName("face_detection_model")]
    public string FaceDetectionModel { get; set; } = string.Empty;

    /// <summary>
    /// Encoding model
    /// </summary>
    [JsonPropertyName("encoding_model")]
    public string EncodingModel { get; set; } = string.Empty;

    /// <summary>
    /// Default threshold
    /// </summary>
    [JsonPropertyName("default_threshold")]
    public double DefaultThreshold { get; set; }
}
