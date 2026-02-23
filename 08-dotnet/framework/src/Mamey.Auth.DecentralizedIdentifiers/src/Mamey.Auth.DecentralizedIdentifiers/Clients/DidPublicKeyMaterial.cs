using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.DecentralizedIdentifiers.Clients;

/// <summary>
/// Key material for DID creation.
/// </summary>
public class DidPublicKeyMaterial
{
    [Required] public string Id { get; set; } // e.g., "#key-1"
    [Required] public string Type { get; set; } // e.g., "Ed25519VerificationKey2020"
    [Required] public string PublicKey { get; set; } // JWK, base58, or multibase
    [Required] public string Purpose { get; set; } // authentication, assertionMethod, etc.
    public string Encoding { get; set; } // e.g., "JWK", "Base58", "Multibase"
}