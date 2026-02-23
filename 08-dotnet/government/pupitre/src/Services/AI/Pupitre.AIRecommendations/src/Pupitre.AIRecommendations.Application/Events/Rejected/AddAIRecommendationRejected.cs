using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIRecommendations.Application.Events.Rejected;

[Contract]
internal record AddAIRecommendationRejected(Guid AIRecommendationId, string Reason, string Code) : IRejectedEvent;
