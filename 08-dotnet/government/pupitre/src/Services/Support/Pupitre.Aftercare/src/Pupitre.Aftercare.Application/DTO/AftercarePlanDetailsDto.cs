using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Aftercare.Application.DTO;

internal class AftercarePlanDetailsDto : AftercarePlanDto
{
    public AftercarePlanDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public AftercarePlanDetailsDto(AftercarePlanDto aftercareplanDto, DateTime createdAt, DateTime? modifiedAt)
        : base(aftercareplanDto.Id, aftercareplanDto.Name, aftercareplanDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
