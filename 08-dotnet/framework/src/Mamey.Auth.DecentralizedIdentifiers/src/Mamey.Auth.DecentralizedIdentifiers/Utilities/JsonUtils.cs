using System.Text.Json;

namespace Mamey.Auth.DecentralizedIdentifiers.Utilities;

/// <summary>
/// Utility for safe JSON (de)serialization and minification.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Minifies a JSON string.
    /// </summary>
    public static string Minify(string json)
    {
        var doc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = false });
    }

    /// <summary>
    /// Pretty-prints a JSON string.
    /// </summary>
    public static string Prettify(string json)
    {
        var doc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Recursively removes any "proof" property from a JsonElement.
    /// Returns a JsonElement representing the input with all proof fields stripped.
    /// </summary>
    /// <param name="element">The JsonElement to process.</param>
    /// <returns>A new JsonElement with all proof properties removed.</returns>
    public static JsonElement RemoveProofProperty(JsonElement element)
    {
        using var doc = RemoveProofPropertyInternal(element);
        return doc.RootElement.Clone();
    }

    private static JsonDocument RemoveProofPropertyInternal(JsonElement element)
    {
        using var ms = new System.IO.MemoryStream();
        using (var writer = new Utf8JsonWriter(ms))
        {
            WriteWithoutProof(element, writer);
        }

        ms.Position = 0;
        return JsonDocument.Parse(ms);
    }

    private static void WriteWithoutProof(JsonElement element, Utf8JsonWriter writer)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    if (property.NameEquals("proof"))
                        continue;
                    writer.WritePropertyName(property.Name);
                    WriteWithoutProof(property.Value, writer);
                }

                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteWithoutProof(item, writer);
                }

                writer.WriteEndArray();
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }
}