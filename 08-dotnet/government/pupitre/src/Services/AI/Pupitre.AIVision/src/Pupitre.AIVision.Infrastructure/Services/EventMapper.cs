using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.AIVision.Application.Events;
using Pupitre.AIVision.Domain.Events;

namespace Pupitre.AIVision.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // VisionAnalysis
            VisionAnalysisCreated e => null, // Event published thru handler
            VisionAnalysisModified e => new VisionAnalysisUpdated(e.VisionAnalysis.Id),
            VisionAnalysisRemoved e => new VisionAnalysisDeleted(e.VisionAnalysis.Id),
            _ => null
        };
}

