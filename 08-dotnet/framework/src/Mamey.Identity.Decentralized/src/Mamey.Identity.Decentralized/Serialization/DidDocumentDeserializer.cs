using System.Text.Json;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Serialization;

/// <summary>
/// Deserializes JSON-LD or JSON DID Documents to strongly typed objects.
/// </summary>
public static class DidDocumentDeserializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Parses a DID Document from a JSON-LD string.
    /// </summary>
    public static DidDocument FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        return JsonSerializer.Deserialize<DidDocument>(json, Options);
    }

    /// <summary>
    /// Parses a DID Document from a UTF-8 JSON-LD byte array.
    /// </summary>
    public static DidDocument FromUtf8Json(byte[] json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        return FromJson(System.Text.Encoding.UTF8.GetString(json));
    }
}