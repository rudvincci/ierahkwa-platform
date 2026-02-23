using Pupitre.Operations.Application.DTO;
using Pupitre.Operations.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Operations.Tests.Integration.Async")]
namespace Pupitre.Operations.Infrastructure.Mongo.Documents;

internal class OperationMetricDocument : IIdentifiable<Guid>
{
    public OperationMetricDocument()
    {

    }

    public OperationMetricDocument(OperationMetric operationmetric)
    {
        if (operationmetric is null)
        {
            throw new NullReferenceException();
        }

        Id = operationmetric.Id.Value;
        Name = operationmetric.Name;
        CreatedAt = operationmetric.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = operationmetric.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = operationmetric.Tags;
        Version = operationmetric.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public OperationMetric AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public OperationMetricDto AsDto()
        => new OperationMetricDto(Id, Name, Tags);
    public OperationMetricDetailsDto AsDetailsDto()
        => new OperationMetricDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

