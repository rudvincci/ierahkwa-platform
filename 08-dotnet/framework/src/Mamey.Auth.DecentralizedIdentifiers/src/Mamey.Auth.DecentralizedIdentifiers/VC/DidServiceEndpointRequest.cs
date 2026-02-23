using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Service endpoint for DID Document.
/// </summary>
public class DidServiceEndpointRequest
{
    [Required] public string Id { get; set; } // e.g., "#svc-1"
    [Required] public string Type { get; set; } // e.g., "VerifiableCredentialService"
    [Required] [Url] public string ServiceEndpoint { get; set; }
    public string Description { get; set; }
}