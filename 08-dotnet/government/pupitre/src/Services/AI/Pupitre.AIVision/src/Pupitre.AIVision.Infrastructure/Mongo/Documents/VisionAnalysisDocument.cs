using Pupitre.AIVision.Application.DTO;
using Pupitre.AIVision.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.Integration.Async")]
namespace Pupitre.AIVision.Infrastructure.Mongo.Documents;

internal class VisionAnalysisDocument : IIdentifiable<Guid>
{
    public VisionAnalysisDocument()
    {

    }

    public VisionAnalysisDocument(VisionAnalysis visionanalysis)
    {
        if (visionanalysis is null)
        {
            throw new NullReferenceException();
        }

        Id = visionanalysis.Id.Value;
        Name = visionanalysis.Name;
        CreatedAt = visionanalysis.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = visionanalysis.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = visionanalysis.Tags;
        Version = visionanalysis.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public VisionAnalysis AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public VisionAnalysisDto AsDto()
        => new VisionAnalysisDto(Id, Name, Tags);
    public VisionAnalysisDetailsDto AsDetailsDto()
        => new VisionAnalysisDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

