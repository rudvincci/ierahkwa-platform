using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.GLEs.Contracts.Commands;

[Contract]
public record DeleteGLE(Guid Id) : ICommand;


