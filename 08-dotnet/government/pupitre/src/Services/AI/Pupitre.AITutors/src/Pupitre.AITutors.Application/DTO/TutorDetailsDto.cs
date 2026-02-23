using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AITutors.Application.DTO;

internal class TutorDetailsDto : TutorDto
{
    public TutorDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public TutorDetailsDto(TutorDto tutorDto, DateTime createdAt, DateTime? modifiedAt)
        : base(tutorDto.Id, tutorDto.Name, tutorDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
