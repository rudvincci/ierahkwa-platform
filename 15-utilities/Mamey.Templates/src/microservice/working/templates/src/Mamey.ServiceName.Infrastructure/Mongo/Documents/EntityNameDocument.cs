using Mamey.ServiceName.Application.DTO;
using Mamey.ServiceName.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.ServiceName.Tests.Integration.Async")]
namespace Mamey.ServiceName.Infrastructure.Mongo.Documents;

internal class EntityNameDocument : IIdentifiable<Guid>
{
    public EntityNameDocument()
    {

    }

    public EntityNameDocument(EntityName entityname)
    {
        if (entityname is null)
        {
            throw new NullReferenceException();
        }

        Id = entityname.Id.Value;
        Name = entityname.Name;
        CreatedAt = entityname.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = entityname.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = entityname.Tags;
        Version = entityname.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public EntityName AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public EntityNameDto AsDto()
        => new EntityNameDto(Id, Name, Tags);
    public EntityNameDetailsDto AsDetailsDto()
        => new EntityNameDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

