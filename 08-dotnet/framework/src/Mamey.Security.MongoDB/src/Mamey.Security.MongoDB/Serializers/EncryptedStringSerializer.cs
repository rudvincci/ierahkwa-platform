using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Linq;

namespace Mamey.Security.MongoDB.Serializers;

/// <summary>
/// MongoDB BSON serializer for automatically encrypting/decrypting string properties marked with [EncryptedAttribute].
/// </summary>
public class EncryptedStringSerializer : SerializerBase<string>
{
    private readonly ISecurityProvider _securityProvider;

    public EncryptedStringSerializer(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.GetCurrentBsonType();
        if (bsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null!;
        }

        var value = context.Reader.ReadString();
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return _securityProvider.Decrypt(value);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }

        if (string.IsNullOrEmpty(value))
        {
            context.Writer.WriteString(string.Empty);
            return;
        }

        var encryptedValue = _securityProvider.Encrypt(value);
        context.Writer.WriteString(encryptedValue);
    }
}

