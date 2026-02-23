using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.ServiceName.Contracts.Commands;

[Contract]
public record DeleteEntityName(Guid Id) : ICommand;


