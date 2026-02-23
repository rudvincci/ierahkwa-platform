using Pupitre.Aftercare.Application.DTO;
using Pupitre.Aftercare.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.Integration.Async")]
namespace Pupitre.Aftercare.Infrastructure.Mongo.Documents;

internal class AftercarePlanDocument : IIdentifiable<Guid>
{
    public AftercarePlanDocument()
    {

    }

    public AftercarePlanDocument(AftercarePlan aftercareplan)
    {
        if (aftercareplan is null)
        {
            throw new NullReferenceException();
        }

        Id = aftercareplan.Id.Value;
        Name = aftercareplan.Name;
        CreatedAt = aftercareplan.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = aftercareplan.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = aftercareplan.Tags;
        Version = aftercareplan.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public AftercarePlan AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AftercarePlanDto AsDto()
        => new AftercarePlanDto(Id, Name, Tags);
    public AftercarePlanDetailsDto AsDetailsDto()
        => new AftercarePlanDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

