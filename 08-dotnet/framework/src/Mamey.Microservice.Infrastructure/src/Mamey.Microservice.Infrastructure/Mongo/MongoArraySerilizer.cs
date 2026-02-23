using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mamey.Microservice.Infrastructure.Mongo
{
    public class MongoArraySerilizer<T> : SerializerBase<IEnumerable<T>>, IBsonArraySerializer
        where T : class
    {
        private static readonly IBsonSerializer<T> itemSerializer = BsonSerializer.LookupSerializer<T>();

        public MongoArraySerilizer()
        {
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IEnumerable<T> value)
        {
            context.Writer.WriteStartArray();

            if (value is null)
            {
                context.Writer.WriteEndArray();
                return;
            }

            foreach (T item in value)
            {
                itemSerializer.Serialize(context, item);
            }

            context.Writer.WriteEndArray();
        }

        public override List<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            switch (context.Reader.CurrentBsonType)
            {
                case BsonType.Array:
                    var list = new List<T>();
                    context.Reader.ReadStartArray();
                    while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
                        list.Add(itemSerializer.Deserialize(context));
                    context.Reader.ReadEndArray();
                    return list;

                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;

                default:
                    throw new SerializationException("Cannot deserialize!");
            }
        }

        public bool TryGetItemSerializationInfo(out BsonSerializationInfo serializationInfo)
        {
            serializationInfo = new BsonSerializationInfo(null, itemSerializer, itemSerializer.ValueType);
            return true;
        }
    }
}

