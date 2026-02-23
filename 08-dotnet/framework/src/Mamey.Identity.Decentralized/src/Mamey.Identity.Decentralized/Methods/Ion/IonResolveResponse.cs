using System.Text.Json.Serialization;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Methods.Ion;

/// <summary>
/// POCO representing the resolve response from ION.
/// </summary>
public class IonResolveResponse
{
    [JsonPropertyName("didDocument")]
    public DidDocument DidDocument { get; set; }
}