using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Assessments.Application.Events;
using Pupitre.Assessments.Domain.Events;

namespace Pupitre.Assessments.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Assessment
            AssessmentCreated e => null, // Event published thru handler
            AssessmentModified e => new AssessmentUpdated(e.Assessment.Id),
            AssessmentRemoved e => new AssessmentDeleted(e.Assessment.Id),
            AssessmentBlockchainRegistered e => new AssessmentCredentialIssued(e.AssessmentId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

