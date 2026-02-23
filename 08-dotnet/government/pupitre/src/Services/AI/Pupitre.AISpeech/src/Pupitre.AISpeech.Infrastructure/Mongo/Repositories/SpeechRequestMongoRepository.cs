using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AISpeech.Domain.Repositories;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AISpeech.Infrastructure.Mongo.Repositories;

internal class SpeechRequestMongoRepository : ISpeechRequestRepository
{
    private readonly IMongoRepository<SpeechRequestDocument, Guid> _repository;

    public SpeechRequestMongoRepository(IMongoRepository<SpeechRequestDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(SpeechRequest speechrequest, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new SpeechRequestDocument(speechrequest));

    public async Task UpdateAsync(SpeechRequest speechrequest, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new SpeechRequestDocument(speechrequest));
    public async Task DeleteAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<SpeechRequest>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<SpeechRequest> GetAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
    {
        var speechrequest = await _repository.GetAsync(id.Value);
        return speechrequest?.AsEntity();
    }
    public async Task<bool> ExistsAsync(SpeechRequestId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



