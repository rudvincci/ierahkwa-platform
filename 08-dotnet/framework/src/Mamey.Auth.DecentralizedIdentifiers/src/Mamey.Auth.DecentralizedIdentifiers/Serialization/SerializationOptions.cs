using System.Text.Json;

namespace Mamey.Auth.DecentralizedIdentifiers.Serialization;

/// <summary>
/// Provides advanced/custom options for JSON serialization of DID Documents.
/// </summary>
public class SerializationOptions
{
    public JsonSerializerOptions JsonOptions { get; set; } = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Whether to enforce strict JSON-LD context validation on serialize/deserialize.
    /// </summary>
    public bool EnforceJsonLdContext { get; set; } = true;
}