using System;
using Pupitre.AITranslation.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AITranslation.Domain.Repositories;

internal interface ITranslationRequestRepository
{
    Task AddAsync(TranslationRequest translationrequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(TranslationRequest translationrequest, CancellationToken cancellationToken = default);
    Task DeleteAsync(TranslationRequestId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TranslationRequest>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<TranslationRequest> GetAsync(TranslationRequestId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TranslationRequestId id, CancellationToken cancellationToken = default);
}
