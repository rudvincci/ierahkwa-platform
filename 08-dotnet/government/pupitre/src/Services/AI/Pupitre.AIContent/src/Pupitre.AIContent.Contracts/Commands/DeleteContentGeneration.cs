using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIContent.Contracts.Commands;

[Contract]
public record DeleteContentGeneration(Guid Id) : ICommand;


