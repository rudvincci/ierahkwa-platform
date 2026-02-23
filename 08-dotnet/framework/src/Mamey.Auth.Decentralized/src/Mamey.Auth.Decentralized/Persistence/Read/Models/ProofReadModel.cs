using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.Auth.Decentralized.Persistence.Read.Models;

/// <summary>
/// Read model for Proofs stored in MongoDB
/// </summary>
public class ProofReadModel
{
    /// <summary>
    /// MongoDB ObjectId
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The proof type
    /// </summary>
    [BsonElement("type")]
    [BsonRequired]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The proof purpose
    /// </summary>
    [BsonElement("proofPurpose")]
    [BsonRequired]
    public string ProofPurpose { get; set; } = string.Empty;

    /// <summary>
    /// The verification method used for the proof
    /// </summary>
    [BsonElement("verificationMethod")]
    [BsonRequired]
    public string VerificationMethod { get; set; } = string.Empty;

    /// <summary>
    /// The created timestamp
    /// </summary>
    [BsonElement("created")]
    [BsonRequired]
    public DateTime Created { get; set; }

    /// <summary>
    /// The domain of the proof
    /// </summary>
    [BsonElement("domain")]
    [BsonIgnoreIfNull]
    public string? Domain { get; set; }

    /// <summary>
    /// The challenge of the proof
    /// </summary>
    [BsonElement("challenge")]
    [BsonIgnoreIfNull]
    public string? Challenge { get; set; }

    /// <summary>
    /// The nonce of the proof
    /// </summary>
    [BsonElement("nonce")]
    [BsonIgnoreIfNull]
    public string? Nonce { get; set; }

    /// <summary>
    /// The proof value (signature)
    /// </summary>
    [BsonElement("proofValue")]
    [BsonRequired]
    public string ProofValue { get; set; } = string.Empty;

    /// <summary>
    /// The JWS (JSON Web Signature) of the proof
    /// </summary>
    [BsonElement("jws")]
    [BsonIgnoreIfNull]
    public string? Jws { get; set; }

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
    /// Whether the proof is active
    /// </summary>
    [BsonElement("isActive")]
    [BsonRequired]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The algorithm used for the proof
    /// </summary>
    [BsonElement("algorithm")]
    [BsonIgnoreIfNull]
    public string? Algorithm { get; set; }

    /// <summary>
    /// The key size used for the proof
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
    /// Metadata for the proof
    /// </summary>
    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}
