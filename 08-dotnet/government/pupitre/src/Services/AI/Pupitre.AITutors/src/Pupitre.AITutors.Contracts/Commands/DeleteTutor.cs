using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITutors.Contracts.Commands;

[Contract]
public record DeleteTutor(Guid Id) : ICommand;


