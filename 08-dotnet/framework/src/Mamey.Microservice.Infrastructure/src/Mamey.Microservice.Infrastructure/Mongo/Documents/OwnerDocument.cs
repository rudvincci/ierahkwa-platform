using MongoDB.Bson.Serialization.Attributes;

namespace Mamey.Microservice.Infrastructure.Mongo.Documents;

internal class OwnerDocument : IIdentifiable<Guid>
{
    public OwnerDocument() 
    {
        Email = string.Empty;
        Name = new NameDocument();
        Phone = new PhoneDocument();
    }

    public OwnerDocument(Owner owner)
    {
        if(owner is null)
        {
            throw new ArgumentNullException(nameof(owner));
        }
        Id = owner.Id;
        Name = owner.Name is not null ? new NameDocument(owner.Name) : new NameDocument();
        Phone = owner.Phone is not null ? new PhoneDocument(owner.Phone) : new PhoneDocument();
        Email = owner.Email ?? string.Empty;
    }

    public Guid Id { get; set; }
    [BsonElement("email"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Email { get; set; }
    public NameDocument Name { get; set; }
    public PhoneDocument Phone { get; set; }
    


    public Owner AsEntity()
        => new Owner(Id, Name.AsEntity(), Email, Phone.AsEntity());
}