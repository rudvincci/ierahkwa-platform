using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Methods.Ethr;

/// <summary>
/// The update options for did:ethr method.
/// </summary>
public class EthrUpdateOptions
{
    [JsonPropertyName("controllerAddress")]
    public string ControllerAddress { get; set; }

    [JsonPropertyName("privateKey")]
    public string PrivateKey { get; set; }

    [JsonPropertyName("patches")]
    public IList<IDictionary<string, object>> Patches { get; set; }
}