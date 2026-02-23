using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Bookstore.Contracts.Commands;

[Contract]
public record UpdateBook : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();
}


