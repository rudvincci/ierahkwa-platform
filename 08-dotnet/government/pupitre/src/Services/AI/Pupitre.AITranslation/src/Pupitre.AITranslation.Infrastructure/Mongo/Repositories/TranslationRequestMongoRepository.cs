using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AITranslation.Domain.Repositories;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AITranslation.Infrastructure.Mongo.Repositories;

internal class TranslationRequestMongoRepository : ITranslationRequestRepository
{
    private readonly IMongoRepository<TranslationRequestDocument, Guid> _repository;

    public TranslationRequestMongoRepository(IMongoRepository<TranslationRequestDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(TranslationRequest translationrequest, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new TranslationRequestDocument(translationrequest));

    public async Task UpdateAsync(TranslationRequest translationrequest, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new TranslationRequestDocument(translationrequest));
    public async Task DeleteAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<TranslationRequest>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<TranslationRequest> GetAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
    {
        var translationrequest = await _repository.GetAsync(id.Value);
        return translationrequest?.AsEntity();
    }
    public async Task<bool> ExistsAsync(TranslationRequestId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



