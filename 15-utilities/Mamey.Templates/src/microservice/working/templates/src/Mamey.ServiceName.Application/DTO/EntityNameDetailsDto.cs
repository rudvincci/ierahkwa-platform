using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.ServiceName.Application.DTO;

internal class EntityNameDetailsDto : EntityNameDto
{
    public EntityNameDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public EntityNameDetailsDto(EntityNameDto entitynameDto, DateTime createdAt, DateTime? modifiedAt)
        : base(entitynameDto.Id, entitynameDto.Name, entitynameDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
