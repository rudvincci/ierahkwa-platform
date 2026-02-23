using System;
using Mamey.Types;
using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.MicroMonolith.Infrastructure.Mongo.Documents;

public class PhoneDocument
{
    public PhoneDocument() { }

    public PhoneDocument(Phone phone)
    {
        CountryCode = phone.CountryCode;
        Number = phone.Number;
        Extension = phone.Extension;
        Type = phone.Type.ToString();
    }

    [BsonConstructor]
    public PhoneDocument(string countryCode, string number, string? extension = null)
    {
        CountryCode = countryCode;
        Number = number;
        Extension = extension;
    }

    [BsonConstructor]
    public PhoneDocument(string countryCode, string number, string? extension = null, string type = null)
    {
        CountryCode = countryCode;
        Number = number;
        Extension = extension;
        Type = type ?? Phone.PhoneType.Main.ToString();
    }

    [BsonElement("countryCode"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string CountryCode { get; }
    [BsonElement("number"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Number { get; }
    [BsonElement("extension"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? Extension { get; }
    [BsonElement("phoneType"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Type { get; }

    public Phone AsEntity()
        => new Phone(CountryCode, Number, Extension, Type.ToEnum<Phone.PhoneType>());

    public override string ToString() => String.Concat($"{Type}: ", CountryCode, Number, Extension);
}
