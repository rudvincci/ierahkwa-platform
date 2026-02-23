using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Security.JsonConverters;

/// <summary>
/// System.Text.Json converter for automatically hashing properties marked with [HashedAttribute].
/// Note: Hashing is one-way, so deserialization returns the stored hash value.
/// </summary>
public class HashedJsonConverter : JsonConverter<string>
{
    private readonly ISecurityProvider _securityProvider;

    public HashedJsonConverter(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Hashing is one-way, return stored value as-is
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        return reader.GetString() ?? string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var hashedValue = _securityProvider.Hash(value);
        writer.WriteStringValue(hashedValue);
    }
}



