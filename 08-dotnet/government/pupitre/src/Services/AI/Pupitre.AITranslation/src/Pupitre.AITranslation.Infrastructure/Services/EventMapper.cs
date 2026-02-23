using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AITranslation.Application.Events;
using Pupitre.AITranslation.Domain.Events;

namespace Pupitre.AITranslation.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // TranslationRequest
            TranslationRequestCreated e => null, // Event published thru handler
            TranslationRequestModified e => new TranslationRequestUpdated(e.TranslationRequest.Id),
            TranslationRequestRemoved e => new TranslationRequestDeleted(e.TranslationRequest.Id),
            _ => null
        };
}

