using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Domain.Events;

namespace Mamey.Government.Identity.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Subject
            SubjectCreated e => null, // Event published thru handler
            SubjectModified e => new SubjectUpdated(e.Subject.Id),
            SubjectRemoved e => new SubjectDeleted(e.Subject.Id),
            _ => null
        };
}

