using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAssessments.Application.Events.Rejected;

[Contract]
internal record UpdateAIAssessmentRejected(Guid AIAssessmentId, string Reason, string Code) : IRejectedEvent;
