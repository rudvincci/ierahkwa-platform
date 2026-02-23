using Pupitre.AIAdaptive.Application.DTO;
using Pupitre.AIAdaptive.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.Integration.Async")]
namespace Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;

internal class AdaptiveLearningDocument : IIdentifiable<Guid>
{
    public AdaptiveLearningDocument()
    {

    }

    public AdaptiveLearningDocument(AdaptiveLearning adaptivelearning)
    {
        if (adaptivelearning is null)
        {
            throw new NullReferenceException();
        }

        Id = adaptivelearning.Id.Value;
        Name = adaptivelearning.Name;
        CreatedAt = adaptivelearning.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = adaptivelearning.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = adaptivelearning.Tags;
        Version = adaptivelearning.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public AdaptiveLearning AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AdaptiveLearningDto AsDto()
        => new AdaptiveLearningDto(Id, Name, Tags);
    public AdaptiveLearningDetailsDto AsDetailsDto()
        => new AdaptiveLearningDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

