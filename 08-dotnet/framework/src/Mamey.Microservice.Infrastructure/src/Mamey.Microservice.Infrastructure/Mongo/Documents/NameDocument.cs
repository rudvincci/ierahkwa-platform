using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.Microservice.Infrastructure.Mongo.Documents;

internal class NameDocument
{
    public NameDocument() 
    {
        FirstName = string.Empty;
        MiddleName = string.Empty;
        LastName = string.Empty;
        Nickname = string.Empty;
    }

    public NameDocument(Name name)
    {
        FirstName = name.FirstName ?? string.Empty;
        MiddleName = name.MiddleName ?? string.Empty;
        LastName = name.LastName ?? string.Empty;
        Nickname = name.Nickname ?? string.Empty;
    }

    [BsonConstructor]
    public NameDocument(string firstName, string middleName, string lastName, string nickname)
    {
        FirstName = firstName ?? string.Empty;
        MiddleName = middleName ?? string.Empty;
        LastName = lastName ?? string.Empty;
        //Surname = surname;
        Nickname = nickname ?? string.Empty;
    }

    [BsonElement("firstName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string FirstName { get; set; }
    [BsonElement("middleName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string MiddleName { get; set; }
    [BsonElement("lastName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string LastName { get; set; }
    //[BsonElement("surname"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    //public string Surname { get; set; }
    [BsonElement("nickname"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Nickname { get; set; }

    public Name AsEntity()
        => new Name(FirstName, LastName, MiddleName, Nickname);
}
