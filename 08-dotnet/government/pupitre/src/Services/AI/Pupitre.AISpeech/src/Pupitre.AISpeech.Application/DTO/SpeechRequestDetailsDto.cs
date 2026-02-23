using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AISpeech.Application.DTO;

internal class SpeechRequestDetailsDto : SpeechRequestDto
{
    public SpeechRequestDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public SpeechRequestDetailsDto(SpeechRequestDto speechrequestDto, DateTime createdAt, DateTime? modifiedAt)
        : base(speechrequestDto.Id, speechrequestDto.Name, speechrequestDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
