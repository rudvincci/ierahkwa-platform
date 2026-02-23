using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Bookstore.Application.Events;
using Pupitre.Bookstore.Domain.Events;

namespace Pupitre.Bookstore.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Book
            BookCreated e => null, // Event published thru handler
            BookModified e => new BookUpdated(e.Book.Id),
            BookRemoved e => new BookDeleted(e.Book.Id),
            _ => null
        };
}

