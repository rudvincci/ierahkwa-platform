using Pupitre.AISpeech.Application.DTO;
using Pupitre.AISpeech.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.Integration.Async")]
namespace Pupitre.AISpeech.Infrastructure.Mongo.Documents;

internal class SpeechRequestDocument : IIdentifiable<Guid>
{
    public SpeechRequestDocument()
    {

    }

    public SpeechRequestDocument(SpeechRequest speechrequest)
    {
        if (speechrequest is null)
        {
            throw new NullReferenceException();
        }

        Id = speechrequest.Id.Value;
        Name = speechrequest.Name;
        CreatedAt = speechrequest.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = speechrequest.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = speechrequest.Tags;
        Version = speechrequest.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public SpeechRequest AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public SpeechRequestDto AsDto()
        => new SpeechRequestDto(Id, Name, Tags);
    public SpeechRequestDetailsDto AsDetailsDto()
        => new SpeechRequestDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

