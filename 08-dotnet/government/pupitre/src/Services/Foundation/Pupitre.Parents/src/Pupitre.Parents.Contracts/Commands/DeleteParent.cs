using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Parents.Contracts.Commands;

[Contract]
public record DeleteParent(Guid Id) : ICommand;


