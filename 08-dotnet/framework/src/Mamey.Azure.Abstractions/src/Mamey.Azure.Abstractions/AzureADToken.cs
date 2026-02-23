using System.Text.Json.Serialization;

namespace Mamey.Azure.Abstractions;

public record AzureADToken
{
    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    [JsonPropertyName("ext_expires_in")]
    public int ExtExpiresIn { get; init; }
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

}