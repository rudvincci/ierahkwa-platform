using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIRecommendations.Application.DTO;

internal class AIRecommendationDetailsDto : AIRecommendationDto
{
    public AIRecommendationDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AIRecommendationDetailsDto(AIRecommendationDto airecommendationDto, DateTime createdAt, DateTime? modifiedAt)
        : base(airecommendationDto.Id, airecommendationDto.Name, airecommendationDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
