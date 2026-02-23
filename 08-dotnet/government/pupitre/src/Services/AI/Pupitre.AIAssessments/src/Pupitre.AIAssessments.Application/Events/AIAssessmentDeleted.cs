using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAssessments.Application.Events;

[Contract]
internal record AIAssessmentDeleted(Guid AIAssessmentId) : IEvent;


