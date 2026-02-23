using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Identity.Decentralized.Converters;

/// <summary>
/// Custom converter for handling single or array subjects.
/// </summary>
public class CredentialSubjectConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
            return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
        if (reader.TokenType == JsonTokenType.StartArray)
            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(ref reader, options);
        throw new JsonException("CredentialSubject must be object or array.");
    }
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is List<Dictionary<string, object>> arr)
            JsonSerializer.Serialize(writer, arr, options);
        else
            JsonSerializer.Serialize(writer, value, options);
    }
}