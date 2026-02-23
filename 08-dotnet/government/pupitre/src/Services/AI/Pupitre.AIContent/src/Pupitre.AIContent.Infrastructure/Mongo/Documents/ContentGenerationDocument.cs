using Pupitre.AIContent.Application.DTO;
using Pupitre.AIContent.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Integration.Async")]
namespace Pupitre.AIContent.Infrastructure.Mongo.Documents;

internal class ContentGenerationDocument : IIdentifiable<Guid>
{
    public ContentGenerationDocument()
    {

    }

    public ContentGenerationDocument(ContentGeneration contentgeneration)
    {
        if (contentgeneration is null)
        {
            throw new NullReferenceException();
        }

        Id = contentgeneration.Id.Value;
        Name = contentgeneration.Name;
        CreatedAt = contentgeneration.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = contentgeneration.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = contentgeneration.Tags;
        Version = contentgeneration.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public ContentGeneration AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public ContentGenerationDto AsDto()
        => new ContentGenerationDto(Id, Name, Tags);
    public ContentGenerationDetailsDto AsDetailsDto()
        => new ContentGenerationDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

