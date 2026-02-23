using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.Auth.Decentralized.Persistence.Read.Models;

/// <summary>
/// Read model for Verification Methods stored in MongoDB
/// </summary>
public class VerificationMethodReadModel
{
    /// <summary>
    /// MongoDB ObjectId
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The verification method ID
    /// </summary>
    [BsonElement("verificationMethodId")]
    [BsonRequired]
    public string VerificationMethodId { get; set; } = string.Empty;

    /// <summary>
    /// The type of verification method
    /// </summary>
    [BsonElement("type")]
    [BsonRequired]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The controller of the verification method
    /// </summary>
    [BsonElement("controller")]
    public string? Controller { get; set; }

    /// <summary>
    /// The public key material in JWK format
    /// </summary>
    [BsonElement("publicKeyJwk")]
    [BsonIgnoreIfNull]
    public Dictionary<string, object>? PublicKeyJwk { get; set; }

    /// <summary>
    /// The public key material in multibase format
    /// </summary>
    [BsonElement("publicKeyMultibase")]
    [BsonIgnoreIfNull]
    public string? PublicKeyMultibase { get; set; }

    /// <summary>
    /// The public key material in hex format
    /// </summary>
    [BsonElement("publicKeyHex")]
    [BsonIgnoreIfNull]
    public string? PublicKeyHex { get; set; }

    /// <summary>
    /// The public key material in base58 format
    /// </summary>
    [BsonElement("publicKeyBase58")]
    [BsonIgnoreIfNull]
    public string? PublicKeyBase58 { get; set; }

    /// <summary>
    /// The public key material in PEM format
    /// </summary>
    [BsonElement("publicKeyPem")]
    [BsonIgnoreIfNull]
    public string? PublicKeyPem { get; set; }

    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [BsonElement("additionalProperties")]
    [BsonIgnoreIfNull]
    public Dictionary<string, object>? AdditionalProperties { get; set; }

    /// <summary>
    /// The creation timestamp
    /// </summary>
    [BsonElement("createdAt")]
    [BsonRequired]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The last update timestamp
    /// </summary>
    [BsonElement("updatedAt")]
    [BsonRequired]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Whether the verification method is active
    /// </summary>
    [BsonElement("isActive")]
    [BsonRequired]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The algorithm used for this verification method
    /// </summary>
    [BsonElement("algorithm")]
    [BsonIgnoreIfNull]
    public string? Algorithm { get; set; }

    /// <summary>
    /// The curve used for this verification method (for elliptic curve algorithms)
    /// </summary>
    [BsonElement("curve")]
    [BsonIgnoreIfNull]
    public string? Curve { get; set; }

    /// <summary>
    /// The key size in bits
    /// </summary>
    [BsonElement("keySize")]
    [BsonIgnoreIfNull]
    public int? KeySize { get; set; }

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Metadata for the verification method
    /// </summary>
    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}
