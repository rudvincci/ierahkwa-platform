using System.Text.Json;
using System.Text.Json.Serialization;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Serialization;

/// <summary>
/// Serializes DID Documents to JSON-LD as required by the W3C DID Core spec.
/// </summary>
public static class DidDocumentSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes a DID Document to a JSON-LD string.
    /// </summary>
    public static string ToJson(DidDocument document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        return JsonSerializer.Serialize(document, Options);
    }

    /// <summary>
    /// Serializes a DID Document to a UTF-8 JSON-LD byte array.
    /// </summary>
    public static byte[] ToUtf8Json(DidDocument document)
    {
        return System.Text.Encoding.UTF8.GetBytes(ToJson(document));
    }
}