using System.ComponentModel.DataAnnotations;
using Mamey.Auth.DecentralizedIdentifiers.Clients;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Request to create a new Decentralized Identifier (DID).
/// </summary>
public class CreateDidRequest
{
    /// <summary>
    /// DID method to use (e.g., "web", "key", "ion", "ethr").
    /// </summary>
    [Required]
    [RegularExpression("^[a-z]+$", ErrorMessage = "Method must be a lowercase string (e.g., 'web', 'ion').")]
    public string Method { get; set; }

    /// <summary>
    /// DID controller(s), usually one or more DIDs. Optional for some methods.
    /// </summary>
    public List<string> Controllers { get; set; } = new();

    /// <summary>
    /// Public key material in JWK, Base58, or Multibase format.
    /// </summary>
    [Required]
    public List<DidPublicKeyMaterial> PublicKeys { get; set; } = new();

    /// <summary>
    /// Initial service endpoints (type, endpoint URI, optional description).
    /// </summary>
    public List<DidServiceEndpointRequest> Services { get; set; } = new();

    /// <summary>
    /// Optional: Additional method-specific options, serialized as a dictionary.
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = new();

    /// <summary>
    /// Metadata for audit/tracking (optional).
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}