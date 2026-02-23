using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Lessons.Application.Events.Rejected;

[Contract]
internal record AddLessonRejected(Guid LessonId, string Reason, string Code) : IRejectedEvent;
