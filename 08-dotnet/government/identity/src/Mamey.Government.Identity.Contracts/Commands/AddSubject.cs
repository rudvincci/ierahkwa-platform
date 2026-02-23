using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record AddSubject : ICommand
{
    public AddSubject(Guid id, string? name, string? email, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Email = email;
        Tags = tags;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Name { get; init; }
    public string? Email { get; init; }
    public IEnumerable<string> Tags { get; init; }
}

