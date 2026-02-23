using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIRecommendations.Application.Events;
using Pupitre.AIRecommendations.Domain.Events;

namespace Pupitre.AIRecommendations.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // AIRecommendation
            AIRecommendationCreated e => null, // Event published thru handler
            AIRecommendationModified e => new AIRecommendationUpdated(e.AIRecommendation.Id),
            AIRecommendationRemoved e => new AIRecommendationDeleted(e.AIRecommendation.Id),
            _ => null
        };
}

