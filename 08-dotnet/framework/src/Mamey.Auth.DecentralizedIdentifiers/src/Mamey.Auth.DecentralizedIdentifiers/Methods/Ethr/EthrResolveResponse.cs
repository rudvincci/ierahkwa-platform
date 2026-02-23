using System.Text.Json.Serialization;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ethr;

/// <summary>
/// DTO for response from a remote Ethr DID registry or resolver service.
/// </summary>
public class EthrResolveResponse
{
    [JsonPropertyName("didDocument")]
    public DidDocument DidDocument { get; set; }
}