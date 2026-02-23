using System;
using Pupitre.Lessons.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Lessons.Domain.Repositories;

internal interface ILessonRepository
{
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default);
    Task UpdateAsync(Lesson lesson, CancellationToken cancellationToken = default);
    Task DeleteAsync(LessonId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lesson>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Lesson> GetAsync(LessonId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(LessonId id, CancellationToken cancellationToken = default);
}
