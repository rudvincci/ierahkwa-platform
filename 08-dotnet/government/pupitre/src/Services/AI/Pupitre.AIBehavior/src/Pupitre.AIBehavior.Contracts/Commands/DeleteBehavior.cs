using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIBehavior.Contracts.Commands;

[Contract]
public record DeleteBehavior(Guid Id) : ICommand;


