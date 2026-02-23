using System;
using Pupitre.Analytics.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Analytics.Domain.Repositories;

internal interface IAnalyticRepository
{
    Task AddAsync(Analytic analytic, CancellationToken cancellationToken = default);
    Task UpdateAsync(Analytic analytic, CancellationToken cancellationToken = default);
    Task DeleteAsync(AnalyticId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Analytic>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Analytic> GetAsync(AnalyticId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AnalyticId id, CancellationToken cancellationToken = default);
}
