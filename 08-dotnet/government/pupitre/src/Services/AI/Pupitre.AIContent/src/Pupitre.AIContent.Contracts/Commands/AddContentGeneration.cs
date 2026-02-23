using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Integration.Async")]
namespace Pupitre.AIContent.Contracts.Commands;

[Contract]
public record AddContentGeneration : ICommand
{
    public AddContentGeneration(Guid id, string? name, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Tags = tags;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Name { get; init; }
    public IEnumerable<string> Tags { get; init; }
}

