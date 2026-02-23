using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Assessments.Application.Events;

[Contract]
internal record AssessmentDeleted(Guid AssessmentId) : IEvent;


