using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Aftercare.Contracts.Commands;

[Contract]
public record DeleteAftercarePlan(Guid Id) : ICommand;


