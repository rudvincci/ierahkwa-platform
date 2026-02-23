using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIAssessments.Application.Events;
using Pupitre.AIAssessments.Domain.Events;

namespace Pupitre.AIAssessments.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // AIAssessment
            AIAssessmentCreated e => null, // Event published thru handler
            AIAssessmentModified e => new AIAssessmentUpdated(e.AIAssessment.Id),
            AIAssessmentRemoved e => new AIAssessmentDeleted(e.AIAssessment.Id),
            _ => null
        };
}

