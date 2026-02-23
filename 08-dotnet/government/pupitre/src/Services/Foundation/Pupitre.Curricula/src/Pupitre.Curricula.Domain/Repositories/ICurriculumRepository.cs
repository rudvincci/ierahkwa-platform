using System;
using Pupitre.Curricula.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Curricula.Domain.Repositories;

internal interface ICurriculumRepository
{
    Task AddAsync(Curriculum curriculum, CancellationToken cancellationToken = default);
    Task UpdateAsync(Curriculum curriculum, CancellationToken cancellationToken = default);
    Task DeleteAsync(CurriculumId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Curriculum>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Curriculum> GetAsync(CurriculumId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CurriculumId id, CancellationToken cancellationToken = default);
}
