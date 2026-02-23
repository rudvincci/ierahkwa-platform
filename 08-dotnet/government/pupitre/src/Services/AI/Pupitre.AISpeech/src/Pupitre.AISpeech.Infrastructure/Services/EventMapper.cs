using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AISpeech.Application.Events;
using Pupitre.AISpeech.Domain.Events;

namespace Pupitre.AISpeech.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // SpeechRequest
            SpeechRequestCreated e => null, // Event published thru handler
            SpeechRequestModified e => new SpeechRequestUpdated(e.SpeechRequest.Id),
            SpeechRequestRemoved e => new SpeechRequestDeleted(e.SpeechRequest.Id),
            _ => null
        };
}

