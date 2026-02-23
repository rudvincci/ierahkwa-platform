using Mamey.CQRS;
using Pupitre.Lessons.Domain.Entities;

namespace Pupitre.Lessons.Domain.Events;

internal record LessonRemoved(Lesson Lesson) : IDomainEvent;