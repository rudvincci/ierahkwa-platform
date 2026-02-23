using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIAssessments.Application.DTO;

internal class AIAssessmentDetailsDto : AIAssessmentDto
{
    public AIAssessmentDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AIAssessmentDetailsDto(AIAssessmentDto aiassessmentDto, DateTime createdAt, DateTime? modifiedAt)
        : base(aiassessmentDto.Id, aiassessmentDto.Name, aiassessmentDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
