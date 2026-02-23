using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAssessments.Contracts.Commands;

[Contract]
public record DeleteAIAssessment(Guid Id) : ICommand;


