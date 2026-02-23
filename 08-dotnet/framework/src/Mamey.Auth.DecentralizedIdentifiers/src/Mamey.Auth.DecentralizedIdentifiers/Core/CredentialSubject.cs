using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

public class CredentialSubject
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    // Store all arbitrary claims (properties not mapped to a concrete property)
    [JsonExtensionData]
    public Dictionary<string, object> Claims { get; set; } = new();
}