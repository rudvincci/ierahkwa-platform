using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Integration.Async")]
namespace Pupitre.AITranslation.Contracts.Commands;

[Contract]
public record AddTranslationRequest : ICommand
{
    public AddTranslationRequest(Guid id, string? name, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Tags = tags;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Name { get; init; }
    public IEnumerable<string> Tags { get; init; }
}

