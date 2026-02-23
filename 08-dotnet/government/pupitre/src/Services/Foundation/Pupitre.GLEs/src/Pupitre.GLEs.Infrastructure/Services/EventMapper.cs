using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.GLEs.Application.Events;
using Pupitre.GLEs.Domain.Events;

namespace Pupitre.GLEs.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // GLE
            GLECreated e => null, // Event published thru handler
            GLEModified e => new GLEUpdated(e.GLE.Id),
            GLERemoved e => new GLEDeleted(e.GLE.Id),
            GLEBlockchainRegistered e => new GLECredentialIssued(e.GLEId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

