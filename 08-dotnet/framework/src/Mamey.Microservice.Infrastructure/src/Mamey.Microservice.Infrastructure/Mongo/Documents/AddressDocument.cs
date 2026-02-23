using MongoDB.Bson.Serialization.Attributes;
using static Mamey.Types.Address;

namespace Mamey.Microservice.Infrastructure.Mongo.Documents;

internal class AddressDocument
{
    public AddressDocument() 
    {
        Line = string.Empty;
        Line2 = string.Empty;
        Line3 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Zip5 = string.Empty;
        Country = string.Empty;
        FirmName = string.Empty;
        Urbanization = string.Empty;
        Type = string.Empty;
    }

    public AddressDocument(Address address)
    {
        if(address is null)
        {
            throw new ArgumentNullException(nameof(address));
        }
        Line = address.Line ?? string.Empty;
        Line2 = address.Line2 ?? string.Empty;
        Line3 = address.Line3 ?? string.Empty;
        Urbanization = address.Urbanization ?? string.Empty;
        City = address.City ?? string.Empty;
        State = address.State ?? string.Empty;
        Zip5 = address.Zip5 ?? string.Empty;
        Zip4 = address.Zip4;
        PostalCode = address.PostalCode;
        Country = address.Country ?? string.Empty;
        Province = address.Province;
        FirmName = address.FirmName ?? string.Empty;
        IsDefault = address.IsDefault;
        Type = address.Type.ToString();
    }

    [BsonElement("line"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Line { get; set; }
    [BsonElement("line2"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Line2 { get; set; }
    [BsonElement("line3"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Line3 { get; set; }
    [BsonElement("city"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string City { get; set; }
    [BsonElement("state"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string State { get; set; }
    [BsonElement("zip5"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Zip5 { get; set; }
    [BsonElement("zip4"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? Zip4 { get; set; }
    [BsonElement("postalCode"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? PostalCode { get; set; }
    [BsonElement("country"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Country { get; set; }
    [BsonElement("province"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? Province { get; set; }
    [BsonElement("firmName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string FirmName { get; set; }
    [BsonElement("urbanization"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Urbanization { get; set; }
    [BsonElement("isDefault"), BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public bool IsDefault { get; set; }
    public string Type { get; set; }

    public Address AsEntity()
        => new Address(FirmName, Line, Line2, Line3, Urbanization, City, State,
            Zip5, Zip4, PostalCode, Country, Province, IsDefault, Type.ToEnum<AddressType>());
}
