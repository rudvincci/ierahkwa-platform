using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Compliance.Application.DTO;

internal class ComplianceRecordDetailsDto : ComplianceRecordDto
{
    public ComplianceRecordDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public ComplianceRecordDetailsDto(ComplianceRecordDto compliancerecordDto, DateTime createdAt, DateTime? modifiedAt)
        : base(compliancerecordDto.Id, compliancerecordDto.Name, compliancerecordDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
