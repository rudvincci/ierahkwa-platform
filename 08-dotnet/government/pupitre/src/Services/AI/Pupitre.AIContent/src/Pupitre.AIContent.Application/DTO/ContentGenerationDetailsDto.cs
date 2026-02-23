using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AIContent.Application.DTO;

internal class ContentGenerationDetailsDto : ContentGenerationDto
{
    public ContentGenerationDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public ContentGenerationDetailsDto(ContentGenerationDto contentgenerationDto, DateTime createdAt, DateTime? modifiedAt)
        : base(contentgenerationDto.Id, contentgenerationDto.Name, contentgenerationDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
