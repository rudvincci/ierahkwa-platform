using Pupitre.AIAssessments.Application.DTO;
using Pupitre.AIAssessments.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.Integration.Async")]
namespace Pupitre.AIAssessments.Infrastructure.Mongo.Documents;

internal class AIAssessmentDocument : IIdentifiable<Guid>
{
    public AIAssessmentDocument()
    {

    }

    public AIAssessmentDocument(AIAssessment aiassessment)
    {
        if (aiassessment is null)
        {
            throw new NullReferenceException();
        }

        Id = aiassessment.Id.Value;
        Name = aiassessment.Name;
        CreatedAt = aiassessment.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = aiassessment.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = aiassessment.Tags;
        Version = aiassessment.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public AIAssessment AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public AIAssessmentDto AsDto()
        => new AIAssessmentDto(Id, Name, Tags);
    public AIAssessmentDetailsDto AsDetailsDto()
        => new AIAssessmentDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

