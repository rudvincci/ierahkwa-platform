using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Biometric template model for external consumption.
/// </summary>
public class BiometricTemplate
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
    /// Template metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public BiometricTemplateMetadata Metadata { get; set; } = new();

    /// <summary>
    /// MinIO object ID for original image
    /// </summary>
    [JsonPropertyName("minio_object_id")]
    public string MinioObjectId { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Biometric template metadata.
/// </summary>
public class BiometricTemplateMetadata
{
    /// <summary>
    /// Quality score of the face
    /// </summary>
    [JsonPropertyName("quality_score")]
    public double QualityScore { get; set; }

    /// <summary>
    /// Image format
    /// </summary>
    [JsonPropertyName("image_format")]
    public string ImageFormat { get; set; } = string.Empty;

    /// <summary>
    /// Image size in bytes
    /// </summary>
    [JsonPropertyName("image_size_bytes")]
    public long ImageSizeBytes { get; set; }

    /// <summary>
    /// Face location in the image
    /// </summary>
    [JsonPropertyName("face_location")]
    public FaceLocation? FaceLocation { get; set; }

    /// <summary>
    /// Additional custom metadata
    /// </summary>
    [JsonPropertyName("custom_data")]
    public Dictionary<string, object> CustomData { get; set; } = new();
}
