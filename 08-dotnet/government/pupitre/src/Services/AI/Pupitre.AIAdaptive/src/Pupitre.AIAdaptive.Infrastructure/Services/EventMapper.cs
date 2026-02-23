using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIAdaptive.Application.Events;
using Pupitre.AIAdaptive.Domain.Events;

namespace Pupitre.AIAdaptive.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // AdaptiveLearning
            AdaptiveLearningCreated e => null, // Event published thru handler
            AdaptiveLearningModified e => new AdaptiveLearningUpdated(e.AdaptiveLearning.Id),
            AdaptiveLearningRemoved e => new AdaptiveLearningDeleted(e.AdaptiveLearning.Id),
            _ => null
        };
}

