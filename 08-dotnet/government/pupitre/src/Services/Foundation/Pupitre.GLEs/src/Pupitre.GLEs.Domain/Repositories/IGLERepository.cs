using System;
using Pupitre.GLEs.Domain.Entities;
using Mamey.Types;

namespace Pupitre.GLEs.Domain.Repositories;

internal interface IGLERepository
{
    Task AddAsync(GLE gle, CancellationToken cancellationToken = default);
    Task UpdateAsync(GLE gle, CancellationToken cancellationToken = default);
    Task DeleteAsync(GLEId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GLE>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<GLE> GetAsync(GLEId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(GLEId id, CancellationToken cancellationToken = default);
}
