using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIContent.Domain.Repositories;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIContent.Infrastructure.Mongo.Repositories;

internal class ContentGenerationMongoRepository : IContentGenerationRepository
{
    private readonly IMongoRepository<ContentGenerationDocument, Guid> _repository;

    public ContentGenerationMongoRepository(IMongoRepository<ContentGenerationDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(ContentGeneration contentgeneration, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new ContentGenerationDocument(contentgeneration));

    public async Task UpdateAsync(ContentGeneration contentgeneration, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new ContentGenerationDocument(contentgeneration));
    public async Task DeleteAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<ContentGeneration>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<ContentGeneration> GetAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
    {
        var contentgeneration = await _repository.GetAsync(id.Value);
        return contentgeneration?.AsEntity();
    }
    public async Task<bool> ExistsAsync(ContentGenerationId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



