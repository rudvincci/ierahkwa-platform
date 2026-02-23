using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Persistence.Write.Entities;

/// <summary>
/// Entity representing a DID Document in the write database
/// </summary>
[Table("did_documents")]
public class DidDocumentEntity
{
    /// <summary>
    /// The unique identifier
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// The DID identifier
    /// </summary>
    [Required]
    [Column("did")]
    [StringLength(500)]
    public string Did { get; set; } = string.Empty;

    /// <summary>
    /// The DID method
    /// </summary>
    [Required]
    [Column("method")]
    [StringLength(100)]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// The DID Document as JSON
    /// </summary>
    [Required]
    [Column("document")]
    public string Document { get; set; } = string.Empty;

    /// <summary>
    /// The version of the DID Document
    /// </summary>
    [Column("version")]
    public long Version { get; set; }

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
    /// Whether the document is active
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The hash of the document for integrity checking
    /// </summary>
    [Column("document_hash")]
    [StringLength(64)]
    public string? DocumentHash { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    [Column("metadata")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Converts the entity to a DID Document
    /// </summary>
    /// <returns>The DID Document</returns>
    public DidDocument ToDidDocument()
    {
        return System.Text.Json.JsonSerializer.Deserialize<DidDocument>(Document) ?? new DidDocument();
    }

    /// <summary>
    /// Creates an entity from a DID Document
    /// </summary>
    /// <param name="didDocument">The DID Document</param>
    /// <param name="did">The DID identifier</param>
    /// <param name="method">The DID method</param>
    /// <returns>The entity</returns>
    public static DidDocumentEntity FromDidDocument(DidDocument didDocument, string did, string method)
    {
        return new DidDocumentEntity
        {
            Id = Guid.NewGuid(),
            Did = did,
            Method = method,
            Document = System.Text.Json.JsonSerializer.Serialize(didDocument),
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            DocumentHash = ComputeHash(System.Text.Json.JsonSerializer.Serialize(didDocument))
        };
    }

    private static string ComputeHash(string content)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
