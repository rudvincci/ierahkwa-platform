using Pupitre.AISafety.Application.DTO;
using Pupitre.AISafety.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.Integration.Async")]
namespace Pupitre.AISafety.Infrastructure.Mongo.Documents;

internal class SafetyCheckDocument : IIdentifiable<Guid>
{
    public SafetyCheckDocument()
    {

    }

    public SafetyCheckDocument(SafetyCheck safetycheck)
    {
        if (safetycheck is null)
        {
            throw new NullReferenceException();
        }

        Id = safetycheck.Id.Value;
        Name = safetycheck.Name;
        CreatedAt = safetycheck.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = safetycheck.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = safetycheck.Tags;
        Version = safetycheck.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public SafetyCheck AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public SafetyCheckDto AsDto()
        => new SafetyCheckDto(Id, Name, Tags);
    public SafetyCheckDetailsDto AsDetailsDto()
        => new SafetyCheckDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

