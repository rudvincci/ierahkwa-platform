using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.ServiceName.Contracts.Commands;

[Contract]
public record UpdateEntityName : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<string> Tags { get; init; }
}


