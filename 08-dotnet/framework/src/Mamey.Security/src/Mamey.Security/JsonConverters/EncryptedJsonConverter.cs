using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mamey.Security.JsonConverters;

/// <summary>
/// System.Text.Json converter for automatically encrypting/decrypting properties marked with [EncryptedAttribute].
/// </summary>
public class EncryptedJsonConverter : JsonConverter<string>
{
    private readonly ISecurityProvider _securityProvider;

    public EncryptedJsonConverter(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null!;

        var encryptedValue = reader.GetString();
        if (string.IsNullOrEmpty(encryptedValue))
            return string.Empty;

        return _securityProvider.Decrypt(encryptedValue);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var encryptedValue = _securityProvider.Encrypt(value);
        writer.WriteStringValue(encryptedValue);
    }
}



