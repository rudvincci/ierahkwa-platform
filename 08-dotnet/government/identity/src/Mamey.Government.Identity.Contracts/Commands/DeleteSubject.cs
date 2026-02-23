using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record DeleteSubject(Guid Id) : ICommand;


