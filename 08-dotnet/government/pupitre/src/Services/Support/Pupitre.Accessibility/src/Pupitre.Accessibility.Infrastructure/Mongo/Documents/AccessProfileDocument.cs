using Pupitre.Accessibility.Application.DTO;
using Pupitre.Accessibility.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.Integration.Async")]
namespace Pupitre.Accessibility.Infrastructure.Mongo.Documents;

internal class AccessProfileDocument : IIdentifiable<Guid>
{
    public AccessProfileDocument()
    {

    }

    public AccessProfileDocument(AccessProfile accessprofile)
    {
        if (accessprofile is null)
        {
            throw new NullReferenceException();
        }

        Id = accessprofile.Id.Value;
        Name = accessprofile.Name;
        CreatedAt = accessprofile.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = accessprofile.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = accessprofile.Tags;
        Version = accessprofile.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public AccessProfile AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AccessProfileDto AsDto()
        => new AccessProfileDto(Id, Name, Tags);
    public AccessProfileDetailsDto AsDetailsDto()
        => new AccessProfileDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

