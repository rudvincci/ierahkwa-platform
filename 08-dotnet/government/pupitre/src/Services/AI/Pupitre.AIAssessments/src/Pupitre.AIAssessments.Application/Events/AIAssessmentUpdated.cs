using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAssessments.Application.Events;

[Contract]
internal record AIAssessmentUpdated(Guid AIAssessmentId) : IEvent;


