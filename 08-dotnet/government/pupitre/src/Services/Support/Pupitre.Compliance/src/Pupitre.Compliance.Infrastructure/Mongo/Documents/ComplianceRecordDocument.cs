using Pupitre.Compliance.Application.DTO;
using Pupitre.Compliance.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.Integration.Async")]
namespace Pupitre.Compliance.Infrastructure.Mongo.Documents;

internal class ComplianceRecordDocument : IIdentifiable<Guid>
{
    public ComplianceRecordDocument()
    {

    }

    public ComplianceRecordDocument(ComplianceRecord compliancerecord)
    {
        if (compliancerecord is null)
        {
            throw new NullReferenceException();
        }

        Id = compliancerecord.Id.Value;
        Name = compliancerecord.Name;
        CreatedAt = compliancerecord.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = compliancerecord.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = compliancerecord.Tags;
        Version = compliancerecord.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public ComplianceRecord AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public ComplianceRecordDto AsDto()
        => new ComplianceRecordDto(Id, Name, Tags);
    public ComplianceRecordDetailsDto AsDetailsDto()
        => new ComplianceRecordDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

