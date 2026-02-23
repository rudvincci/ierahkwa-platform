using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Progress.Contracts.Commands;

[Contract]
public record DeleteLearningProgress(Guid Id) : ICommand;


