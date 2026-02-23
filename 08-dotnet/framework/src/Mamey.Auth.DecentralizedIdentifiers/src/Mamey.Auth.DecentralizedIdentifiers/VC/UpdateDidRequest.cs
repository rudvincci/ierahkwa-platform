using System.ComponentModel.DataAnnotations;
using Mamey.Auth.DecentralizedIdentifiers.Clients;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Request to update an existing DID Document.
/// </summary>
public class UpdateDidRequest
{
    /// <summary>
    /// Target DID to update.
    /// </summary>
    [Required]
    public string Did { get; set; }

    /// <summary>
    /// Update operation type (add, remove, replace, etc.).
    /// </summary>
    [Required]
    [RegularExpression("^(add|remove|replace)$", ErrorMessage = "Operation must be 'add', 'remove', or 'replace'.")]
    public string Operation { get; set; }

    /// <summary>
    /// Items to add/update: controllers, keys, or services.
    /// </summary>
    public List<string> Controllers { get; set; }
    public List<DidPublicKeyMaterial> PublicKeys { get; set; }
    public List<DidServiceEndpointRequest> Services { get; set; }

    /// <summary>
    /// Key or service IDs to remove (if Operation is remove).
    /// </summary>
    public List<string> RemoveKeyIds { get; set; }
    public List<string> RemoveServiceIds { get; set; }

    /// <summary>
    /// Optional: Additional method-specific options, serialized as a dictionary.
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = new();

    /// <summary>
    /// Metadata for audit/tracking (optional).
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}