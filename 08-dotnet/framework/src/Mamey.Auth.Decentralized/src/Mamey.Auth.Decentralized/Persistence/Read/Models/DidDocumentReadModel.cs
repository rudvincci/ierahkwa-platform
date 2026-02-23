using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Persistence.Read.Models;

/// <summary>
/// Read model for DID Documents stored in MongoDB
/// </summary>
public class DidDocumentReadModel
{
    /// <summary>
    /// MongoDB ObjectId
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The DID identifier
    /// </summary>
    [BsonElement("did")]
    [BsonRequired]
    public string Did { get; set; } = string.Empty;

    /// <summary>
    /// The DID method
    /// </summary>
    [BsonElement("method")]
    [BsonRequired]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// The DID Document context
    /// </summary>
    [BsonElement("context")]
    public List<string> Context { get; set; } = new();

    /// <summary>
    /// The DID Document ID
    /// </summary>
    [BsonElement("id")]
    [BsonRequired]
    public string DocumentId { get; set; } = string.Empty;

    /// <summary>
    /// The controller of the DID Document
    /// </summary>
    [BsonElement("controller")]
    public string? Controller { get; set; }

    /// <summary>
    /// The verification methods
    /// </summary>
    [BsonElement("verificationMethod")]
    public List<VerificationMethodReadModel> VerificationMethods { get; set; } = new();

    /// <summary>
    /// The authentication methods
    /// </summary>
    [BsonElement("authentication")]
    public List<string> Authentication { get; set; } = new();

    /// <summary>
    /// The assertion method
    /// </summary>
    [BsonElement("assertionMethod")]
    public List<string> AssertionMethod { get; set; } = new();

    /// <summary>
    /// The key agreement methods
    /// </summary>
    [BsonElement("keyAgreement")]
    public List<string> KeyAgreement { get; set; } = new();

    /// <summary>
    /// The capability invocation methods
    /// </summary>
    [BsonElement("capabilityInvocation")]
    public List<string> CapabilityInvocation { get; set; } = new();

    /// <summary>
    /// The capability delegation methods
    /// </summary>
    [BsonElement("capabilityDelegation")]
    public List<string> CapabilityDelegation { get; set; } = new();

    /// <summary>
    /// The service endpoints
    /// </summary>
    [BsonElement("service")]
    public List<ServiceEndpointReadModel> Service { get; set; } = new();

    /// <summary>
    /// The also known as identifiers
    /// </summary>
    [BsonElement("alsoKnownAs")]
    public List<string> AlsoKnownAs { get; set; } = new();

    /// <summary>
    /// The public key
    /// </summary>
    [BsonElement("publicKey")]
    public List<VerificationMethodReadModel> PublicKey { get; set; } = new();

    /// <summary>
    /// The created timestamp
    /// </summary>
    [BsonElement("created")]
    public DateTime? Created { get; set; }

    /// <summary>
    /// The updated timestamp
    /// </summary>
    [BsonElement("updated")]
    public DateTime? Updated { get; set; }

    /// <summary>
    /// The proof
    /// </summary>
    [BsonElement("proof")]
    public List<ProofReadModel> Proof { get; set; } = new();

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
    /// Whether the document is active
    /// </summary>
    [BsonElement("isActive")]
    [BsonRequired]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The version of the document
    /// </summary>
    [BsonElement("version")]
    [BsonRequired]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Metadata for the document
    /// </summary>
    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}
