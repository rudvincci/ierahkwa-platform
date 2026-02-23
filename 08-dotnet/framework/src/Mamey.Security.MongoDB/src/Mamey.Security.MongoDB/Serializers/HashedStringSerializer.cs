using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Linq;

namespace Mamey.Security.MongoDB.Serializers;

/// <summary>
/// MongoDB BSON serializer for automatically hashing string properties marked with [HashedAttribute].
/// Note: Hashing is one-way, so deserialization returns the stored hash value.
/// </summary>
public class HashedStringSerializer : SerializerBase<string>
{
    private readonly ISecurityProvider _securityProvider;

    public HashedStringSerializer(ISecurityProvider securityProvider)
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

        // Hashing is one-way, return stored value as-is
        return context.Reader.ReadString();
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

        // Only hash if the value doesn't look like it's already hashed
        // (SHA-512 produces 128 character hex strings)
        if (value.Length == 128 && value.All(c => char.IsLetterOrDigit(c)))
        {
            // Already hashed, write as-is
            context.Writer.WriteString(value);
        }
        else
        {
            var hashedValue = _securityProvider.Hash(value);
            context.Writer.WriteString(hashedValue);
        }
    }
}

