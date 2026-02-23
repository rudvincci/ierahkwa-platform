using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Converters;

/// <summary>
/// Custom converter to handle issuer as either string or object (per spec).
/// </summary>
public class IssuerConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
            return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
        if (reader.TokenType == JsonTokenType.String)
            return reader.GetString();
        throw new JsonException("Issuer field must be string or object.");
    }
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is string s)
            writer.WriteStringValue(s);
        else
            JsonSerializer.Serialize(writer, value, options);
    }
    
}
