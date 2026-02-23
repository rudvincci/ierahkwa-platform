using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIVision.Application.Events.Rejected;

[Contract]
internal record DeleteVisionAnalysisRejected(Guid VisionAnalysisId, string Reason, string Code) : IRejectedEvent;