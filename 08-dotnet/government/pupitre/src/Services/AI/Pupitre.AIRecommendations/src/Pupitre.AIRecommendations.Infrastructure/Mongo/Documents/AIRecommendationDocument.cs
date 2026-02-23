using Pupitre.AIRecommendations.Application.DTO;
using Pupitre.AIRecommendations.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.Integration.Async")]
namespace Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;

internal class AIRecommendationDocument : IIdentifiable<Guid>
{
    public AIRecommendationDocument()
    {

    }

    public AIRecommendationDocument(AIRecommendation airecommendation)
    {
        if (airecommendation is null)
        {
            throw new NullReferenceException();
        }

        Id = airecommendation.Id.Value;
        Name = airecommendation.Name;
        CreatedAt = airecommendation.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = airecommendation.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = airecommendation.Tags;
        Version = airecommendation.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public AIRecommendation AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AIRecommendationDto AsDto()
        => new AIRecommendationDto(Id, Name, Tags);
    public AIRecommendationDetailsDto AsDetailsDto()
        => new AIRecommendationDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

