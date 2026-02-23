using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Analytics.Application.DTO;

internal class AnalyticDetailsDto : AnalyticDto
{
    public AnalyticDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AnalyticDetailsDto(AnalyticDto analyticDto, DateTime createdAt, DateTime? modifiedAt)
        : base(analyticDto.Id, analyticDto.Name, analyticDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
