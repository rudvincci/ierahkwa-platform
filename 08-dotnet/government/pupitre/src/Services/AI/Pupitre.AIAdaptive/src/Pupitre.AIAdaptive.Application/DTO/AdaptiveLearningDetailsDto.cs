using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Application.DTO;

internal class AdaptiveLearningDetailsDto : AdaptiveLearningDto
{
    public AdaptiveLearningDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AdaptiveLearningDetailsDto(AdaptiveLearningDto adaptivelearningDto, DateTime createdAt, DateTime? modifiedAt)
        : base(adaptivelearningDto.Id, adaptivelearningDto.Name, adaptivelearningDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
