using Mamey.CQRS;
using Pupitre.Lessons.Domain.Entities;

namespace Pupitre.Lessons.Domain.Events;

internal record LessonModified(Lesson Lesson): IDomainEvent;

