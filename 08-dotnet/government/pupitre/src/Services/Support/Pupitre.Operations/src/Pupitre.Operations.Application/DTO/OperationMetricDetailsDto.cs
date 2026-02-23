using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Operations.Application.DTO;

internal class OperationMetricDetailsDto : OperationMetricDto
{
    public OperationMetricDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public OperationMetricDetailsDto(OperationMetricDto operationmetricDto, DateTime createdAt, DateTime? modifiedAt)
        : base(operationmetricDto.Id, operationmetricDto.Name, operationmetricDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
