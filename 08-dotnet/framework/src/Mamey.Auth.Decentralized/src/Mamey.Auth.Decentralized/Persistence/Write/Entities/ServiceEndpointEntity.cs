using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Persistence.Write.Entities;

/// <summary>
/// Entity representing a Service Endpoint in the write database
/// </summary>
[Table("service_endpoints")]
public class ServiceEndpointEntity
{
    /// <summary>
    /// The unique identifier
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// The DID Document ID this service endpoint belongs to
    /// </summary>
    [Required]
    [Column("did_document_id")]
    public Guid DidDocumentId { get; set; }

    /// <summary>
    /// The service endpoint ID
    /// </summary>
    [Required]
    [Column("service_endpoint_id")]
    [StringLength(500)]
    public string ServiceEndpointId { get; set; } = string.Empty;

    /// <summary>
    /// The service endpoint type
    /// </summary>
    [Required]
    [Column("type")]
    [StringLength(100)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The service endpoint URI
    /// </summary>
    [Required]
    [Column("service_endpoint")]
    [StringLength(1000)]
    public string ServiceEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// The creation timestamp
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The last update timestamp
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Whether the service endpoint is active
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional properties as JSON
    /// </summary>
    [Column("additional_properties")]
    public string? AdditionalProperties { get; set; }

    /// <summary>
    /// Navigation property to the DID Document
    /// </summary>
    [ForeignKey(nameof(DidDocumentId))]
    public virtual DidDocumentEntity DidDocument { get; set; } = null!;

    /// <summary>
    /// Converts the entity to a Service Endpoint
    /// </summary>
    /// <returns>The Service Endpoint</returns>
    public ServiceEndpoint ToServiceEndpoint()
    {
        return new ServiceEndpoint
        {
            Id = ServiceEndpointId,
            Type = Type,
            ServiceEndpointUrl = ServiceEndpoint,
            Properties = string.IsNullOrEmpty(AdditionalProperties) 
                ? new Dictionary<string, System.Text.Json.JsonElement>() 
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(AdditionalProperties) ?? new Dictionary<string, System.Text.Json.JsonElement>()
        };
    }

    /// <summary>
    /// Creates an entity from a Service Endpoint
    /// </summary>
    /// <param name="serviceEndpoint">The Service Endpoint</param>
    /// <param name="didDocumentId">The DID Document ID</param>
    /// <returns>The entity</returns>
    public static ServiceEndpointEntity FromServiceEndpoint(ServiceEndpoint serviceEndpoint, Guid didDocumentId)
    {
        return new ServiceEndpointEntity
        {
            Id = Guid.NewGuid(),
            DidDocumentId = didDocumentId,
            ServiceEndpointId = serviceEndpoint.Id,
            Type = serviceEndpoint.Type,
            ServiceEndpoint = serviceEndpoint.ServiceEndpointUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            AdditionalProperties = serviceEndpoint.Properties.Any() 
                ? System.Text.Json.JsonSerializer.Serialize(serviceEndpoint.Properties) 
                : null
        };
    }
}
