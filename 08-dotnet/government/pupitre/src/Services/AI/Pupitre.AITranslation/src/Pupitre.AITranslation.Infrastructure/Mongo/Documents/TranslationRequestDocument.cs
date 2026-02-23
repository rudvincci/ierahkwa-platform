using Pupitre.AITranslation.Application.DTO;
using Pupitre.AITranslation.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Integration.Async")]
namespace Pupitre.AITranslation.Infrastructure.Mongo.Documents;

internal class TranslationRequestDocument : IIdentifiable<Guid>
{
    public TranslationRequestDocument()
    {

    }

    public TranslationRequestDocument(TranslationRequest translationrequest)
    {
        if (translationrequest is null)
        {
            throw new NullReferenceException();
        }

        Id = translationrequest.Id.Value;
        Name = translationrequest.Name;
        CreatedAt = translationrequest.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = translationrequest.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = translationrequest.Tags;
        Version = translationrequest.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public TranslationRequest AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public TranslationRequestDto AsDto()
        => new TranslationRequestDto(Id, Name, Tags);
    public TranslationRequestDetailsDto AsDetailsDto()
        => new TranslationRequestDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

