using System.Text.Json.Serialization;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

/// <summary>
/// POCO representing the resolve response from ION.
/// </summary>
public class IonResolveResponse
{
    [JsonPropertyName("didDocument")]
    public DidDocument DidDocument { get; set; }
}