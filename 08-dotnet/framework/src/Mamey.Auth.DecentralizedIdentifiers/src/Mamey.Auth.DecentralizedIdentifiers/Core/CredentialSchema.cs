using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// W3C CredentialSchema object for referencing a JSON-LD/JSON schema.
/// </summary>
public class CredentialSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}