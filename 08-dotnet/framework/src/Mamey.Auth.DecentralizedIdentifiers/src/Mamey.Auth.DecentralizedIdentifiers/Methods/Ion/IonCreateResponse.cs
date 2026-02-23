using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

/// <summary>
/// POCO representing the create response from ION node.
/// </summary>
public class IonCreateResponse
{
    [JsonPropertyName("did")]
    public string Did { get; set; }
    [JsonPropertyName("operation")]
    public object Operation { get; set; }
}