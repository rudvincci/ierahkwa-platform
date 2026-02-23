using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIVision.Application.DTO;

internal class VisionAnalysisDetailsDto : VisionAnalysisDto
{
    public VisionAnalysisDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public VisionAnalysisDetailsDto(VisionAnalysisDto visionanalysisDto, DateTime createdAt, DateTime? modifiedAt)
        : base(visionanalysisDto.Id, visionanalysisDto.Name, visionanalysisDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
