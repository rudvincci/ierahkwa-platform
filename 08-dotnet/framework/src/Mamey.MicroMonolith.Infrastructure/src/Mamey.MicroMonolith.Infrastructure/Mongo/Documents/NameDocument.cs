using Mamey.Types;
using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.MicroMonolith.Infrastructure.Mongo.Documents;
internal class NameDocument
{
    public NameDocument() { }

    public NameDocument(Name name)
    {
        FirstName = name.FirstName;
        MiddleName = name.MiddleName;
        LastName = name.LastName;
        Nickname = name.Nickname;
    }

    [BsonConstructor]
    public NameDocument(string firstName, string middleName, string lastName, string nickname)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Nickname = nickname;
    }

    [BsonElement("firstName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string FirstName { get; set; }
    [BsonElement("middleName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string MiddleName { get; set; }
    [BsonElement("lastName"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string LastName { get; set; }

    [BsonElement("nickname"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Nickname { get; set; }

    public Name AsEntity()
        => new Name(FirstName, LastName, MiddleName, Nickname);
}
