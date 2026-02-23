using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIVision.Contracts.Commands;

[Contract]
public record UpdateVisionAnalysis : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();
}


