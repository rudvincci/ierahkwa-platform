using Pupitre.AIBehavior.Application.DTO;
using Pupitre.AIBehavior.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.Integration.Async")]
namespace Pupitre.AIBehavior.Infrastructure.Mongo.Documents;

internal class BehaviorDocument : IIdentifiable<Guid>
{
    public BehaviorDocument()
    {

    }

    public BehaviorDocument(Behavior behavior)
    {
        if (behavior is null)
        {
            throw new NullReferenceException();
        }

        Id = behavior.Id.Value;
        Name = behavior.Name;
        CreatedAt = behavior.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = behavior.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = behavior.Tags;
        Version = behavior.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public Behavior AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public BehaviorDto AsDto()
        => new BehaviorDto(Id, Name, Tags);
    public BehaviorDetailsDto AsDetailsDto()
        => new BehaviorDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

