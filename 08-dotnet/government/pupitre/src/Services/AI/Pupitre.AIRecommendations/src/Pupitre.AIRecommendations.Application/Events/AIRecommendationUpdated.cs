using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIRecommendations.Application.Events;

[Contract]
internal record AIRecommendationUpdated(Guid AIRecommendationId) : IEvent;


