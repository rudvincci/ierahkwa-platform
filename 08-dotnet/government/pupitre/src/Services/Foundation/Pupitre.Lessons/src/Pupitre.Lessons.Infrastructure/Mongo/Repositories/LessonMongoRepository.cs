using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Lessons.Domain.Repositories;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Lessons.Infrastructure.Mongo.Repositories;

internal class LessonMongoRepository : ILessonRepository
{
    private readonly IMongoRepository<LessonDocument, Guid> _repository;

    public LessonMongoRepository(IMongoRepository<LessonDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new LessonDocument(lesson));

    public async Task UpdateAsync(Lesson lesson, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new LessonDocument(lesson));
    public async Task DeleteAsync(LessonId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Lesson>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Lesson> GetAsync(LessonId id, CancellationToken cancellationToken = default)
    {
        var lesson = await _repository.GetAsync(id.Value);
        return lesson?.AsEntity();
    }
    public async Task<bool> ExistsAsync(LessonId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



