using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.AITranslation.Application.DTO;

internal class TranslationRequestDetailsDto : TranslationRequestDto
{
    public TranslationRequestDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public TranslationRequestDetailsDto(TranslationRequestDto translationrequestDto, DateTime createdAt, DateTime? modifiedAt)
        : base(translationrequestDto.Id, translationrequestDto.Name, translationrequestDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
