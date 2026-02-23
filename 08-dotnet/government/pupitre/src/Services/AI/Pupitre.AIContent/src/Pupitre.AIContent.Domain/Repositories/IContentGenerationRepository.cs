using System;
using Pupitre.AIContent.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIContent.Domain.Repositories;

internal interface IContentGenerationRepository
{
    Task AddAsync(ContentGeneration contentgeneration, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContentGeneration contentgeneration, CancellationToken cancellationToken = default);
    Task DeleteAsync(ContentGenerationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentGeneration>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<ContentGeneration> GetAsync(ContentGenerationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(ContentGenerationId id, CancellationToken cancellationToken = default);
}
