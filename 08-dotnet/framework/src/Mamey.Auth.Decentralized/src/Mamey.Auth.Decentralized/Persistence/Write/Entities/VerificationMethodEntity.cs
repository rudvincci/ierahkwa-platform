using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Persistence.Write.Entities;

/// <summary>
/// Entity representing a Verification Method in the write database
/// </summary>
[Table("verification_methods")]
public class VerificationMethodEntity
{
    /// <summary>
    /// The unique identifier
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// The DID Document ID this verification method belongs to
    /// </summary>
    [Required]
    [Column("did_document_id")]
    public Guid DidDocumentId { get; set; }

    /// <summary>
    /// The verification method ID
    /// </summary>
    [Required]
    [Column("verification_method_id")]
    [StringLength(500)]
    public string VerificationMethodId { get; set; } = string.Empty;

    /// <summary>
    /// The verification method type
    /// </summary>
    [Required]
    [Column("type")]
    [StringLength(100)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The controller of the verification method
    /// </summary>
    [Column("controller")]
    [StringLength(500)]
    public string? Controller { get; set; }

    /// <summary>
    /// The public key material as JSON
    /// </summary>
    [Required]
    [Column("public_key_material")]
    public string PublicKeyMaterial { get; set; } = string.Empty;

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
    /// Whether the verification method is active
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
    /// Converts the entity to a Verification Method
    /// </summary>
    /// <returns>The Verification Method</returns>
    public VerificationMethod ToVerificationMethod()
    {
        return new VerificationMethod
        {
            Id = VerificationMethodId,
            Type = Type,
            Controller = Controller,
            PublicKeyJwk = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(PublicKeyMaterial),
            AdditionalProperties = string.IsNullOrEmpty(AdditionalProperties) 
                ? new Dictionary<string, System.Text.Json.JsonElement>() 
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(AdditionalProperties) ?? new Dictionary<string, System.Text.Json.JsonElement>()
        };
    }

    /// <summary>
    /// Creates an entity from a Verification Method
    /// </summary>
    /// <param name="verificationMethod">The Verification Method</param>
    /// <param name="didDocumentId">The DID Document ID</param>
    /// <returns>The entity</returns>
    public static VerificationMethodEntity FromVerificationMethod(VerificationMethod verificationMethod, Guid didDocumentId)
    {
        return new VerificationMethodEntity
        {
            Id = Guid.NewGuid(),
            DidDocumentId = didDocumentId,
            VerificationMethodId = verificationMethod.Id,
            Type = verificationMethod.Type,
            Controller = verificationMethod.Controller,
            PublicKeyMaterial = verificationMethod.PublicKeyJwk != null 
                ? System.Text.Json.JsonSerializer.Serialize(verificationMethod.PublicKeyJwk) 
                : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            AdditionalProperties = verificationMethod.AdditionalProperties.Any() 
                ? System.Text.Json.JsonSerializer.Serialize(verificationMethod.AdditionalProperties) 
                : null
        };
    }
}
