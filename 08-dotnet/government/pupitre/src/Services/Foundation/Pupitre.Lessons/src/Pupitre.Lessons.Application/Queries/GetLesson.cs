using System;
using Mamey.CQRS.Queries;
using Pupitre.Lessons.Application.DTO;

namespace Pupitre.Lessons.Application.Queries;

internal record GetLesson(Guid Id) : IQuery<LessonDetailsDto>;
