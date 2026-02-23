using Pupitre.AITutors.Application.DTO;
using Pupitre.AITutors.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.Integration.Async")]
namespace Pupitre.AITutors.Infrastructure.Mongo.Documents;

internal class TutorDocument : IIdentifiable<Guid>
{
    public TutorDocument()
    {

    }

    public TutorDocument(Tutor tutor)
    {
        if (tutor is null)
        {
            throw new NullReferenceException();
        }

        Id = tutor.Id.Value;
        Name = tutor.Name;
        CreatedAt = tutor.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = tutor.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = tutor.Tags;
        Version = tutor.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public Tutor AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public TutorDto AsDto()
        => new TutorDto(Id, Name, Tags);
    public TutorDetailsDto AsDetailsDto()
        => new TutorDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

