using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIVision.Application.Events;

[Contract]
internal record VisionAnalysisUpdated(Guid VisionAnalysisId) : IEvent;


