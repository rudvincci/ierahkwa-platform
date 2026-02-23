using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Educators.Contracts.Commands;

[Contract]
public record DeleteEducator(Guid Id) : ICommand;


