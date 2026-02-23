using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.Auth.Decentralized.Persistence.Read.Models;

/// <summary>
/// Read model for Service Endpoints stored in MongoDB
/// </summary>
public class ServiceEndpointReadModel
{
    /// <summary>
    /// MongoDB ObjectId
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The service endpoint ID
    /// </summary>
    [BsonElement("serviceEndpointId")]
    [BsonRequired]
    public string ServiceEndpointId { get; set; } = string.Empty;

    /// <summary>
    /// The type of service endpoint
    /// </summary>
    [BsonElement("type")]
    [BsonRequired]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The service endpoint URL or identifier
    /// </summary>
    [BsonElement("serviceEndpoint")]
    [BsonRequired]
    public string ServiceEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Additional properties specific to the service type
    /// </summary>
    [BsonElement("properties")]
    [BsonIgnoreIfNull]
    public Dictionary<string, object>? Properties { get; set; }

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
    /// Whether the service endpoint is active
    /// </summary>
    [BsonElement("isActive")]
    [BsonRequired]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The priority of the service endpoint
    /// </summary>
    [BsonElement("priority")]
    [BsonIgnoreIfNull]
    public int? Priority { get; set; }

    /// <summary>
    /// The routing key for the service endpoint
    /// </summary>
    [BsonElement("routingKey")]
    [BsonIgnoreIfNull]
    public string? RoutingKey { get; set; }

    /// <summary>
    /// The service endpoint description
    /// </summary>
    [BsonElement("description")]
    [BsonIgnoreIfNull]
    public string? Description { get; set; }

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Metadata for the service endpoint
    /// </summary>
    [BsonElement("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}
