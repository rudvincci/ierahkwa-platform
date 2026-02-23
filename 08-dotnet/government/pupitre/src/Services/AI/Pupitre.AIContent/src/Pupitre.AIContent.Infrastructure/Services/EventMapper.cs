using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIContent.Application.Events;
using Pupitre.AIContent.Domain.Events;

namespace Pupitre.AIContent.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // ContentGeneration
            ContentGenerationCreated e => null, // Event published thru handler
            ContentGenerationModified e => new ContentGenerationUpdated(e.ContentGeneration.Id),
            ContentGenerationRemoved e => new ContentGenerationDeleted(e.ContentGeneration.Id),
            _ => null
        };
}

