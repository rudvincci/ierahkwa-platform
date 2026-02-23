using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Progress.Application.Events;
using Pupitre.Progress.Domain.Events;

namespace Pupitre.Progress.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // LearningProgress
            LearningProgressCreated e => null, // Event published thru handler
            LearningProgressModified e => new LearningProgressUpdated(e.LearningProgress.Id),
            LearningProgressRemoved e => new LearningProgressDeleted(e.LearningProgress.Id),
            LearningProgressBlockchainRegistered e => new LearningProgressCredentialIssued(e.LearningProgressId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

