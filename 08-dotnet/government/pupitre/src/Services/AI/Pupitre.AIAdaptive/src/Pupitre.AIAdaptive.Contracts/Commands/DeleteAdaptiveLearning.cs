using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAdaptive.Contracts.Commands;

[Contract]
public record DeleteAdaptiveLearning(Guid Id) : ICommand;


