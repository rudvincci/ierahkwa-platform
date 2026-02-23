using System;
using Pupitre.AISpeech.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AISpeech.Domain.Repositories;

internal interface ISpeechRequestRepository
{
    Task AddAsync(SpeechRequest speechrequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(SpeechRequest speechrequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(SpeechRequestId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpeechRequest>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<SpeechRequest> GetAsync(SpeechRequestId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SpeechRequestId id, CancellationToken cancellationToken = default);
}
