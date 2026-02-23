using Pupitre.Analytics.Application.DTO;
using Pupitre.Analytics.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.Integration.Async")]
namespace Pupitre.Analytics.Infrastructure.Mongo.Documents;

internal class AnalyticDocument : IIdentifiable<Guid>
{
    public AnalyticDocument()
    {

    }

    public AnalyticDocument(Analytic analytic)
    {
        if (analytic is null)
        {
            throw new NullReferenceException();
        }

        Id = analytic.Id.Value;
        Name = analytic.Name;
        CreatedAt = analytic.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = analytic.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = analytic.Tags;
        Version = analytic.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public Analytic AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AnalyticDto AsDto()
        => new AnalyticDto(Id, Name, Tags);
    public AnalyticDetailsDto AsDetailsDto()
        => new AnalyticDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

