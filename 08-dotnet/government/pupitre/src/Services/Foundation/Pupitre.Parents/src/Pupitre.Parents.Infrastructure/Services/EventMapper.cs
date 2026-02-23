using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Parents.Application.Events;
using Pupitre.Parents.Domain.Events;

namespace Pupitre.Parents.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Parent
            ParentCreated e => null, // Event published thru handler
            ParentModified e => new ParentUpdated(e.Parent.Id),
            ParentRemoved e => new ParentDeleted(e.Parent.Id),
            ParentBlockchainRegistered e => new ParentCredentialIssued(e.ParentId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

