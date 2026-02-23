using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Accessibility.Application.DTO;

internal class AccessProfileDetailsDto : AccessProfileDto
{
    public AccessProfileDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AccessProfileDetailsDto(AccessProfileDto accessprofileDto, DateTime createdAt, DateTime? modifiedAt)
        : base(accessprofileDto.Id, accessprofileDto.Name, accessprofileDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
