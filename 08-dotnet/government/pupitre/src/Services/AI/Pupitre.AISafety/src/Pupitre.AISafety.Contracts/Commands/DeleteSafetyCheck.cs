using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISafety.Contracts.Commands;

[Contract]
public record DeleteSafetyCheck(Guid Id) : ICommand;


