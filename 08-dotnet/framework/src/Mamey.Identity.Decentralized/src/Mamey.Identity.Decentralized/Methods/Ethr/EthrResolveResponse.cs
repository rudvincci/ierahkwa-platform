using System.Text.Json.Serialization;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Methods.Ethr;

/// <summary>
/// DTO for response from a remote Ethr DID registry or resolver service.
/// </summary>
public class EthrResolveResponse
{
    [JsonPropertyName("didDocument")]
    public DidDocument DidDocument { get; set; }
}