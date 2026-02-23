using System;
using Pupitre.Educators.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Educators.Domain.Repositories;

internal interface IEducatorRepository
{
    Task AddAsync(Educator educator, CancellationToken cancellationToken = default);
    Task UpdateAsync(Educator educator, CancellationToken cancellationToken = default);
    Task DeleteAsync(EducatorId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Educator>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Educator> GetAsync(EducatorId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(EducatorId id, CancellationToken cancellationToken = default);
}
