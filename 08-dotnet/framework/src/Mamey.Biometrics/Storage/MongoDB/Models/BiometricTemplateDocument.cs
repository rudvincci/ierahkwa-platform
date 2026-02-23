using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Mamey.Biometrics.Storage.MongoDB.Models;

/// <summary>
/// MongoDB document for biometric template storage.
/// </summary>
public class BiometricTemplateDocument
{
    /// <summary>
    /// MongoDB ObjectId
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Template ID (GUID)
    /// </summary>
    [BsonElement("template_id")]
    [JsonPropertyName("template_id")]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Subject ID (user GUID)
    /// </summary>
    [BsonElement("subject_id")]
    [JsonPropertyName("subject_id")]
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Face encoding (128-dimensional array)
    /// </summary>
    [BsonElement("encoding")]
    [JsonPropertyName("encoding")]
    public double[] Encoding { get; set; } = Array.Empty<double>();

    /// <summary>
    /// Template metadata
    /// </summary>
    [BsonElement("metadata")]
    [JsonPropertyName("metadata")]
    public BiometricTemplateMetadata Metadata { get; set; } = new();

    /// <summary>
    /// MinIO object ID for original image
    /// </summary>
    [BsonElement("minio_object_id")]
    [JsonPropertyName("minio_object_id")]
    public string MinioObjectId { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [BsonElement("created_at")]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    [BsonElement("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    [BsonElement("tags")]
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Metadata for biometric template.
/// </summary>
public class BiometricTemplateMetadata
{
    /// <summary>
    /// Quality score of the face
    /// </summary>
    [BsonElement("quality_score")]
    [JsonPropertyName("quality_score")]
    public double QualityScore { get; set; }

    /// <summary>
    /// Image format
    /// </summary>
    [BsonElement("image_format")]
    [JsonPropertyName("image_format")]
    public string ImageFormat { get; set; } = string.Empty;

    /// <summary>
    /// Image size in bytes
    /// </summary>
    [BsonElement("image_size_bytes")]
    [JsonPropertyName("image_size_bytes")]
    public long ImageSizeBytes { get; set; }

    /// <summary>
    /// Face location in the image
    /// </summary>
    [BsonElement("face_location")]
    [JsonPropertyName("face_location")]
    public FaceLocation? FaceLocation { get; set; }

    /// <summary>
    /// Additional custom metadata
    /// </summary>
    [BsonElement("custom_data")]
    [JsonPropertyName("custom_data")]
    public Dictionary<string, object> CustomData { get; set; } = new();
}

/// <summary>
/// Face location coordinates.
/// </summary>
public class FaceLocation
{
    /// <summary>
    /// Top coordinate
    /// </summary>
    [BsonElement("top")]
    [JsonPropertyName("top")]
    public int Top { get; set; }

    /// <summary>
    /// Right coordinate
    /// </summary>
    [BsonElement("right")]
    [JsonPropertyName("right")]
    public int Right { get; set; }

    /// <summary>
    /// Bottom coordinate
    /// </summary>
    [BsonElement("bottom")]
    [JsonPropertyName("bottom")]
    public int Bottom { get; set; }

    /// <summary>
    /// Left coordinate
    /// </summary>
    [BsonElement("left")]
    [JsonPropertyName("left")]
    public int Left { get; set; }
}
