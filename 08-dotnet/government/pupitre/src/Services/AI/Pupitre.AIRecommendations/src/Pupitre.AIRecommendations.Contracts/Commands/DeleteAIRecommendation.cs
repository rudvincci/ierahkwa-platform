using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIRecommendations.Contracts.Commands;

[Contract]
public record DeleteAIRecommendation(Guid Id) : ICommand;


