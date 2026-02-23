using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Lessons.Application.Events;

[Contract]
internal record LessonUpdated(Guid LessonId) : IEvent;


