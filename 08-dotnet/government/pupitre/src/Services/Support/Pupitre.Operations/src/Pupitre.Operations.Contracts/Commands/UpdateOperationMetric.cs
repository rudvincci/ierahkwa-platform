using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Operations.Contracts.Commands;

[Contract]
public record UpdateOperationMetric : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();
}


