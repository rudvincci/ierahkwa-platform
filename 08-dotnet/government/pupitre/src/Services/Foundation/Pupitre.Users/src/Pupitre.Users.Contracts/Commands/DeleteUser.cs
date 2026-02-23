using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Users.Contracts.Commands;

[Contract]
public record DeleteUser(Guid Id) : ICommand;


