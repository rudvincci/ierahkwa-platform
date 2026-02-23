using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Assessments.Application.Events.Rejected;

[Contract]
internal record UpdateAssessmentRejected(Guid AssessmentId, string Reason, string Code) : IRejectedEvent;
