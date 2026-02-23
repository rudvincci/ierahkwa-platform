using Mamey.CQRS;
using Pupitre.AIRecommendations.Domain.Entities;

namespace Pupitre.AIRecommendations.Domain.Events;

internal record AIRecommendationCreated(AIRecommendation AIRecommendation) : IDomainEvent;

