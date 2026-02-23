using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Operations.Contracts.Commands;

[Contract]
public record DeleteOperationMetric(Guid Id) : ICommand;


