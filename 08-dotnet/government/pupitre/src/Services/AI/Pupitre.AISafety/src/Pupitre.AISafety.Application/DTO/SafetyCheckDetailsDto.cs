using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AISafety.Application.DTO;

internal class SafetyCheckDetailsDto : SafetyCheckDto
{
    public SafetyCheckDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public SafetyCheckDetailsDto(SafetyCheckDto safetycheckDto, DateTime createdAt, DateTime? modifiedAt)
        : base(safetycheckDto.Id, safetycheckDto.Name, safetycheckDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
