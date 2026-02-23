using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIBehavior.Application.DTO;

internal class BehaviorDetailsDto : BehaviorDto
{
    public BehaviorDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public BehaviorDetailsDto(BehaviorDto behaviorDto, DateTime createdAt, DateTime? modifiedAt)
        : base(behaviorDto.Id, behaviorDto.Name, behaviorDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
