using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.DTO;

internal class IdentityDetailsDto : IdentityDto
{
    public IdentityDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public IdentityDetailsDto(IdentityDto identityDto, DateTime createdAt, DateTime? modifiedAt)
        : base(identityDto.Id, identityDto.Name, identityDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
