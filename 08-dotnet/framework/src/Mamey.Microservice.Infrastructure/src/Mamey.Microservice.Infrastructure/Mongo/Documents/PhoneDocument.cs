using MongoDB.Bson.Serialization.Attributes;
using static Mamey.Types.Phone;

namespace Mamey.Microservice.Infrastructure.Mongo.Documents;

internal class PhoneDocument
{
    public PhoneDocument() 
    {
        CountryCode = string.Empty;
        Number = string.Empty;
        Type = string.Empty;
    }

    public PhoneDocument(Phone phone)
    {
        CountryCode = phone.CountryCode;
        Number = phone.Number;
        Extension = phone.Extension;
        Type = phone.Type.ToString();
    }

    [BsonConstructor]
    public PhoneDocument(string countryCode, string number, string type, string? extension = null)
    {
        CountryCode = countryCode;
        Number = number;
        Extension = extension;
        Type = type;
    }

    [BsonElement("countryCode"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string CountryCode { get; set; }
    [BsonElement("number"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Number { get; set; }
    [BsonElement("extension"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? Extension { get; set; }
    [BsonElement("type"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Type { get; set; }

    public Phone AsEntity()
        => new Phone(CountryCode, Number, Extension, Type.ToEnum<PhoneType>());
}
